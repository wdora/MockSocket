using MediatR;
using Microsoft.Extensions.Options;
using MockSocket.Tcp.Commands;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;

namespace MockSocket.Tcp.Handlers;
public class TS2CInitUserClientHandler : IRequestHandler<TS2CInitUserClientCmd>
{
    MockAgentConfig config;
    ITcpClient realClient;
    ITcpClient dataClient;
    ITcpPairService pairService;

    public TS2CInitUserClientHandler(ITcpClient realClient, ITcpClient dataClient, ITcpPairService pairService, IOptions<MockAgentConfig> options)
    {
        this.realClient = realClient;
        this.dataClient = dataClient;
        this.pairService = pairService;
        config = options.Value;
    }

    public async Task Handle(TS2CInitUserClientCmd request, CancellationToken cancellationToken)
    {
        await dataClient.ConnectAsync(config.MockServerEP);

        await dataClient.SendAsync(new TC2SInitDataAgentCmd(request.UserClientId), cancellationToken);

        await realClient.ConnectAsync(config.RealServerEP);

        _ = pairService.PairAsync(dataClient, realClient, cancellationToken);
    }
}
