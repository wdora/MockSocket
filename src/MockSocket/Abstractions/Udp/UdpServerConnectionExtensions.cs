using System.Net;

namespace MockSocket.Abstractions.Udp
{
    public static class UdpServerConnectionExtensions
    {
        public static ValueTask ListenAsync(this IUdpServerConnection serverConnection, int port)
            => serverConnection.ListenAsync(new IPEndPoint(IPAddress.Any, port));
    }
}
