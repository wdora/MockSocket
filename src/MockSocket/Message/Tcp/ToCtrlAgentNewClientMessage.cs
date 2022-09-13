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
        private readonly ClientOptions options;
        private readonly IExchangeConnection exchangeConnection;

        public ToCtrlAgentNewClientHandle(TcpClientConnectionFactory tcpClientConnectionFactory, IOptions<ClientOptions> options, IExchangeConnection exchangeConnection)
        {
            this.tcpClientConnectionFactory = tcpClientConnectionFactory;
            this.options = options.Value;
            this.exchangeConnection = exchangeConnection;
        }


        public async Task<Unit> Handle(ToCtrlAgentNewClientMessage request, CancellationToken cancellationToken)
        {
            var remoteEP = options.HoleServerEP;
            var realServerEP = options.AgentRealServerEP;

            var agentDataClient = await tcpClientConnectionFactory.CreateAsync(remoteEP, cancellationToken).AsTask();

            await agentDataClient.SendAsync(new FromDataAgentInitMessage { UserClientId = request.ClientId });

            var realClient = await tcpClientConnectionFactory.CreateAsync(realServerEP, cancellationToken).AsTask();

            _ = exchangeConnection.ExchangeAsync(agentDataClient, realClient, cancellationToken);

            return Unit.Value;
        }
    }
}
