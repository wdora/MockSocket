using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Exceptions;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Commands;
using MockSocket.Udp.Exceptions;
using MockSocket.Udp.Utilities;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Services;

public class UdpMockAgent : IMockAgent
{
    MockAgentConfig config;

    ILogger logger;

    IUdpClient udpClient;

    ISender sender;

    ICancellationTokenService cancellationTokenService;

    public UdpMockAgent(ILogger<UdpMockAgent> logger, IUdpClient udpClient, ISender sender, ICancellationTokenService cancellationTokenService, IOptions<MockAgentConfig> options)
    {
        this.logger = logger;
        this.udpClient = udpClient;
        this.sender = sender;
        this.cancellationTokenService = cancellationTokenService;

        config = options.Value;

        logger.LogInformation("当前配置：" + config);
    }
    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        if (!config.Enable)
            return;

        var policy = Policy
            .Handle<Exception>(e =>
            {
                e = e.InnerException ?? e;

                if (e is ServiceUnavailableException)
                {
                    logger.LogInformation($"{config.MockServerEP} 服务不可用");
                    return true;
                }

                if (e is PortConflictException)
                {
                    logger.LogInformation($"{config.MockServerEP} 服务不可用");
                    return true;
                }

                logger.LogError(e, "Other");

                return true;
            }
            )
            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(retryAttempt > 5 ? 60 : Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(async token => await StartCoreAsync(token), cancellationToken);
    }

    public async ValueTask StartCoreAsync(CancellationToken cancellationToken)
    {
        using var token = cancellationTokenService.CreateToken(cancellationToken);

        var serverEP = config.MockServerEP;

        udpClient.Connect(serverEP);

        CurrentContext.MockAgent = udpClient;

        await RequestAppPortAsync(token);

        await await Task.WhenAny(LoopHeartBeatAsync(token), LoopServerCmdAsync(token));
    }

    private async Task LoopServerCmdAsync(TokenResult token)
    {
        while (true)
        {
            var cmd = await udpClient.ReceiveAsync<ICmd>(token);

            await sender.Send(cmd as object, token);
        }
    }

    private async Task LoopHeartBeatAsync(CancellationToken cancellationToken)
    {
        var heartInterval = TimeSpan.FromSeconds(config.HeartInterval);

        while (true)
        {
            await udpClient.SendAsync(new UC2SHeartBeat(config.HeartInterval), cancellationToken);

            await Task.Delay(heartInterval, cancellationToken);
        }
    }

    private async Task RequestAppPortAsync(CancellationToken cancellationToken)
    {
        var appPort = config.AppServerPort;

        await udpClient.SendAsync(new UC2SInitCtrlAgent(appPort, config.HeartInterval), cancellationToken);

        var cmd = await udpClient.ReceiveAsync<US2CResult>(cancellationToken);

        if (!cmd.IsOk)
            throw new PortConflictException(appPort);

        logger.LogInformation($"公网应用服务监听成功: udp://{config.MockServerAddress}:{appPort}");
    }

    public void Stop()
    {
        udpClient.Dispose();
    }
}
