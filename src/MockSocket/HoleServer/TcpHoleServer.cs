using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Tcp;
using MockSocket.Extensions;
using MockSocket.Message;
using System.Net;

namespace MockSocket.HoleServer
{
    public class HoleServerOption
    {
        public int ListenPort { get; set; } = 9090;
    }

    public class TcpHoleServer : IHoleServer
    {
        private readonly ITcpServerConnection server;
        private readonly HoleServerOption option;
        private readonly IMediator mediator;
        private readonly ILogger<TcpHoleServer> logger;

        public TcpHoleServer(ITcpServerConnection server, IOptions<HoleServerOption> option, IMediator mediator, ILogger<TcpHoleServer> logger)
        {
            this.server = server;
            this.option = option.Value;
            this.mediator = mediator;
            this.logger = logger;
        }

        public void Dispose() => server?.Dispose();

        public ValueTask StartAsync()
        {
            return default;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            var ep = new IPEndPoint(IPAddress.Any, option.ListenPort);

            await server.ListenAsync(ep);

            logger.LogInformation($"TcpHoleServer is started on {ep}");

            while (true)
            {
                var agent = await server.AcceptAsync(cancellationToken);

                _ = HandleAgentAsync(agent, cancellationToken);
            }
        }

        private async ValueTask HandleAgentAsync(ITcpConnection agentConnection, CancellationToken cancellationToken)
        {
            using var agent = agentConnection;

            while (true)
            {
                var message = await agent.GetMessageAsync(cancellationToken);

                await mediator.Send(message, cancellationToken);
            }
        }
    }
}
