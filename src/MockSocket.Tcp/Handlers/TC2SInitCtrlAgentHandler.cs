using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Tcp.Commands;
using MockSocket.Tcp.Interfaces;
using MockSocket.Tcp.Utilities;

namespace MockSocket.Tcp.Handlers;
public class TC2SInitCtrlAgentHandler : IRequestHandler<TC2SInitCtrlAgentCmd>
{
    private ITcpServer appServer;
    private ILogger logger;
    private IMemoryCache cache;
    private ITcpClient agent;

    public TC2SInitCtrlAgentHandler(ITcpServer appServer, ILogger<TC2SInitCtrlAgentHandler> logger, IMemoryCache cache)
    {
        this.appServer = appServer;
        this.logger = logger;
        this.cache = cache;

        agent = CurrentContext.Agent.Value!;
    }

    public Task Handle(TC2SInitCtrlAgentCmd request, CancellationToken cancellationToken)
    {
        appServer.Listen(request.Port);

        _ = LoopAsync(cancellationToken);

        return Task.CompletedTask;
    }

    private async Task LoopAsync(CancellationToken cancellationToken)
    {
        using var server = appServer;

        while (true)
        {
            var client = await appServer.AcceptAsync(cancellationToken);

            var clientId = client.ReceiveId!;

            logger.LogInformation("监听到新的连接请求，来自 UserClient {agent}，正在建立连接 ...", clientId);

            await agent.SendAsync(new TS2CInitUserClientCmd(clientId), cancellationToken);

            cache.Set(clientId, client);
        }
    }
}
