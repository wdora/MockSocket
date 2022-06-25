using System.Net;

namespace MockSocket.Core.Tcp
{
    public interface ITcpServerConnection : ITcpConnection
    {
        ValueTask ListenAsync(IPEndPoint localEP);

        ValueTask<ITcpConnection> AcceptAsync(CancellationToken cancellation = default);
    }
}
