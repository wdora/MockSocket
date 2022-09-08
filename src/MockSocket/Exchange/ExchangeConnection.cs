using Microsoft.Extensions.Logging;
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
        readonly ILogger logger;

        public ExchangeConnection(ILogger<ExchangeConnection> logger)
        {
            this.logger = logger;
        }

        public async ValueTask ExchangeAsync(ITcpConnection srcConnection, ITcpConnection dstConnection, CancellationToken cancellationToken = default)
        {
            using var src = srcConnection;
            using var dst = dstConnection;

            logger.LogDebug($"连接{srcConnection}<=>{dstConnection}开始镜像...");

            await Task.WhenAny(SwapMessageAsync(srcConnection, dstConnection), SwapMessageAsync(dstConnection, srcConnection));
        }

        public virtual async Task SwapMessageAsync(ITcpConnection send, ITcpConnection receive, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(2048);

            try
            {
                Memory<byte> memory = buffer;

                while (true)
                {
                    var realSize = await receive.ReceiveAsync(memory, cancellationToken);

                    if (realSize == 0 && !receive.IsConnected)
                        return;

                    await send.SendAsync(memory.Slice(0, realSize), cancellationToken);
                }
            }
            catch (Exception e) when (e is SocketException se)
            {
                logger.LogDebug(se.Message, se.SocketErrorCode);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);

                logger.LogDebug($"conn {receive} closed");
            }
        }
    }
}
