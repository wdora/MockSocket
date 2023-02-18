using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace MockSocket.Agent
{
    public interface ITcpConnection
    {
        ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
    }

    public interface ITcpClient : ITcpConnection, IDisposable
    {
        ValueTask DisconnectAsync();

        ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken = default);
    }

    public abstract class TcpSocketClient : ITcpClient
    {
        readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public ValueTask DisconnectAsync()
        {
            return _socket.DisconnectAsync(false);
        }

        public ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            return _socket.ConnectAsync(host, port, cancellationToken);
        }

        public ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default)
        {
            return _socket.ReceiveAsync(data, cancellationToken);
        }

        public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            await _socket.SendAsync(data, cancellationToken);

            return;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }

    public class MockTcpClient : TcpSocketClient
    {
        private const int HEADCOUNT = 5;

        public ValueTask SendAsync<T>(T model, CancellationToken cancellationToken = default)
        {
            return BufferPool.Instance.Run(buffer =>
            {
                var dataLen = JsonEncoder.Instance.Encode(model, buffer.Slice(HEADCOUNT));

                var totalLen = Encode<T>(buffer, dataLen);

                return SendAsync(buffer.Slice(0, totalLen), cancellationToken);
            });
        }

        private int Encode<T>(Memory<byte> buffer, int dataLen)
        {
            // Header + Body
            // Header: 4Bytes BodyByte's Length + 1Byte TypeNameByte's Length
            // Body: Encoding.UTF8.GetBytes(json + TypeName)
            BitConverter.TryWriteBytes(buffer.Span, dataLen);

            // https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/memory-t-usage-guidelines
            // Recommend: Memory.Slice.Span
            var typeNameLength = (byte)Encoding.UTF8.GetBytes(typeof(T).FullName, buffer.Slice(dataLen + HEADCOUNT).Span);

            buffer.Span[HEADCOUNT - 1] = typeNameLength;

            return dataLen + typeNameLength;
        }

        public ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken = default)
        {
            return BufferPool.Instance.Run(async buffer =>
            {
                var len = await ReceiveAsync(buffer.Slice(0, HEADCOUNT), cancellationToken);

                if (len != HEADCOUNT)
                    throw new ArgumentOutOfRangeException(nameof(len), len, $"len != HEADCOUNT: {new { len, HEADCOUNT }}");

                ReadLength(buffer, out var dataLen, out var typeNameLength, out var totalLength);

                if (totalLength > buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(totalLength), totalLength, $"dataLen > buffer.Length: {new { totalLength, buffer.Length }}");

                len = await ReceiveAsync(buffer.Slice(0, totalLength), cancellationToken);

                if (len != totalLength)
                    throw new ArgumentOutOfRangeException(nameof(len), len, $"len != dataLen: {new { len, totalLength }}");

                (var type, buffer) = Decode(buffer, dataLen, typeNameLength);

                return (T)JsonEncoder.Instance.Decode(buffer, totalLength, type);
            });
        }

        private void ReadLength(Memory<byte> buffer, out int dataLen, out byte typeNameLength, out int totalLength)
        {
            dataLen = BitConverter.ToInt32(buffer.Span);
            typeNameLength = buffer.Span[HEADCOUNT - 1];
            totalLength = dataLen + typeNameLength;
        }

        private (Type type, Memory<byte> buffer) Decode(Memory<byte> buffer, int dataLen, byte typeNameLength)
        {
            var typeNameRaw = Encoding.UTF8.GetString(buffer.Slice(dataLen, typeNameLength).Span);

            return (Type.GetType(typeNameRaw), buffer.Slice(0, dataLen));
        }
    }

    public interface IMockAgent
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }

    public class MockAgent : IMockAgent
    {
        private readonly MockAgentConfig config;
        private readonly ILogger<MockAgent> logger;
        private readonly IPairService pairService;

        private MockTcpClient agent;

        public MockAgent(IOptions<MockAgentConfig> config, ILogger<MockAgent> logger, IPairService pairService)
        {
            this.config = config.Value;
            this.logger = logger;
            this.pairService = pairService;
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            return Retry(StartCoreAsync, ex => ex is not AppServerException, cancellationToken);
        }

        private async ValueTask Retry(Func<CancellationToken, ValueTask> startCoreAsync, Func<Exception, bool> checkExp, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await startCoreAsync(cancellationToken);

                    return;
                }
                catch (Exception e)
                {
                    if (!checkExp(e))
                        throw;
                }

                var delay = TimeSpan.FromSeconds(config.RetryInterval);

                logger.LogError($"连接故障，{delay} 后重新连接");

                await Task.Delay(delay);
            }
        }

        public async ValueTask StartCoreAsync(CancellationToken cancellationToken)
        {
            var (host, port) = config.RemoteServer;

            agent = await TcpSocketFactory.Create(host, port);

            using var client = agent;

            await CreateAppServerAsync();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var heartTask = HeartBeatAsync(cts);

            var handleTask = HandleNewClientAsync(cts.Token);

            await Task.WhenAny(heartTask, handleTask);
        }

        private async Task HandleNewClientAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var userClientId = await agent.ReceiveAsync<string>();

                try
                {
                    var realClient = await CreateRealClientAsync();

                    var dataClient = await CreateDataClientAsync(userClientId);

                    _ = pairService.PairAsync(realClient, dataClient, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError("服务请求失败", e);
                }
            }
        }

        private async ValueTask<MockTcpClient> CreateDataClientAsync(string userClientId)
        {
            var (host, port) = config.RemoteServer;

            var connection = await TcpSocketFactory.Create(host, port);

            await connection.SendAsync(new DataClientCmd(userClientId));

            return connection;
        }

        private ValueTask<MockTcpClient> CreateRealClientAsync()
        {
            var (host, port) = config.RealServer;

            return TcpSocketFactory.Create(host, port);
        }

        private async ValueTask CreateAppServerAsync()
        {
            var appServer = config.AppServer;

            await agent.SendAsync(new CreateAppServerCmd(appServer.Port, appServer.Protocal));

            var isOk = await agent.ReceiveAsync<bool>();

            if (!isOk)
                throw new AppServerException($"无法监听: {config.AppServer}");
        }

        private async Task HeartBeatAsync(CancellationTokenSource cancellationTokenSource)
        {
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                var heartInterval = TimeSpan.FromSeconds(config.HeartInterval);

                while (true)
                {
                    await agent.SendAsync((DateTime.Now, config.HeartInterval), cancellationToken);

                    await Task.Delay(heartInterval, cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "连接断开");

                cancellationTokenSource.Cancel();

                throw;
            }
        }
    }

    public interface IPairService
    {
        ValueTask PairAsync(MockTcpClient realClient, MockTcpClient dataClient, CancellationToken cancellationToken);
    }

    public class PairService : IPairService
    {
        public ValueTask PairAsync(MockTcpClient realClient, MockTcpClient dataClient, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICmd
    {
    }

    public record class CreateAppServerCmd(int port, string type) : ICmd;

    public record class DataClientCmd(string userClientId) : ICmd;

    internal class TcpSocketFactory
    {
        internal static async ValueTask<MockTcpClient> Create(string host, int port)
        {
            var client = new MockTcpClient();

            await client.ConnectAsync(host, port);

            return client;
        }
    }

    public class AppServerException : Exception
    {
        public AppServerException(string? message) : base(message)
        {
        }
    }

    public class MockAgentConfig
    {
        public ServerEndpoint RemoteServer { get; internal set; } = new("wdora.com", 10000);

        public ServerEndpoint RealServer { get; internal set; } = new("localhost", 80);

        public AppServer AppServer { get; internal set; } = new(8080, "tcp");

        public int HeartInterval { get; internal set; } = 30;

        public int RetryInterval { get; internal set; } = 3;
    }

    public record ServerEndpoint(string Host, int Port);

    public record AppServer(int Port, string Protocal)
    {
        public override string ToString()
        {
            // net.tcp://0.0.0.0:8080
            return $"net.{Protocal}://0.0.0.0:{Port}";
        }
    }

    public class BufferPool
    {
        private readonly int BUFFER_SIZE = 1024;

        public static BufferPool Instance { get; } = new BufferPool();

        public async ValueTask Run(Func<Memory<byte>, ValueTask> func)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            try
            {
                await func(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public async ValueTask<T> Run<T>(Func<Memory<byte>, ValueTask<T>> func)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            try
            {
                var model = await func(buffer);

                return model;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}