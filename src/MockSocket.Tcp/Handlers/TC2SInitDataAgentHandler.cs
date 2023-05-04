using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MockSocket.Tcp.Commands;
using MockSocket.Tcp.Interfaces;
using MockSocket.Tcp.Utilities;

namespace MockSocket.Tcp.Handlers;
public class TC2SInitDataAgentHandler : IRequestHandler<TC2SInitDataAgentCmd>
{
    private IMemoryCache cache;
    private ITcpPairService pairService;

    public TC2SInitDataAgentHandler(IMemoryCache cache, ITcpPairService pairService)
    {
        this.cache = cache;
        this.pairService = pairService;
    }

    public async Task Handle(TC2SInitDataAgentCmd request, CancellationToken cancellationToken)
    {
        var agentClient = CurrentContext.Agent.Value!;

        var userClient = cache.Get<ITcpClient>(request.UserClientId)!;

        _ = pairService.PairAsync(agentClient, userClient, cancellationToken);

        cache.Remove(request.UserClientId);
    }
}
