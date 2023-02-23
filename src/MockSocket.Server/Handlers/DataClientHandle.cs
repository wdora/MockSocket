using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MockSocket.Core.Commands;
using MockSocket.Core.Interfaces;
using MockSocket.Core.Services;

namespace MockSocket.Server.Handlers
{
    public class DataClientHandle : IRequestHandler<DataClientCmd>
    {
        IMemoryCache cacheService;

        IPairService pairService;

        public DataClientHandle(IMemoryCache cacheService, IPairService pairService)
        {
            this.cacheService = cacheService;
            this.pairService = pairService;
        }

        public async Task Handle(DataClientCmd request, CancellationToken cancellationToken)
        {
            if (!cacheService.TryGetValue<MockTcpClient>(request.UserClientId, out var userClient))
                return;

            cacheService.Remove(request.UserClientId);

            await pairService.PairAsync(userClient!, CurrentContext.Agent, cancellationToken);
        }
    }
}
