namespace MockSocket.Agent
{
    //public class UdpMockAgent : IMockAgent
    //{
    //    readonly MockAgentConfig config;
    //    readonly IUdpClient agent;
    //    readonly ILogger logger;
    //    readonly IBufferService bufferService;
    //    readonly IMemorySerializer memorySerializer;
    //    readonly ISender sender;

    //    public UdpMockAgent(IOptions<MockAgentConfig> config, IUdpClient agent, ILogger<UdpMockAgent> logger, IBufferService bufferService, IMemorySerializer memorySerializer, ISender sender)
    //    {
    //        this.config = config.Value;
    //        this.agent = agent;
    //        this.logger = logger;
    //        this.bufferService = bufferService;
    //        this.memorySerializer = memorySerializer;
    //        this.sender = sender;
    //    }

    //    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    //    {
    //        await CreateAppServerAsync(cancellationToken);

    //        _ = LoopHeartBeatAsync(cancellationToken);

    //        using var buffer = bufferService.Rent(IBufferService.UDP_BUFFER_SIZE);

    //        while (true)
    //        {
    //            var result = await agent.ReceiveFromAsync(buffer, cancellationToken);

    //            var cmd = memorySerializer.Deserialize<US2CCreateDataClient>(buffer.SliceTo(result.ReceivedBytes).Span);

    //            await sender.Send(cmd, cancellationToken);
    //        }
    //    }

    //    private async Task LoopHeartBeatAsync(CancellationToken cancellationToken)
    //    {
    //        var heartInterval = config.HeartInterval;

    //        using var buffer = bufferService.Rent();

    //        while (true)
    //        {
    //            var len = memorySerializer.Serialize(new UC2SCtrlAgentHeartBeat(), buffer);

    //            await agent.SendToAsync(buffer.SliceTo(len), config.UdpMockServer);

    //            logger.LogDebug("心跳成功");

    //            await Task.Delay(heartInterval, cancellationToken);
    //        }
    //    }

    //    private async ValueTask CreateAppServerAsync(CancellationToken cancellationToken)
    //    {
    //        logger.LogInformation($"正在连接服务器 {config.UdpMockServer} ...");

    //        var mockServerEP = IPEndPoint.Parse(config.UdpMockServer.ToString());

    //        using var buffer = bufferService.Rent();

    //        var len = memorySerializer.Serialize(new UC2SInitCtrlAgent(config.UdpAppServer.Port), buffer);

    //        await agent.SendToAsync(buffer.SliceTo(len), mockServerEP, cancellationToken);

    //        var response = await agent.ReceiveFromAsync(buffer, cancellationToken);

    //        var result = memorySerializer.Deserialize<US2CResult>(buffer.SliceTo(response.ReceivedBytes).Span);

    //        if (!result.IsOk)
    //            throw new AppServerException($"无法监听: {config.UdpAppServer}");

    //        var appServerEP = $"udp://{config.UdpMockServer.Address}:{config.UdpAppServer.Port}";

    //        var realServerEP = $"{config.UdpAppServer.Protocal}://{config.RealServer}";

    //        logger.LogInformation("创建服务成功，远程服务:{0}, 本地服务:{1}", appServerEP, realServerEP);
    //    }

    //    public ValueTask StopAsync()
    //    {
    //        // dispose anything
    //        throw new NotImplementedException();
    //    }
    //}

    //public class US2CCreateDataClientHandle : IRequestHandler<US2CCreateDataClient>
    //{
    //    readonly IUdpClient dataClient;
    //    readonly IUdpClient realClient;
    //    readonly IUdpPairService udpPairService;
    //    readonly IMemorySerializer memorySerializer;
    //    readonly IBufferService bufferService;
    //    readonly MockAgentConfig config;

    //    public US2CCreateDataClientHandle(IUdpClient dataClient, IUdpClient realClient, IUdpPairService udpPairService, IMemorySerializer memorySerializer, IBufferService bufferService, IOptions<MockAgentConfig> config)
    //    {
    //        this.dataClient = dataClient;
    //        this.realClient = realClient;
    //        this.udpPairService = udpPairService;
    //        this.memorySerializer = memorySerializer;
    //        this.bufferService = bufferService;
    //        this.config = config.Value;
    //    }

    //    public async Task Handle(US2CCreateDataClient request, CancellationToken cancellationToken)
    //    {
    //        using var buffer = bufferService.Rent();

    //        var len = memorySerializer.Serialize(new UC2SInitDataClient(request.UserClientId), buffer);

    //        await dataClient.SendToAsync(buffer.SliceTo(len), config.UdpMockServer, cancellationToken);

    //        _ = udpPairService.PairAsync((dataClient, config.UdpMockServer), (realClient, config.UdpRealServer), cancellationToken);
    //    }
    //}
}
