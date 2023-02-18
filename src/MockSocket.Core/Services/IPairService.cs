using MockSocket.Core.Tcp;

namespace MockSocket.Core.Services
{
    public interface IPairService
    {
        ValueTask PairAsync(MockTcpClient realClient, MockTcpClient dataClient, CancellationToken cancellationToken);
    }
}