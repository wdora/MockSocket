using MockSocket.Core.Tcp;
using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace MockSocket.Abstractions.Tcp
{
    public static class TcpConnectionExtensions
    {
        static byte lengthTag = (byte)':';

        public static async ValueTask<T> GetAsync<T>(this ITcpConnection connection, CancellationToken cancellationToken = default)
        {
            var json = await GetStringAsync(connection, cancellationToken);

            return JsonSerializer.Deserialize<T>(json)!;
        }

        public static async ValueTask<string> GetStringAsync(this ITcpConnection connection, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4096);

            Memory<byte> memory = buffer;

            try
            {
                var offset = -1;
                while (true)
                {
                    if (offset > 1000)
                        throw new SocketException((int)SocketError.InvalidArgument);
                    var temp = memory.Slice(++offset, 1);
                    await connection.ReceiveAsync(temp, cancellationToken);
                    if (temp.Span[0] == lengthTag)
                        break;
                }
                var length = int.Parse(Encoding.UTF8.GetString(memory.Span.Slice(0, offset)));

                var size = 0;
                while (size != length)
                {
                    size += await connection.ReceiveAsync(memory.Slice(offset + 1, length), cancellationToken);

                    if (size == 0)
                        throw new SocketException((int)SocketError.ConnectionAborted);

                    if (size > length)
                        throw new SocketException((int)SocketError.InvalidArgument);
                }

                return Encoding.UTF8.GetString(memory.Span.Slice(offset + 1, size));
            }
            catch (Exception)
            {
                ArrayPool<byte>.Shared.Return(buffer);
                throw;
            }
        }

        public static async ValueTask SendAsync<T>(this ITcpConnection connection, T model, CancellationToken cancellationToken = default)
        {
            var data = JsonSerializer.Serialize(model);

            var prefix = Encoding.UTF8.GetByteCount(data) + ":";

            var buffer = ArrayPool<byte>.Shared.Rent(4096);

            try
            {
                Memory<byte> memory = buffer;

                var size = Encoding.UTF8.GetBytes(prefix, memory.Span);

                size += Encoding.UTF8.GetBytes(data, memory.Span.Slice(size));

                await connection.SendAsync(memory.Slice(0, size), cancellationToken);
            }
            catch (Exception)
            {
                ArrayPool<byte>.Shared.Return(buffer);
                throw;
            }

        }

        public static bool IsConnected(this Socket so)
        {
            try
            {
                return !(so.Poll(1, SelectMode.SelectRead) && so.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public static async Task OnClosedAsync(this ITcpConnection connection, Action<ITcpConnection> connectionClosed, CancellationToken cancellationToken)
        {
            while (connection.IsConnected && !cancellationToken.IsCancellationRequested)
                await Task.Delay(1);

            connectionClosed(connection);
        }
    }
}
