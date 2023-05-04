using MediatR;
using Microsoft.Extensions.Options;
using MockClient.Udp.Interfaces;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class US2CCreateDataClientHandler : IRequestHandler<US2CCreateDataClient>
{
    readonly IUdpClient dataClient;
    readonly IUdpClient realClient;
    readonly IUdpPairService udpPairService;
    MockAgentConfig agentConfig;

    public US2CCreateDataClientHandler(IUdpClient dataClient, IUdpClient realClient, IUdpPairService udpPairService, IOptions<MockAgentConfig> options)
    {
        this.dataClient = dataClient;
        this.realClient = realClient;
        this.udpPairService = udpPairService;

        agentConfig = options.Value;
    }

    public async Task Handle(US2CCreateDataClient request, CancellationToken cancellationToken)
    {
        dataClient.Connect(agentConfig.MockServerEP);

        await dataClient.SendAsync(new UC2SInitDataClient(request.UserClientId), cancellationToken);

        realClient.Connect(agentConfig.RealServerEP);

        _ = udpPairService.PairAsync(dataClient, realClient, cancellationToken);
    }
}
