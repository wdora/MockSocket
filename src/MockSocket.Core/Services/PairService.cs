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
            logger.LogDebug("开始交换连接");

            try
            {
                var task1 = ForwardAsync(client1, client2, cancellationToken);

                var task2 = ForwardAsync(client2, client1, cancellationToken);

                await Task.WhenAny(task1, task2);
            }
            finally
            {
                logger.LogDebug("交换连接结束");
            }
        }

        static Task ForwardAsync(MockTcpClient send, MockTcpClient receive, CancellationToken cancellationToken)
        {
            return BufferPool.Instance.Run(async memory =>
            {
                while (true)
                {
                    var realSize = await send.ReceiveAsync(memory, cancellationToken);

                    await receive.SendAsync(memory[..realSize], cancellationToken);
                }
            }).AsTask();
        }
    }
}