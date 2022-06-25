using System.Net;
using System.Text;
using System.Text.Json;

namespace MockSocket.Abstractions.Udp
{
    public static class UdpConnectionExtensions
    {
        public static async ValueTask SendAsync<T>(this IUdpConnection connection, EndPoint remoteEP, T model, CancellationToken cancellationToken = default)
        {
            var data = JsonSerializer.Serialize(model);

            var bytes = Encoding.UTF8.GetBytes(data);

            await connection.SendAsync(remoteEP, bytes, cancellationToken);
        }
    }
}
