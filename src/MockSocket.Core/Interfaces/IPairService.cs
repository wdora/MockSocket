using MockSocket.Core.Services;

namespace MockSocket.Core.Interfaces
{
    public interface IPairService
    {
        ValueTask PairAsync(MockTcpClient realClient, MockTcpClient dataClient, CancellationToken cancellationToken);
    }
}