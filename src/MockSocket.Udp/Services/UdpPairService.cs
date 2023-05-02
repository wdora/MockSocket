using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Constants;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Services;
public class UdpPairService : IUdpPairService
{
    readonly IBufferService bufferService;
    readonly ICancellationTokenService cancellationTokenService;
    ILogger logger;

    public UdpPairService(IBufferService bufferService, ICancellationTokenService cancellationTokenService, ILogger<UdpPairService> logger)
    {
        this.bufferService = bufferService;
        this.cancellationTokenService = cancellationTokenService;
        this.logger = logger;
    }

    public async Task PairAsync(IUdpClient dataClient, IUdpClient realClient, CancellationToken cancellationToken)
    {
        logger.LogDebug("{0}<=>{1}开始交换...", dataClient, realClient);

        using var token = cancellationTokenService.CreateToken(cancellationToken, () => logger.LogDebug("{0}<=>{1}交换结束", dataClient, realClient));

        var fromTask = ForwardToAsync(dataClient, realClient, token);

        var toTask = ForwardToAsync(realClient, dataClient, token);

        await await Task.WhenAny(fromTask, toTask);
    }

    private async Task ForwardToAsync(IUdpClient dataClient, IUdpClient realClient, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent(BufferSizes.Udp);

        while (true)
        {
            var len = await dataClient.ReceiveFromAsync(buffer, cancellationToken);

            await realClient.SendToAsync(buffer.SliceTo(len), cancellationToken);
        }
    }
}
