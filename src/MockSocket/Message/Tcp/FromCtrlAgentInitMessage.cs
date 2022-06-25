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
        ITcpServerConnection tcpServerConnection;
        ICacheService cacheService;
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

            await tcpServerConnection.ListenAsync(request.AppServerPort);

            logger.LogInformation($"{ctrlAgent} request to listen on {request.AppServerPort} success");

            _ = LoopAppServer(ctrlAgent, cancellationToken)
                .ContinueWith(t => logger.LogInformation($"{request.AppServerPort} disposed"));

            return Unit.Value;
        }

        private async Task LoopAppServer(ITcpConnection ctrlAgent, CancellationToken cancellationToken)
        {
            using var appServer = tcpServerConnection;

            while (true)
            {
                var userClient = await appServer.AcceptAsync(cancellationToken);

                var key = userClient.ToString()!;

                cacheService.Add(key, userClient);

                await ctrlAgent.SendAsync(new ToCtrlAgentNewClientMessage { ClientId = key });
            }
        }
    }
}
