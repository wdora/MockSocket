using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Connection.Tcp;
using MockSocket.Message.Udp;

namespace MockSocket.HoleClient
{
    public class UdpHoleClient : IHoleClient
    {
        ClientOptions config;
        private TcpClientConnection agent;

        public UdpHoleClient(IOptions<ClientOptions> option)
        {
            this.config = option.Value;
        }

        public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            agent = new TcpClientConnection();

            await agent.ConnectAsync(config.HoleServerEP);

            await agent.SendAsync(new UdpCtrlAgentInitMessage { AppServerPort = config.HoleAppServerPort });
        }

        public void Dispose() => agent?.Dispose();
    }
}
