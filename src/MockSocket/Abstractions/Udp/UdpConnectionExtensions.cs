using MockSocket.Message;
using System.Buffers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MockSocket.Abstractions.Udp
{
    public static class UdpConnectionExtensions
    {
        public static async ValueTask SendAsync<T>(this IUdpConnection connection, EndPoint remoteEP, T model, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(MessageEncoding.BUFFER_SIZE);

            try
            {
                Memory<byte> memory = buffer;

                var len = MessageEncoding.Encode(model, buffer);

                var data = memory.Slice(0, len);

                await connection.SendAsync(remoteEP, data, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
