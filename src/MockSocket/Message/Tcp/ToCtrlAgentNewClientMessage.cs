using MediatR;
using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using MockSocket.HoleClient;

namespace MockSocket.Message.Tcp
{
    public class ToCtrlAgentNewClientMessage : TcpBaseMessage, IRequest
    {
        public string ClientId { get; set; } = "";
    }

    public class ToCtrlAgentNewClientHandle : IRequestHandler<ToCtrlAgentNewClientMessage>
    {
        private readonly TcpClientConnectionFactory tcpClientConnectionFactory;
        private readonly HoleClientOptions options;
        private readonly IExchangeConnection exchangeConnection;

        public ToCtrlAgentNewClientHandle(TcpClientConnectionFactory tcpClientConnectionFactory, IOptions<HoleClientOptions> options, IExchangeConnection exchangeConnection)
        {
            this.tcpClientConnectionFactory = tcpClientConnectionFactory;
            this.options = options.Value;
            this.exchangeConnection = exchangeConnection;
        }


        public async Task<Unit> Handle(ToCtrlAgentNewClientMessage request, CancellationToken cancellationToken)
        {
            var remoteEP = options.HoleServerEP;
            var realServerEP = options.AgentRealServerEP;

            var agentDataClient = await tcpClientConnectionFactory.CreateAsync(remoteEP, cancellationToken);

            _ = agentDataClient.SendAsync(new FromDataAgentInitMessage { UserClientId = request.ClientId });

            var realClient = await tcpClientConnectionFactory.CreateAsync(realServerEP, cancellationToken);

            await exchangeConnection.ExchangeAsync(agentDataClient, realClient, cancellationToken);

            return Unit.Value;
        }
    }
}
