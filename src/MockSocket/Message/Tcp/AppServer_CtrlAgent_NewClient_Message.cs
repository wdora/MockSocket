using MediatR;
using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using MockSocket.HoleClient;

namespace MockSocket.Message.Tcp
{
    public class AppServer_CtrlAgent_NewClient_Message : TcpBaseMessage, IRequest
    {
        public string ClientId { get; set; } = "";
    }

    public class ToCtrlAgentNewClientHandle : IRequestHandler<AppServer_CtrlAgent_NewClient_Message>
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


        public async Task<Unit> Handle(AppServer_CtrlAgent_NewClient_Message request, CancellationToken cancellationToken)
        {
            var remoteEP = options.HoleServerEP;
            var realServerEP = options.AgentRealServerEP;

            var agentDataClient = await tcpClientConnectionFactory.CreateAsync(remoteEP, cancellationToken);

            _ = agentDataClient.SendAsync(new DataAgent_HoleServer_Init_Message { UserClientId = request.ClientId });

            var realClient = await tcpClientConnectionFactory.CreateAsync(realServerEP, cancellationToken);

            _ = exchangeConnection.ExchangeAsync(agentDataClient, realClient, cancellationToken);

            return Unit.Value;
        }
    }
}
