using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Constants;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Config;
using MockSocket.Udp.Utilities;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Services;
public class UdpMockServer : IMockServer
{
    MockServerConfig config = new MockServerConfig();

    IUdpServer udpServer;

    ILogger logger;

    IBufferService bufferService;

    ISender sender;

    IMemorySerializer memorySerializer;

    IMemoryCache cache;

    public UdpMockServer(IUdpServer udpServer, ILogger<UdpMockServer> logger, IBufferService bufferService, ISender sender, IMemorySerializer memorySerializer, IMemoryCache cache)
    {
        this.udpServer = udpServer;
        this.logger = logger;
        this.bufferService = bufferService;
        this.sender = sender;
        this.memorySerializer = memorySerializer;
        this.cache = cache;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        udpServer.Listen(config.Port);

        CurrentContext.MockServer = udpServer;

        logger.LogInformation($"监听端口{config.Port}成功");

        using var buffer = bufferService.Rent(BufferSizes.Udp);

        while (true)
        {
            var result = await udpServer.ReceiveFromAsync(buffer, cancellationToken);

            if (cache.TryGetValue<string>(result.clientEP.ToString(), out var userClientId))
            {
                await sender.Send(new DataClientToUserClientCmd(userClientId, buffer, result.length));

                continue;
            }

            CurrentContext.ClientEP = result.clientEP;

            var cmd = memorySerializer.Deserialize<ICmd>(buffer.SliceTo(result.length).Span);

            await sender.Send(cmd as object, cancellationToken);
        }
    }

    public ValueTask StopAsync(CancellationToken cancellationToken)
    {
        udpServer.Dispose();

        return default;
    }
}
