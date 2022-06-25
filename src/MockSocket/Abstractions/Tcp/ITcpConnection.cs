namespace MockSocket.Core.Tcp
{
    public interface ITcpConnection : IDisposable
    {
        bool IsConnected { get; }
        ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

        ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
