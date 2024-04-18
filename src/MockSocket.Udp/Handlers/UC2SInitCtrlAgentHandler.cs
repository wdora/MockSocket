using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Constants;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Exceptions;
using MockSocket.Udp.Models;
using MockSocket.Udp.Utilities;
using Polly;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Handlers;
public class UC2SInitCtrlAgentHandler : IRequestHandler<UC2SInitCtrlAgent>
{
    IUdpServer udpAppServer;

    ILogger logger;

    IBufferService bufferService;

    IMemoryCache memoryCache;

    ICancellationTokenService cancellationTokenService;

    MockServerConfig config;

    private IPEndPoint agentEP;

    public UC2SInitCtrlAgentHandler(IUdpServer mockUdpServer, ILogger<UC2SInitCtrlAgentHandler> logger, IBufferService bufferService, ICancellationTokenService cancellationTokenService, IMemoryCache memoryCache, IOptions<MockServerConfig> options)
    {
        this.udpAppServer = mockUdpServer;
        this.logger = logger;
        this.bufferService = bufferService;
        this.cancellationTokenService = cancellationTokenService;
        this.memoryCache = memoryCache;

        agentEP = CurrentContext.ClientEP;
        config = options.Value;
    }

    public async Task Handle(UC2SInitCtrlAgent request, CancellationToken cancellationToken)
    {
        try
        {
            udpAppServer.Listen(request.Port);
        }
        catch (PortConflictException)
        {
            await CurrentContext.MockServer.SendAsync(CurrentContext.ClientEP, new US2CResult(false), cancellationToken);
            return;
        }

        await CurrentContext.MockServer.SendAsync(CurrentContext.ClientEP, new US2CResult(true), cancellationToken);

        _ = LoopAsync(cancellationToken);
    }

    private async ValueTask LoopAsync(CancellationToken cancellationToken)
    {
        using var token = cancellationTokenService.CreateToken(cancellationToken, udpAppServer.Dispose);

        await await Task.WhenAny(LoopAgentClientHeartBeatAsync(token), LoopUserClientAsync(token));
    }

    private async Task LoopAgentClientHeartBeatAsync(CancellationToken cancellationToken)
    {
        var interval = TimeSpan.FromSeconds(config.HeartInterval);

        memoryCache.Set(agentEP, true, interval);

        while (memoryCache.TryGetValue(agentEP, out _))
        {
            await Task.Delay(interval, cancellationToken);
        }
    }

    private async Task LoopUserClientAsync(CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent(BufferSizes.Udp);

        while (true)
        {
            var response = await udpAppServer.ReceiveFromAsync(buffer, cancellationToken);

            try
            {
                UserClientContext context = await WaitPairDataAgentAsync(response.clientEP, cancellationToken);

                await CurrentContext.MockServer.SendToAsync(context.DataClientEP, buffer.SliceTo(response.length), cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "loop user");
            }

        }
    }

    private async ValueTask<UserClientContext> WaitPairDataAgentAsync(IPEndPoint clientEP, CancellationToken cancellationToken)
    {
        var key = clientEP.ToString();

        if (memoryCache.TryGetValue<UserClientContext>(key, out var context))
            return context;

        logger.LogInformation("{0}配对中...", key);

        await CurrentContext.MockServer.SendAsync(agentEP, new US2CCreateDataClient(key), cancellationToken);

        var tcs = new TaskCompletionSource<IPEndPoint>();

        memoryCache.Set(key, tcs);

        var dataClientEP = await tcs.Task;

        logger.LogInformation("{0} 与 {1}配对成功...", key, dataClientEP);

        context = new UserClientContext(udpAppServer, clientEP, dataClientEP);

        return memoryCache.Set(key, context, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(config.ExpireUdpTime) })!;
    }
}
