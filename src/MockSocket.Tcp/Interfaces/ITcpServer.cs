namespace MockSocket.Tcp.Interfaces;
public interface ITcpServer : IDisposable
{
    ValueTask<ITcpClient> AcceptAsync(CancellationToken cancellationToken);

    void Listen(int port);

    ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken);
}
