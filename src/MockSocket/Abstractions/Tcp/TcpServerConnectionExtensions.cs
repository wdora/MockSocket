using MockSocket.Core.Tcp;
using System.Net;

namespace MockSocket.Abstractions.Tcp
{
    public static class TcpServerConnectionExtensions
    {
        public static ValueTask ListenAsync(this ITcpServerConnection serverConnection, int port)
            => serverConnection.ListenAsync(new IPEndPoint(IPAddress.Any, port));
    }
}
