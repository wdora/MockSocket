using MockSocket.Core.Services;

namespace MockSocket.Core.Interfaces
{
    public interface IMockTcpServer : IDisposable
    {
        ValueTask<IMockTcpClient> AcceptAsync(CancellationToken cancellationToken);

        ValueTask ListenAsync(int listenPort);
    }
}
