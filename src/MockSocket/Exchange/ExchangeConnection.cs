using Microsoft.Extensions.Logging;
using MockSocket.Core.Tcp;
using System.Buffers;

namespace MockSocket.Core.Exchange
{
    /// <summary>
    /// Buffer
    /// </summary>
    public class ExchangeConnection : IExchangeConnection
    {
        readonly ILogger logger;

        public ExchangeConnection(ILogger<ExchangeConnection> logger)
        {
            this.logger = logger;
        }

        public async ValueTask ExchangeAsync(ITcpConnection srcConnection, ITcpConnection dstConnection, CancellationToken cancellationToken = default)
        {
            logger.LogDebug($"连接{srcConnection}<=>{dstConnection}开始镜像...");

            await Task.WhenAny(SwapMessageAsync(srcConnection, dstConnection, cancellationToken), SwapMessageAsync(dstConnection, srcConnection, cancellationToken));
        }

        public virtual async Task SwapMessageAsync(ITcpConnection send, ITcpConnection receive, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(2048);

            try
            {
                Memory<byte> memory = buffer;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var realSize = await receive.ReceiveAsync(memory, cancellationToken);

                    if (realSize == 0)
                        return;

                    await send.SendAsync(memory.Slice(0, realSize), cancellationToken);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                
                logger.LogDebug($"连接{receive}已中断");

                receive.Dispose();
            }
        }
    }
}
