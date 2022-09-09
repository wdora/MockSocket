using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using MockSocket.HoleClient;

namespace MockSocket.Forward
{
    public class TcpForwardServer : IForwardServer
    {
        private readonly ClientOptions options;
        private readonly ITcpServerConnection tcpServer;
        private readonly TcpClientConnectionFactory tcpClientConnectionFactory;
        private readonly IExchangeConnection exchangeConnection;

        public TcpForwardServer(IOptions<ClientOptions> options, ITcpServerConnection tcpServer, TcpClientConnectionFactory tcpClientConnectionFactory, IExchangeConnection exchangeConnection)
        {
            this.options = options.Value;
            this.tcpServer = tcpServer;
            this.tcpClientConnectionFactory = tcpClientConnectionFactory;
            this.exchangeConnection = exchangeConnection;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await tcpServer.ListenAsync(options.HoleAppServerPort);

            while (true)
            {
                var client = await tcpServer.AcceptAsync(cancellationToken);

                _ = SwapAsync(client);
            }
        }

        private async ValueTask SwapAsync(ITcpConnection client)
        {
            var proxy = await tcpClientConnectionFactory.CreateAsync(options.AgentRealServerEP);

            await exchangeConnection.ExchangeAsync(client, proxy);
        }
    }
}
