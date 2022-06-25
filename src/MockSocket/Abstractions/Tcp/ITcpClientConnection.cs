using System.Net;

namespace MockSocket.Core.Tcp
{
    public interface ITcpClientConnection : ITcpConnection
    {
        ValueTask ConnectAsync(IPEndPoint remoteEP, CancellationToken cancellationToken = default);
    }
}
