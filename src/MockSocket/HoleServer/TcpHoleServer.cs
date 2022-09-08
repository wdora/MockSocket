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

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            var ep = new IPEndPoint(IPAddress.Any, option.ListenPort);

            await server.ListenAsync(ep);

            logger.LogInformation($"TcpHoleServer is started on {ep}");

            while (true)
            {
                var client = await server.AcceptAsync(cancellationToken);

                if (client == null)
                    break;

                _ = HandleAgentAsync(client, cancellationToken);
            }
        }

        private async ValueTask HandleAgentAsync(ITcpConnection clientConnection, CancellationToken token)
        {
            using var client = clientConnection;

            var cancellationToken = CheckConnection(client, token);

            while (true)
            {
                var message = await client.GetMessageAsync(cancellationToken);

                await mediator.Send(message, cancellationToken);
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
