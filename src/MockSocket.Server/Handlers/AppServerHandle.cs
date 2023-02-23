using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Core.Commands;
using MockSocket.Core.Interfaces;

namespace MockSocket.Server.Handlers
{
    public class AppServerHandle : IRequestHandler<CreateAppServerCmd>
    {
        IMockTcpServer server;

        IMemoryCache cacheService;

        ILogger logger;

        public AppServerHandle(IMockTcpServer server, IMemoryCache cacheService, ILogger<AppServerHandle> logger)
        {
            this.server = server;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task Handle(CreateAppServerCmd request, CancellationToken cancellationToken)
        {
            await server.ListenAsync(request.Port);

            await CurrentContext.Agent.SendCmdAsync(true);

            _ = LoopAsync(server, cancellationToken);
        }

        private async Task LoopAsync(IMockTcpServer server, CancellationToken cancellationToken)
        {
            using var appServer = server;

            while (true)
            {
                var userClient = await appServer.AcceptAsync(cancellationToken);

                logger.LogInformation($"userClient {userClient.Id} is coming");

                await CurrentContext.Agent.SendCmdAsync(userClient.Id);

                cacheService.Set(userClient.Id, userClient);
            }
        }
    }
}
