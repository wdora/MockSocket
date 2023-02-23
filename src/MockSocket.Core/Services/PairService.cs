using Microsoft.Extensions.Logging;
using MockSocket.Core.Interfaces;

namespace MockSocket.Core.Services
{
    public class PairService : IPairService
    {
        readonly ILogger logger;

        public PairService(ILogger<PairService> logger)
        {
            this.logger = logger;
        }

        public async ValueTask PairAsync(IMockTcpClient client1, IMockTcpClient client2, CancellationToken cancellationToken)
        {
            logger.LogInformation($"交换连接开始：{client1.Id} <=> {client2.Id}");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using var srcClient = client1;
            using var dstClient = client2;

            try
            {
                using var task1 = SwapAsync(client1, client2, cts.Token);

                using var task2 = SwapAsync(client2, client1, cts.Token);

                var anyTask = await Task.WhenAny(task1, task2);

                cts.Cancel();

                await Task.WhenAll(task1, task2);
            }
            catch (Exception)
            {
            }
            finally
            {
                logger.LogInformation($"交换连接结束：{client1.Id} <=> {client2.Id}");
            }
        }

        static Task SwapAsync(IMockTcpClient send, IMockTcpClient receive, CancellationToken cancellationToken)
        {
            return BufferPool.Instance.Run(async memory =>
            {
                while (true)
                {
                    var realSize = await send.ReceiveAsync(memory, cancellationToken);

                    if (realSize == 0)
                        return;

                    await receive.SendAsync(data: memory[..realSize], cancellationToken);
                }
            }).AsTask();
        }
    }
}