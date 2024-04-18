using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UC2SHeartBeatHandler : IRequestHandler<UC2SHeartBeat>
{
    IMemoryCache memoryCache;

    MockServerConfig config;

    public UC2SHeartBeatHandler(IMemoryCache memoryCache, IOptions<MockServerConfig> options)
    {
        this.memoryCache = memoryCache;

        config = options.Value;
    }

    public Task Handle(UC2SHeartBeat request, CancellationToken cancellationToken)
    {
        var exp = TimeSpan.FromSeconds(request.Interval + config.HeartInterval);

        memoryCache.Set(CurrentContext.ClientEP, true, exp);

        return Task.CompletedTask;

        //await CurrentContext.MockServer.SendAsync(CurrentContext.ClientEP, new US2CHeartBeat(), cancellationToken);
    }
}

public class US2CHeartBeatHandler : IRequestHandler<US2CHeartBeat>
{
    public Task Handle(US2CHeartBeat request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}