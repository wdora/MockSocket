using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class DataClientToUserClientCmdHandler : IRequestHandler<DataClientToUserClientCmd>
{
    readonly IMemoryCache cache;

    readonly ILogger logger;

    public DataClientToUserClientCmdHandler(IMemoryCache cache, ILogger<DataClientToUserClientCmdHandler> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public async Task Handle(DataClientToUserClientCmd request, CancellationToken cancellationToken)
    {
        if (!cache.TryGetValue<UserClientContext>(request.userClientId, out var context))
        {
            return;
        }

        // appServer -> userClient

        await context.AppServer.SendToAsync(context.UserClientEP, request.buffer.SliceTo(request.length), cancellationToken);

        logger.LogDebug("userClient({0}) 与dataClient({1}) 交换数据完成", request.userClientId, context.DataClientEP);
    }
}
