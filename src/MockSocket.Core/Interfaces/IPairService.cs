using MockSocket.Core.Services;

namespace MockSocket.Core.Interfaces
{
    public interface IPairService
    {
        ValueTask PairAsync(IMockTcpClient realClient, IMockTcpClient dataClient, CancellationToken cancellationToken);
    }
}