﻿using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Models;
using MockSocket.Udp.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UC2SInitDataClientHandler : IRequestHandler<UC2SInitDataClient>
{
    IMemoryCache cache;

    ISender sender;

    MockServerConfig config = new MockServerConfig();

    public UC2SInitDataClientHandler(IMemoryCache cache, ISender sender)
    {
        this.cache = cache;
        this.sender = sender;
    }

    public async Task Handle(UC2SInitDataClient request, CancellationToken cancellationToken)
    {
        var dataClient = CurrentContext.ClientEP;

        cache.GetOrCreate(dataClient.ToString(), entry =>
        {
            var context = cache.Get<UserClientContext>(request.UserClientId);

            context.DataClientEP = dataClient;

            entry.SlidingExpiration = TimeSpan.FromSeconds(config.ExpireUdpTime);

            return request.UserClientId;
        });

        await sender.Send(new UserClientToDataClientCmd(request.UserClientId));
    }
}