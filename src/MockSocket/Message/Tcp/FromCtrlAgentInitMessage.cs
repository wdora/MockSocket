using MediatR;
using Microsoft.Extensions.Logging;
using MockSocket.Abstractions.Tcp;
using MockSocket.Cache;
using MockSocket.Core.Tcp;

namespace MockSocket.Message.Tcp
{
    public class FromCtrlAgentInitMessage : TcpBaseMessage, IRequest
    {
        public int AppServerPort { get; set; }
    }

    public class CtrlAgentInitHandle : IRequestHandler<FromCtrlAgentInitMessage>
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

        public async Task<Unit> Handle(FromCtrlAgentInitMessage request, CancellationToken cancellationToken)
        {
            var ctrlAgent = request.Connection;

            try
            {
                await tcpServerConnection.ListenAsync(request.AppServerPort);

                logger.LogInformation($"{ctrlAgent} request to listen on {request.AppServerPort} success");

                await LoopAppServer(ctrlAgent, cancellationToken);
            }
            finally
            {
                logger.LogInformation($"{ctrlAgent} disconnect and the {request.AppServerPort} disposed");
            }

            return Unit.Value;
        }

        private async Task LoopAppServer(ITcpConnection ctrlAgent, CancellationToken cancellationToken)
        {
            using var appServer = tcpServerConnection;

            while (true)
            {
                var userClient = await appServer.AcceptAsync(cancellationToken);

                var key = userClient.ToString()!;

                await ctrlAgent.SendAsync(new ToCtrlAgentNewClientMessage { ClientId = key });

                cacheService.Add(key, userClient);
            }
        }
    }
}
