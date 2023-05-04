using MediatR;
using Microsoft.Extensions.Logging;
using MockSocket.Common.Exceptions;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Commands;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;
using Polly;

namespace MockSocket.Tcp.Services;
public class TcpMockAgent : IMockAgent
{
    MockAgentConfig config = new MockAgentConfig();

    ITcpClient agent;

    ISender sender;

    ILogger logger;

    public TcpMockAgent(ITcpClient agent, ISender sender, ILogger<TcpMockAgent> logger)
    {
        this.agent = agent;
        this.sender = sender;
        this.logger = logger;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        var policy = Policy
           .Handle<Exception>(e =>
           {
               e = e.InnerException ?? e;

               if (e is ServiceUnavailableException)
               {
                   logger.LogInformation($"{config.MockServerEP} 服务不可用");
                   return true;
               }

               if (e is TaskCanceledException || e is OperationCanceledException)
               {
                   logger.LogInformation($"取消重试");
                   return true;
               }

               logger.LogError(e, "Other");

               return false;
           }
           )
           .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(retryAttempt > 5 ? 60 : Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(StartCoreAsync, cancellationToken);
    }

    private async Task StartCoreAsync(CancellationToken cancellationToken)
    {
        await agent.ConnectAsync(config.MockServerEP);

        logger.LogInformation($"Connect mockServer({config.MockServerEP}) success");

        var appPort = config.AppServerPort;

        await agent.SendAsync(new TC2SInitCtrlAgentCmd(appPort), cancellationToken);

        logger.LogInformation($"公网应用服务监听成功: tcp://{config.MockServerAddress}:{appPort}");

        await await Task.WhenAny(LoopSendAsync(cancellationToken), LoopReceiveAsync(cancellationToken));
    }

    private async Task LoopReceiveAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var cmd = await agent.ReceiveAsync<ICmd>(cancellationToken);

            await sender.Send(cmd as object, cancellationToken);
        }
    }

    private async Task LoopSendAsync(CancellationToken cancellationToken)
    {
        var interval = TimeSpan.FromSeconds(config.HeartInterval);

        while (true)
        {
            await agent.SendAsync(new TC2SHeartBeatCmd(), cancellationToken);

            await Task.Delay(interval, cancellationToken);
        }
    }
}
