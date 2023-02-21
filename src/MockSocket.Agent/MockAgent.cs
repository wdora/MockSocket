using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Core.Commands;
using MockSocket.Core.Configurations;
using MockSocket.Core.Exceptions;
using MockSocket.Core.Extensions;
using MockSocket.Core.Services;
using MockSocket.Core.Tcp;

namespace MockSocket.Agent
{
    public class MockAgent : IMockAgent
    {
        private readonly MockAgentConfig config;
        private readonly ILogger<MockAgent> logger;
        private readonly IPairService pairService;

        private MockTcpClient agent = null!;
        private CancellationTokenSource cts;

        public MockAgent(IOptions<MockAgentConfig> config, ILogger<MockAgent> logger, IPairService pairService)
        {
            this.config = config.Value;
            this.logger = logger;
            this.pairService = pairService;
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return StartCoreAsync(cts.Token).RetryAsync(cancellationToken: cts.Token);
        }

        public async ValueTask StartCoreAsync(CancellationToken cancellationToken)
        {
            var (host, port) = config.RemoteServer;

            logger.LogDebug($"正在连接服务器 {config.RemoteServer} ...");

            agent = await TcpSocketFactory.Create(host, port);

            logger.LogDebug("连接服务器成功");

            using var client = agent;

            await CreateAppServerAsync();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var heartTask = HeartBeatAsync(cts);

            var handleTask = HandleNewClientAsync(cts.Token);

            await Task.WhenAny(heartTask, handleTask);
        }

        private async Task HandleNewClientAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var userClientId = await agent.ReceiveAsync<string>();

                logger.LogInformation($"userClient {userClientId} is coming");

                try
                {
                    var realClient = await CreateRealClientAsync();

                    var dataClient = await CreateDataClientAsync(userClientId);

                    _ = pairService.PairAsync(realClient, dataClient, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError($"userClient {userClientId} handle error", e);
                }
            }
        }

        private async ValueTask<MockTcpClient> CreateDataClientAsync(string userClientId)
        {
            var (host, port) = config.RemoteServer;

            var connection = await TcpSocketFactory.Create(host, port);

            await connection.SendAsync(new DataClientCmd(userClientId));

            return connection;
        }

        private ValueTask<MockTcpClient> CreateRealClientAsync()
        {
            var (host, port) = config.RealServer;

            return TcpSocketFactory.Create(host, port);
        }

        private async ValueTask CreateAppServerAsync()
        {
            var appServer = config.AppServer;

            await agent.SendAsync(new CreateAppServerCmd(appServer.Port, appServer.Protocal));

            var isOk = await agent.ReceiveAsync<bool>()
                        .TimeoutAsync(() => false);

            if (!isOk)
                throw new AppServerException($"无法监听: {config.AppServer}");

            var appServerEP = $"{config.AppServer.Protocal}://{config.RemoteServer.Host}:{config.AppServer.Port}";

            var realServerEP = $"{config.RealServer}";

            logger.LogDebug("创建服务成功，远程服务:{0},本地服务:{1}", appServerEP, realServerEP);
        }

        private async Task HeartBeatAsync(CancellationTokenSource cancellationTokenSource)
        {
            var cancellationToken = cancellationTokenSource.Token;

            var heartInterval = TimeSpan.FromSeconds(config.HeartInterval);

            try
            {
                while (true)
                {
                    await agent.SendAsync(new HeartBeatCmd(new { DateTime.Now, heartInterval }.ToString()!), cancellationToken);

                    logger.LogDebug("心跳成功");

                    await Task.Delay(heartInterval, cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "心跳失败");

                cancellationTokenSource.Cancel();

                return;
            }
        }

        public ValueTask StopAsync()
        {
            logger.LogInformation("停止服务");

            cts.Cancel();

            agent?.Dispose();

            logger.LogInformation("停止服务成功");
            return default;
        }
    }
}