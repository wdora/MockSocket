using MockSocket.Message;
using System.Buffers;
using System.Net.Sockets;

namespace MockSocket.Agent
{
    public interface ITcpAgent
    {
        ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken = default);

        ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
    }

    public class TcpAgent : ITcpAgent
    {
        readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
    }

    public static class TcpAgentExtension
    {
        public static ValueTask SendAsync<T>(this ITcpAgent tcpAgent, T model, CancellationToken cancellationToken = default)
        {
            return BufferPool.Instance.Run(buffer =>
            {
                var len = FastEncoder.Instance.Encode(model, buffer.Span);

                return tcpAgent.SendAsync(buffer.Slice(0, len), cancellationToken);
            });
        }

        public static ValueTask<T> ReceiveAsync<T>(this ITcpAgent tcpAgent, CancellationToken cancellationToken = default)
            where T : class
        {
            return BufferPool.Instance.Run(async buffer =>
            {
                var len = await tcpAgent.ReceiveAsync(buffer.Slice(0, 4), cancellationToken);

                var dataLen = BitConverter.ToInt32(buffer.Slice(0, 4).Span);

                if (dataLen > buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(dataLen), dataLen, $"dataLen > buffer.Length: {new { dataLen, buffer.Length }}");

                len = await tcpAgent.ReceiveAsync(buffer.Slice(0, dataLen), cancellationToken);

                if (len != dataLen)
                    throw new ArgumentOutOfRangeException(nameof(dataLen), dataLen, $"len != dataLen: {new { len, dataLen }}");

                // https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/memory-t-usage-guidelines
                // Memory.Slice().Span
                return FastEncoder.Instance.Decode<T>(buffer.Slice(0, dataLen).Span);
            });
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
