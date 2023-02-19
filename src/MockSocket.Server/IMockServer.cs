using MediatR;
using Microsoft.Extensions.Caching.Memory;
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

        public MockServer(IOptions<MockServerConfig> config, IMockTcpServer server, IMediator mediator)
        {
            this.config = config.Value;
            this.server = server;
            this.mediator = mediator;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await server.ListenAsync(config.ListenPort);

            while (true)
            {
                var agent = await server.AcceptAsync(cancellationToken);

                _ = HandleAgent(agent, cancellationToken);
            }
        }


        private async Task HandleAgent(MockTcpClient agent, CancellationToken cancellationToken)
        {
            using var client = CurrentContext.Agent = agent;

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _ = client.Register(cts.Cancel);

            while (true)
            {
                var command = await client.ReceiveAsync<ICmd>(cts.Token);

                await mediator.Send(command, cts.Token);
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

        public AppServerHandle(IMockTcpServer server, IMemoryCache cacheService)
        {
            this.server = server;
            this.cacheService = cacheService;
        }

        public async Task Handle(CreateAppServerCmd request, CancellationToken cancellationToken)
        {
            await server.ListenAsync(request.port);

            while (true)
            {
                var userClient = await server.AcceptAsync(cancellationToken);

                await CurrentContext.Agent.SendAsync(userClient.Id);

                cacheService.Set(userClient.Id, userClient);
            }
        }
    }

    public class MockTcpServer : IMockTcpServer
    {
        protected readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public async ValueTask<MockTcpClient> AcceptAsync(CancellationToken cancellationToken)
        {
            var client = await _socket.AcceptAsync(cancellationToken);

            return new MockTcpClient(client);
        }

        public ValueTask ListenAsync(int listenPort)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));

            _socket.Listen();

            return default;
        }
    }

    public interface IMockTcpServer
    {
        ValueTask<MockTcpClient> AcceptAsync(CancellationToken cancellationToken);

        ValueTask ListenAsync(int listenPort);
    }
}
