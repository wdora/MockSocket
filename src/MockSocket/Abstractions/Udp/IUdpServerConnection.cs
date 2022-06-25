using System.Net;

namespace MockSocket.Abstractions.Udp
{
    public interface IUdpServerConnection : IUdpConnection
    {
        ValueTask ListenAsync(IPEndPoint localEP);
    }
}
