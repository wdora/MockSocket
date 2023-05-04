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

        logger.LogInformation($"监听端口{config.Port}成功");

        while (true)
        {
            var agent = await mockServer.AcceptAsync(cancellationToken);

            logger.LogInformation("有新的Agent {Agent}请求加入", agent);

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
