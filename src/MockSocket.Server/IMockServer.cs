using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Core.Commands;
using MockSocket.Core.Configurations;
using MockSocket.Core.Services;
using MockSocket.Core.Tcp;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Server
{
    public interface IMockServer
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }

    public class MockServer : IMockServer
    {
        MockServerConfig config;

        IMockTcpServer server;

        IMediator mediator;

        ILogger logger;

        public MockServer(IOptions<MockServerConfig> config, IMockTcpServer server, IMediator mediator, ILogger<MockServer> logger)
        {
            this.config = config.Value;
            this.server = server;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await server.ListenAsync(config.ListenPort);

            while (true)
            {
                var agent = await server.AcceptAsync(cancellationToken);

                _ = HandleAgentAsync(agent, cancellationToken);
            }
        }


        private async Task HandleAgentAsync(MockTcpClient agent, CancellationToken cancellationToken)
        {
            using var client = CurrentContext.Agent = agent;

            var token = client.Register(cancellationToken);

            while (true)
            {
                var command = await client.ReceiveAsync<ICmd>(token);

                logger.LogDebug($"{client.Id}: {command}");

                await mediator.Send(command as object, token);
            }
        }
    }

    public class CurrentContext
    {
        static AsyncLocal<MockTcpClient> LocalAgent = new AsyncLocal<MockTcpClient>();

        public static MockTcpClient Agent
        {
            get => LocalAgent.Value!;

            set => LocalAgent.Value = value;
        }
    }

    public class HeartBeatHandle : IRequestHandler<HeartBeatCmd>
    {
        public Task Handle(HeartBeatCmd request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class DataClientHandle : IRequestHandler<DataClientCmd>
    {
        IMemoryCache cacheService;

        IPairService pairService;

        public DataClientHandle(IMemoryCache cacheService, IPairService pairService)
        {
            this.cacheService = cacheService;
            this.pairService = pairService;
        }

        public async Task Handle(DataClientCmd request, CancellationToken cancellationToken)
        {
            if (!cacheService.TryGetValue<MockTcpClient>(request.UserClientId, out var userClient))
                return;

            cacheService.Remove(request.UserClientId);

            await pairService.PairAsync(userClient!, CurrentContext.Agent, cancellationToken);
        }
    }

    public class AppServerHandle : IRequestHandler<CreateAppServerCmd>
    {
        IMockTcpServer server;

        IMemoryCache cacheService;

        ILogger logger;

        public AppServerHandle(IMockTcpServer server, IMemoryCache cacheService, ILogger<AppServerHandle> logger)
        {
            this.server = server;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task Handle(CreateAppServerCmd request, CancellationToken cancellationToken)
        {
            await server.ListenAsync(request.Port);

            await CurrentContext.Agent.SendAsync(true);

            _ = LoopAsync(server, cancellationToken);
        }

        private async Task LoopAsync(IMockTcpServer server, CancellationToken cancellationToken)
        {
            using var appServer = server;

            while (true)
            {
                var userClient = await appServer.AcceptAsync(cancellationToken);

                logger.LogInformation($"userClient {userClient.Id} is coming");

                await CurrentContext.Agent.SendAsync(userClient.Id);

                cacheService.Set(userClient.Id, userClient);
            }
        }
    }

    public class MockTcpServer : IMockTcpServer
    {
        protected readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        ILogger logger;
        IPEndPoint localEP = null!;

        public MockTcpServer(ILogger<MockTcpServer> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<MockTcpClient> AcceptAsync(CancellationToken cancellationToken)
        {
            var client = await _socket.AcceptAsync(cancellationToken);

            return new MockTcpClient(client);
        }

        public void Dispose()
        {
            // this's endpoint couldn't visit after dispose 
            logger.LogInformation("服务停止监听:" + this);

            _socket.Dispose();
        }

        public ValueTask ListenAsync(int listenPort)
        {
            localEP = new IPEndPoint(IPAddress.Any, listenPort);

            _socket.Bind(localEP);

            _socket.Listen();

            logger.LogInformation("服务开始监听:" + this);

            return default;
        }


        public override string ToString()
        {
            return localEP?.ToString() ?? "";
        }
    }

    public interface IMockTcpServer : IDisposable
    {
        ValueTask<MockTcpClient> AcceptAsync(CancellationToken cancellationToken);

        ValueTask ListenAsync(int listenPort);
    }
}
