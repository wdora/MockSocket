using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Models;
using MockSocket.Udp.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UserDataCommandHandler : IRequestHandler<UserDataCommand>
{
    IMemoryCache cache;
    ISender sender;
    ILogger logger;

    MockServerConfig config = new MockServerConfig();

    public UserDataCommandHandler(IMemoryCache cache, ISender sender, ILogger<UserDataCommandHandler> logger)
    {
        this.cache = cache;
        this.sender = sender;
        this.logger = logger;
    }

    public async Task Handle(UserDataCommand request, CancellationToken cancellationToken)
    {
        var key = request.UserClientEP.ToString();

        var context = await cache.GetOrCreateAsync(key, async entry =>
        {
            await CurrentContext.MockServer.SendAsync(request.AgentEP, new US2CCreateDataClient(key), cancellationToken);

            // todo 滑动30s -> userClient 失效
            entry.SetSlidingExpiration(TimeSpan.FromSeconds(config.ExpireUdpTime));

            entry.RegisterPostEvictionCallback((k, v, r, s) => logger.LogInformation("{k} 由于{r}，被自动移除", k, r));

            return new UserClientContext(request.udpAppServer, request.UserClientEP);
        });

        context.Queue!.Enqueue(new ByteArray(request.Buffer, request.Length));

        await sender.Send(new UserClientToDataClientCmd(key));
    }
}
