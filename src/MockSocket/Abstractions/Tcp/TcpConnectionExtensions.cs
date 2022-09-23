using MockSocket.Core.Tcp;
using MockSocket.Message;
using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

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
            var buffer = ArrayPool<byte>.Shared.Rent(MessageEncoding.BUFFER_SIZE);

            try
            {
                Memory<byte> memory = buffer;

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

                var length = MessageEncoding.FastGetTagLength(memory.Span.Slice(0, offset));

                await connection.ReceiveAsync(memory.Slice(0, length), cancellationToken);

                return MessageEncoding.Default.GetString(memory.Span.Slice(0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static async ValueTask SendAsync<T>(this ITcpConnection connection, T model, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(MessageEncoding.BUFFER_SIZE);

            try
            {
                Memory<byte> memory = buffer;

                var len = MessageEncoding.Encode(model, buffer);

                var data = memory.Slice(0, len);

                await connection.SendAsync(data, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static bool IsConnected(this Socket so)
        {
            try
            {
                return !(so.Poll(1, SelectMode.SelectRead) && so.Available == 0);
            }
            catch (Exception)
            {
                // 如：System.ObjectDisposedException: Cannot access a disposed object
                return false;
            }
        }

        static async Task OnClosedAsync(this ITcpConnection connection, Action<ITcpConnection> connectionClosed, CancellationToken cancellationToken)
        {
            // connection.IsConnected 存在瞬态false情况
            while ((connection.IsConnected ? true : connection.IsConnected) && !cancellationToken.IsCancellationRequested)
                await Task.Delay(1000);

            connectionClosed(connection);
        }

        public static Task OnClosedAsync(this ITcpConnection connection, CancellationTokenSource cancellationTokenSource)
            => OnClosedAsync(connection, _ =>
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }, cancellationTokenSource.Token);
    }
}
