using MediatR;
using Microsoft.Extensions.Logging;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Tcp;
using MockSocket.Message;
using MockSocket.Message.Tcp;
using Microsoft.Extensions.Options;

namespace MockSocket.HoleClient
{
    public class TcpHoleClient : IHoleClient
    {
        private readonly ITcpClientConnection client;
        private readonly HoleClientOptions options;
        private readonly IMediator mediator;
        private readonly ILogger<TcpHoleClient> logger;

        public TcpHoleClient(ITcpClientConnection client, IMediator mediator, ILogger<TcpHoleClient> logger, IOptions<HoleClientOptions> options)
        {
            this.client = client;
            this.mediator = mediator;
            this.logger = logger;
            this.options = options.Value;
        }

        public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            await client.ConnectAsync(options.HoleServerEP);

            await client.SendAsync(new FromCtrlAgentInitMessage { AppServerPort = options.HoleAppServerPort });

            logger.LogInformation($"request app Server: {options.HoleAppServerPort}");

            _ = HeartBeatAsync();

            while (true)
            {
                var message = await client.GetMessageAsync();

                _ = mediator.Send(message, cancellationToken);
            }
        }

        private async Task HeartBeatAsync()
        {
            while (true)
            {
                await Task.Delay(options.HeartInterval);

                await client.SendAsync(new FromCtrlAgentHeartMessage());
            }
        }

        public void Dispose() => client.Dispose();
    }
}
