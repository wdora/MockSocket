using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Models;
using MockSocket.Udp.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UserClientToDataClientCmdHandler : IRequestHandler<UserClientToDataClientCmd>
{
    IMemoryCache cache;
    ILogger logger;

    public UserClientToDataClientCmdHandler(IMemoryCache cache, ILogger<UserClientToDataClientCmdHandler> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public async Task Handle(UserClientToDataClientCmd request, CancellationToken cancellationToken)
    {
        if (!cache.TryGetValue<UserClientContext>(request.userClientId, out var context))
            return;

        if (context.DataClientEP == null)
        {
            logger.LogDebug("当前userClient {0}等待配对中...", request.userClientId);
            return;
        }

        while (context.Queue.TryDequeue(out var byteArray))
        {
            using var buffer = byteArray.Buffer;

            await CurrentContext.MockServer.SendToAsync(context.DataClientEP, buffer.SliceTo(byteArray.Length), cancellationToken);
        }

        logger.LogDebug("userClient({0}) 与dataClient({1}) 交换数据完成", request.userClientId, context.DataClientEP);
    }
}
