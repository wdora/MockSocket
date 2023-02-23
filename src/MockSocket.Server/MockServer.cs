using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Core.Commands;
using MockSocket.Core.Configurations;
using MockSocket.Core.Interfaces;
using MockSocket.Core.Services;

namespace MockSocket.Server
{
    public class MockServer : IMockServer
    {
        MockServerConfig config;

        IMockTcpServer server;

        IMediator mediator;

        ILogger logger;

        public MockServer(IOptions<MockServerConfig> config, IMockTcpServer server, IMediator mediator, ILogger<MockServer> logger)
        {
            this.config = config.Value;
            this.server = server;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await server.ListenAsync(config.ListenPort);

            while (true)
            {
                var agent = await server.AcceptAsync(cancellationToken);

                _ = HandleAgentAsync(agent, cancellationToken);
            }
        }


        private async Task HandleAgentAsync(MockTcpClient agent, CancellationToken cancellationToken)
        {
            using var client = CurrentContext.Agent = agent;

            var token = client.Register(cancellationToken);

            while (true)
            {
                var command = await client.ReceiveAsync<ICmd>(token);

                logger.LogDebug($"{client.Id}: {command}");

                await mediator.Send(command as object, token);
            }
        }
    }
}
