using Microsoft.Extensions.Logging;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Interfaces;

namespace MockSocket.Tcp.Services;
public class TcpPairService : ITcpPairService
{
    IBufferService bufferService;
    ICancellationTokenService cancellationTokenService;
    ILogger logger;

    public TcpPairService(IBufferService bufferService, ICancellationTokenService cancellationTokenService, ILogger<TcpPairService> logger)
    {
        this.bufferService = bufferService;
        this.cancellationTokenService = cancellationTokenService;
        this.logger = logger;
    }

    public async ValueTask PairAsync(ITcpClient agentClient, ITcpClient userClient, CancellationToken cancellationToken)
    {
        using var client1 = agentClient;
        using var client2 = userClient;

        logger.LogDebug("{0}<=>{1}开始交换...", agentClient, userClient);

        using var token = cancellationTokenService.CreateToken(cancellationToken, () => logger.LogDebug("{0}<=>{1}交换结束", agentClient, userClient));

        var fromTask = ForwardAsync(agentClient, userClient, cancellationToken);

        var toTask = ForwardAsync(userClient, agentClient, cancellationToken);

        await await Task.WhenAny(fromTask, toTask);
    }

    private async Task ForwardAsync(ITcpClient fromClient, ITcpClient toClient, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        while (true)
        {
            var len = await fromClient.ReceiveBytesAsync(buffer, cancellationToken);

            await toClient.SendBytesAsync(buffer.SliceTo(len), cancellationToken);
        }
    }
}
