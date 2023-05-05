using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Models;
using MockSocket.Udp.Utilities;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UC2SInitDataClientHandler : IRequestHandler<UC2SInitDataClient>
{
    IMemoryCache cache;

    ISender sender;

    MockServerConfig config;

    public UC2SInitDataClientHandler(IMemoryCache cache, ISender sender, IOptions<MockServerConfig> options)
    {
        this.cache = cache;
        this.sender = sender;

        config = options.Value;
    }

    public async Task Handle(UC2SInitDataClient request, CancellationToken cancellationToken)
    {
        var dataClient = CurrentContext.ClientEP;

        cache.GetOrCreate(dataClient.ToString(), entry =>
        {
            var tcs = cache.Get<TaskCompletionSource<IPEndPoint>>(request.UserClientId);

            tcs.SetResult(dataClient);

            entry.SlidingExpiration = TimeSpan.FromSeconds(config.ExpireUdpTime);

            return request.UserClientId;
        });

        //await sender.Send(new UserClientToDataClientCmd(request.UserClientId));
    }
}