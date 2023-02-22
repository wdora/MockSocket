using Microsoft.Extensions.Logging;
using MockSocket.Core.Tcp;

namespace MockSocket.Core.Services
{
    public class PairService : IPairService
    {
        readonly ILogger logger;

        public PairService(ILogger<PairService> logger)
        {
            this.logger = logger;
        }

        public async ValueTask PairAsync(MockTcpClient client1, MockTcpClient client2, CancellationToken cancellationToken)
        {
            logger.LogDebug($"交换连接开始：{client1.Id} <=> {client2.Id}");

            using var srcClient = client1;
            using var dstClient = client2;

            try
            {
                var task1 = ForwardAsync(client1, client2, cancellationToken);

                var task2 = ForwardAsync(client2, client1, cancellationToken);

                await await Task.WhenAny(task1, task2);
            }
            catch (Exception)
            {
            }
            finally
            {
                logger.LogDebug($"交换连接结束：{client1.Id} <=> {client2.Id}");
            }
        }

        static Task ForwardAsync(MockTcpClient send, MockTcpClient receive, CancellationToken cancellationToken)
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