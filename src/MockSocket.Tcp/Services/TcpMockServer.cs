using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;
using MockSocket.Tcp.Utilities;

namespace MockSocket.Tcp.Services;
public class TcpMockServer : IMockServer
{
    MockServerConfig config;

    ITcpServer mockServer;

    ILogger logger;

    ISender sender;

    ICancellationTokenService cancellationTokenService;

    public TcpMockServer(ITcpServer mockServer, ILogger<TcpMockServer> logger, ISender sender, ICancellationTokenService cancellationTokenService, IOptions<MockServerConfig> options)
    {
        this.mockServer = mockServer;
        this.logger = logger;
        this.sender = sender;
        this.cancellationTokenService = cancellationTokenService;
        config = options.Value;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        mockServer.Listen(config.Port);

        while (true)
        {
            var agent = await mockServer.AcceptAsync(cancellationToken);

            logger.LogInformation("监听到新的连接请求，来自 AgentClient {agent}，正在建立连接 ...", agent);

            _ = LoopAgentAsync(agent, cancellationToken);
        }
    }

    private async ValueTask LoopAgentAsync(ITcpClient agent, CancellationToken cancellationToken)
    {
        CurrentContext.Agent.Value = agent;

        using var token = cancellationTokenService.CreateToken(cancellationToken, agent.Dispose);

        while (true)
        {
            var cmd = await agent.ReceiveAsync<ICmd>(token);

            await sender.Send(cmd as object, token);
        }
    }

    public ValueTask StopAsync(CancellationToken cancellationToken)
    {
        mockServer.Dispose();

        return default;
    }
}
