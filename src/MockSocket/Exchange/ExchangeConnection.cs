using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            this.logger = logger ?? NullLogger<ExchangeConnection>.Instance;
        }

        public async ValueTask ExchangeAsync(ITcpConnection srcConnection, ITcpConnection dstConnection, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            cts.Token.Register(() => srcConnection.Dispose());
            cts.Token.Register(() => dstConnection.Dispose());

            logger.LogDebug($"连接{srcConnection}<=>{dstConnection}开始镜像...");

            try
            {
                await Task.WhenAny(SwapMessageAsync(srcConnection, dstConnection, cts.Token), SwapMessageAsync(dstConnection, srcConnection, cts.Token));

                logger.LogDebug($"连接{srcConnection}<=>{dstConnection}已中断");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exchange异常");
            }
            finally
            {
                cts.Cancel();
            }
        }

        public virtual async Task SwapMessageAsync(ITcpConnection send, ITcpConnection receive, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4096);

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
            }
        }
    }
}
