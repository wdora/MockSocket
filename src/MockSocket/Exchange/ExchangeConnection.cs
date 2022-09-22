using Microsoft.Extensions.Logging;
using MockSocket.Cache;
using MockSocket.Core.Tcp;
using System.Buffers;
using System.Net.Sockets;

namespace MockSocket.Core.Exchange
{
    /// <summary>
    /// Buffer
    /// </summary>
    public class ExchangeConnection : IExchangeConnection
    {
        private readonly ILogger logger;
        const int bufferSize = 4096;

        public ExchangeConnection(ILogger<ExchangeConnection> logger)
        {
            this.logger = logger;
        }

        public async ValueTask ExchangeAsync(ITcpConnection srcConnection, ITcpConnection dstConnection, CancellationToken cancellationToken = default)
        {
            using var src = srcConnection;
            using var dst = dstConnection;

            logger.LogDebug("Conn {srcConnection}<=>{dstConnection} start exchange...", src, dst);

            await Task.WhenAny(SwapMessageAsync(srcConnection, dstConnection), SwapMessageAsync(dstConnection, srcConnection));

            logger.LogDebug("Conn {srcConnection}<=>{dstConnection} end exchange.", src, dst);
        }

        public virtual async Task SwapMessageAsync(ITcpConnection send, ITcpConnection receive, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            var totalSize = 0;

            try
            {
                Memory<byte> memory = buffer;

                while (true)
                {
                    var realSize = await receive.ReceiveAsync(memory, cancellationToken);

                    if (realSize == 0)
                        return;

                    totalSize += realSize;

                    var realBuffer = memory.Slice(0, realSize);

                    await send.SendAsync(realBuffer, cancellationToken);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                logger.LogDebug("{receive} received length:{totalSize}", receive, totalSize);

                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
