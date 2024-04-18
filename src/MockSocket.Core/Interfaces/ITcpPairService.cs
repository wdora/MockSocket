using MockSocket.Core.Services;

namespace MockSocket.Core.Interfaces
{
    public interface ITcpPairService
    {
        ValueTask PairAsync(IMockTcpClient realClient, IMockTcpClient dataClient, CancellationToken cancellationToken);
    }
}