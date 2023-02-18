namespace MockSocket.Agent
{
    public interface ITcpConnection
    {
        ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
    }
}