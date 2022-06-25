using System.Net;

namespace MockSocket.Core.Tcp
{
    public interface ITcpClientConnection : ITcpConnection
    {
        ValueTask ConnectAsync(EndPoint remoteEP, CancellationToken cancellationToken = default);
    }
}
