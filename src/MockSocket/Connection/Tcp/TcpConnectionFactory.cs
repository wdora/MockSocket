using MockSocket.Connection.Tcp;
using System.Net;

namespace MockSocket.Core.Tcp
{
    public class TcpClientConnectionFactory
    {
        public async ValueTask<TcpConnection> CreateAsync(EndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            var client = new TcpClientConnection();

            await client.ConnectAsync(remoteEP, cancellationToken);

            return client;
        }
    }
}
