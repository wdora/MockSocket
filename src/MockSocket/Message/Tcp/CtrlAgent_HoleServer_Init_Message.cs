using MediatR;
using Microsoft.Extensions.Logging;
using MockSocket.Abstractions.Tcp;
using MockSocket.Cache;
using MockSocket.Core.Tcp;
using MockSocket.Extensions;

namespace MockSocket.Message.Tcp
{
    public class CtrlAgent_HoleServer_Init_Message : TcpBaseMessage, IRequest
    {
        public int AppServerPort { get; set; }
    }

    public class CtrlAgentInitHandle : IRequestHandler<CtrlAgent_HoleServer_Init_Message>
    {
        private readonly ITcpServerConnection tcpServerConnection;
        private readonly ICacheService cacheService;
        private readonly ILogger<CtrlAgentInitHandle> logger;

        public CtrlAgentInitHandle(ITcpServerConnection tcpServerConnection, ICacheService cacheService, ILogger<CtrlAgentInitHandle> logger)
        {
            this.tcpServerConnection = tcpServerConnection;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<Unit> Handle(CtrlAgent_HoleServer_Init_Message request, CancellationToken cancellationToken)
        {
            var ctrlAgent = request.Connection;

            var appPort = request.AppServerPort;

            await tcpServerConnection.ListenAsync(appPort);

            var token = CheckConnection(ctrlAgent, cancellationToken);

            _ = LoopAppServer(ctrlAgent, appPort, token);

            return Unit.Value;
        }

        private async Task LoopAppServer(ITcpConnection ctrlAgent, int appPort, CancellationToken cancellationToken)
        {
            var id = ctrlAgent.ToString();

            logger.LogInformation($"{id} request to listen on {appPort} success");

            try
            {
                using var appServer = tcpServerConnection;

                while (true)
                {
                    var userClient = await appServer.AcceptAsync(cancellationToken);

                    var key = userClient.ToString()!;

                    await ctrlAgent.SendAsync(new AppServer_CtrlAgent_NewClient_Message { ClientId = key });

                    cacheService.Add(key, userClient);
                }
            }
            finally
            {
                logger.LogInformation($"{id} disconnect and the {appPort} disposed");
            }
        }

        private static CancellationToken CheckConnection(ITcpConnection client, CancellationToken token)
        {
            var (cts, cancellationToken) = token.CreateChildToken();

            _ = client.OnClosedAsync(_ => cts.Cancel(), cancellationToken);

            return cancellationToken;
        }
    }
}
