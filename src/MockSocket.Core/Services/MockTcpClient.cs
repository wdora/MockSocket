using MockSocket.Core.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace MockSocket.Core.Services
{
    public class MockTcpClient : TcpSocketClient, IMockTcpClient
    {
        private const int HEADCOUNT = 5;

        public MockTcpClient(Socket client) : base(client) { }

        public string Id => $"{_socket.LocalEndPoint}-{_socket.RemoteEndPoint}";

        public ValueTask SendCmdAsync<T>(T model, CancellationToken cancellationToken = default)
        {
            return BufferPool.Instance.Run(async buffer =>
            {
                var dataLen = JsonEncodeService.Instance.Encode(model, buffer.Slice(HEADCOUNT));

                var totalLen = Encode<T>(buffer, dataLen);

                await SendAsync(data: buffer.Slice(0, totalLen), cancellationToken);
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

            return dataLen + typeNameLength + HEADCOUNT;
        }

        public ValueTask<T> ReceiveCmdAsync<T>(CancellationToken cancellationToken = default)
        {
            return BufferPool.Instance.Run(async buffer =>
            {
                var len = await ReceiveAsync(data: buffer.Slice(0, HEADCOUNT), cancellationToken);

                if (len != HEADCOUNT)
                    throw new ArgumentOutOfRangeException(nameof(len), len, $"len != HEADCOUNT: {new { len, HEADCOUNT }}");

                ReadLength(buffer, out var dataLen, out var typeNameLength, out var totalLength);

                if (totalLength > buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(totalLength), totalLength, $"dataLen > buffer.Length: {new { totalLength, buffer.Length }}");

                len = await ReceiveAsync(data: buffer.Slice(0, totalLength), cancellationToken);

                if (len != totalLength)
                    throw new ArgumentOutOfRangeException(nameof(len), len, $"len != dataLen: {new { len, totalLength }}");

                (var type, buffer) = Decode(buffer, dataLen, typeNameLength);

                return (T)JsonEncodeService.Instance.Decode(buffer, dataLen, type)!;
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

            return (Type.GetType(typeNameRaw)!, buffer.Slice(0, dataLen));
        }

        public CancellationToken Register(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _ = Cancel(cts);

            return cts.Token;
        }

        private async ValueTask Cancel(CancellationTokenSource cts)
        {
            try
            {
                while (true)
                {
                    bool part1 = _socket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = _socket.Available == 0;
                    if (part1 && part2)
                        return;

                    // usually { part1 = False, part2 = True }
                    await Task.Delay(1000);
                }
            }
            finally
            {
                Console.WriteLine("主动检测到连接断开");
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}