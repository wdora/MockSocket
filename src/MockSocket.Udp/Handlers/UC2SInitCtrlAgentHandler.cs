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
using MockSocket.Udp.Utilities;
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

    ISender sender;

    MockServerConfig config;

    private IPEndPoint agentEP;

    public UC2SInitCtrlAgentHandler(IUdpServer mockUdpServer, ILogger<UC2SInitCtrlAgentHandler> logger, IBufferService bufferService, ICancellationTokenService cancellationTokenService, IMemoryCache memoryCache, ISender sender, IOptions<MockServerConfig> options)
    {
        this.udpAppServer = mockUdpServer;
        this.logger = logger;
        this.bufferService = bufferService;
        this.cancellationTokenService = cancellationTokenService;
        this.memoryCache = memoryCache;
        this.sender = sender;

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
        while (true)
        {
            var buffer = bufferService.Rent(BufferSizes.Udp);

            var response = await udpAppServer.ReceiveFromAsync(buffer, cancellationToken);

            await sender.Send(new UserDataCommand(buffer, response.length, response.clientEP, agentEP, udpAppServer));
        }
    }
}
