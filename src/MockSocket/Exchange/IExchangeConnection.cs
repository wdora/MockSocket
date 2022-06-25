using MockSocket.Core.Tcp;

namespace MockSocket.Core.Exchange
{
    public interface IExchangeConnection
    {
        ValueTask ExchangeAsync(ITcpConnection srcConnection, ITcpConnection dstConnection, CancellationToken cancellationToken = default);
    }
}
