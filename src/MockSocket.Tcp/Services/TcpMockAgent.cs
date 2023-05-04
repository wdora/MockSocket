using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Common.Exceptions;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Commands;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;
using Polly;

namespace MockSocket.Tcp.Services;
public class TcpMockAgent : IMockAgent
{
    MockAgentConfig config;

    ITcpClient agent;
    readonly TcpClientFactory agentFactory;
    ISender sender;

    ILogger logger;

    public TcpMockAgent(TcpClientFactory agentFactory, ISender sender, ILogger<TcpMockAgent> logger, IOptions<MockAgentConfig> options)
    {
        this.agentFactory = agentFactory;
        this.agent = agentFactory.Create();
        this.sender = sender;
        this.logger = logger;

        config = options.Value;

        logger.LogInformation("当前配置：" + config);
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

               agent.Dispose();

               agent = agentFactory.Create();

               logger.LogError(e, "Other");

               return true;
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

        agent.EnableKeepAlive();

        await LoopReceiveAsync(cancellationToken);
    }

    private async Task LoopReceiveAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var cmd = await agent.ReceiveAsync<ICmd>(cancellationToken);

            await sender.Send(cmd as object, cancellationToken);
        }
    }

    public void Stop()
    {
        agent.Dispose();
    }
}
