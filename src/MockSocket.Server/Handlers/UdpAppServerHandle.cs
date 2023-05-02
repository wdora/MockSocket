using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Common.Interfaces;
using System.Collections.Concurrent;
using System.Net;

namespace MockSocket.Server.Handlers;

//public class UdpDataService : IUdpDataService
//{
//    IMemoryCache memoryCache;

//    MockServerConfig config;

//    IUdpPairService udpPairService;

//    public UdpDataService(IMemoryCache memoryCache, IOptions<MockServerConfig> config, IUdpPairService udpPairService)
//    {
//        this.memoryCache = memoryCache;
//        this.config = config.Value;
//        this.udpPairService = udpPairService;
//    }

//    public async ValueTask FromAppServer(IUdpServer appServer, IPEndPoint userEP, Memory<byte> data, CancellationToken cancellationToken = default)
//    {
//        var userClientId = userEP.Id();

//        var queue = await memoryCache.GetOrCreateAsync(userClientId, async entry =>
//        {
//            await CurrentContext.CtrlAgent4Udp.SendCmdAsync(new US2CCreateDataClient(userClientId));

//            return new ConcurrentQueue<byte[]>();
//        });

//        queue!.Enqueue(data.ToArray());
//    }

//    public bool IsDataClient(IPEndPoint dataEP)
//    {
//        return memoryCache.TryGetValue(dataEP.Id(), out _);
//    }

//    public ValueTask RegisterDataClient(IPEndPoint dataEP, IPEndPoint userEP)
//    {
//        // tag + pair
//        var key = dataEP.Id();

//        memoryCache.Set(key, new ConcurrentQueue<byte[]>(), new MemoryCacheEntryOptions { SlidingExpiration = config.Udp.SlidingTimeout });

//        _ = udpPairService.PairAsync(userEP, dataEP, default);

//        return default;
//    }

//    public ValueTask ToAppServer(IPEndPoint dataEP, Memory<byte> data, CancellationToken cancellationToken = default)
//    {
//        var dataClientId = dataEP.Id();

//        if (!memoryCache.TryGetValue<ConcurrentQueue<byte[]>>(dataClientId, out var queue))
//            return default;

//        if (!data.IsEmpty)
//            return default;

//        queue!.Enqueue(data.ToArray());

//        return default;
//    }
//}

///// <summary>
///// udpctrlagent
///// udpdataclient
///// </summary>
//public class UdpMockServer : IMockServer
//{
//    IUdpServer mockUdpServer;

//    IUdpDataService udpDataService;

//    IMediator mediator;

//    MockServerConfig config;

//    IMemorySerializer memorySerializer;

//    ILogger logger;

//    IBufferService bufferService;

//    public UdpMockServer(IUdpServer mockUdpServer, IOptions<MockServerConfig> config, IMediator mediator, IUdpDataService udpDataService, IMemorySerializer memorySerializer, ILogger<UdpMockServer> logger, IBufferService bufferService)
//    {
//        this.mockUdpServer = mockUdpServer;
//        this.config = config.Value;
//        this.mediator = mediator;
//        this.udpDataService = udpDataService;
//        this.memorySerializer = memorySerializer;
//        this.logger = logger;
//        this.bufferService = bufferService;
//    }

//    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
//    {
//        mockUdpServer.Listen(config.ListenUdpPort);

//        CurrentContext.UdpMockServer = mockUdpServer;

//        using var memory = bufferService.Rent();

//        while (true)
//        {
//            var result = await mockUdpServer.ReceiveFromAsync(memory, cancellationToken);

//            var clientEP = result.RemoteEndPoint;

//            if (!udpDataService.IsDataClient(clientEP))
//            {
//                CurrentContext.UdpEndPoint = clientEP;

//                var cmd = memorySerializer.Deserialize<ICmd>(memory.SliceTo(result.ReceivedBytes).Span);

//                // Unhandled exception. System.InvalidOperationException: No service for type 'MediatR.IRequestHandler`1[MockSocket.Core.Commands.ICmd]' has been registered.
//                await mediator.Send(cmd as object, cancellationToken);

//                continue;
//            }

//            // udp data pair
//            await udpDataService.ToAppServer(clientEP, memory, cancellationToken);
//        }
//    }
//}

//public class UC2SInitDataClientHandle : IRequestHandler<UC2SInitDataClient>
//{
//    IUdpDataService udpDataService;
//    ILogger logger;

//    public UC2SInitDataClientHandle(IUdpDataService udpDataService, ILogger<UC2SInitDataClientHandle> logger)
//    {
//        this.udpDataService = udpDataService;
//        this.logger = logger;
//    }

//    public async Task Handle(UC2SInitDataClient request, CancellationToken cancellationToken)
//    {
//        await udpDataService.RegisterDataClient(CurrentContext.UdpEndPoint, IPEndPoint.Parse(request.UserClientId));

//        logger.LogInformation($"userClient {CurrentContext.UdpEndPoint} is coming");
//    }
//}

//public class UC2SCtrlAgentHeartBeatHandle : IRequestHandler<UC2SCtrlAgentHeartBeat>
//{
//    public Task Handle(UC2SCtrlAgentHeartBeat request, CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }
//}

//public class UC2SInitCtrlAgentHandle : IRequestHandler<UC2SInitCtrlAgent>
//{
//    IUdpServer udpAppServer;

//    IUdpDataService udpDataService;

//    IMemorySerializer memorySerializer;

//    IBufferService bufferService;

//    ILogger logger;

//    public UC2SInitCtrlAgentHandle(IUdpServer mockUdpServer, IUdpDataService udpDataService, IMemorySerializer memorySerializer, IBufferService bufferService, ILogger<UC2SInitCtrlAgentHandle> logger)
//    {
//        this.udpAppServer = mockUdpServer;
//        this.udpDataService = udpDataService;
//        this.memorySerializer = memorySerializer;
//        this.bufferService = bufferService;
//        this.logger = logger;
//    }

//    public async Task Handle(UC2SInitCtrlAgent request, CancellationToken cancellationToken)
//    {
//        udpAppServer.Listen(request.Port);

//        using var buffer = bufferService.Rent();

//        Memory<byte> memory = buffer;

//        var len = memorySerializer.Serialize(new US2CResult(true), memory.Span);

//        await CurrentContext.UdpMockServer.SendToAsync(memory.Slice(0, len), CurrentContext.UdpEndPoint, cancellationToken);

//        _ = LoopServerAsync(cancellationToken);
//    }

//    private async ValueTask LoopServerAsync(CancellationToken cancellationToken)
//    {
//        var buffer = new byte[65536].AsMemory();

//        while (true)
//        {
//            var response = await udpAppServer.ReceiveFromAsync(buffer, cancellationToken);

//            var userEP = response.RemoteEndPoint;
//            var realLength = response.ReceivedBytes;
            
//            logger.LogInformation($"userClient {userEP} is coming");

//            if (realLength == 0)
//                continue;

//            await udpDataService.FromAppServer(udpAppServer, userEP, buffer.Slice(0, realLength));
//        }
//    }
//}
