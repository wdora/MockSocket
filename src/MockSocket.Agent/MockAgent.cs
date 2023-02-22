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
        private CancellationTokenSource cts = null!;

        public MockAgent(IOptions<MockAgentConfig> config, ILogger<MockAgent> logger, IPairService pairService)
        {
            this.config = config.Value;
            this.logger = logger;
            this.pairService = pairService;
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            var core = StartCoreAsync;

            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return core.RetryAsync(cancellationToken: cts.Token);
        }

        public async ValueTask StartCoreAsync(CancellationToken cancellationToken)
        {
            var (host, port) = config.RemoteServer;

            logger.LogInformation($"正在连接服务器 {config.RemoteServer} ...");

            agent = await TcpSocketFactory.Create(host, port);

            logger.LogInformation("连接服务器成功");

            using var client = agent;

            await CreateAppServerAsync();

            var token = RegisterHeartBeat(cancellationToken);

            var handleTask = HandleNewClientAsync(token);

            await handleTask;
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

            var realServerEP = $"{config.AppServer.Protocal}://{config.RealServer}";

            logger.LogInformation("创建服务成功，远程服务:{0}, 本地服务:{1}", appServerEP, realServerEP);
        }

        private CancellationToken RegisterHeartBeat(CancellationToken token)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var cancellationToken = cts.Token;

            var heartInterval = TimeSpan.FromSeconds(config.HeartInterval);

            _ = HeartBeatAsync(cts, heartInterval, cancellationToken);

            return cancellationToken;


        }

        private async Task HeartBeatAsync(CancellationTokenSource cts, TimeSpan heartInterval, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await agent.SendAsync(new HeartBeatCmd(new { DateTime.Now, heartInterval }.ToString()!));

                    logger.LogDebug("心跳成功");

                    await Task.Delay(heartInterval, cancellationToken);
                }
            }
            catch (Exception)
            {
                logger.LogInformation("心跳失败,与服务器断开连接");

                cts.Cancel();

                cts.Dispose();
            }
        }

        public ValueTask StopAsync()
        {
            cts.Cancel();

            agent?.Dispose();

            logger.LogInformation("停止服务成功");

            return default;
        }
    }
}