namespace MockSocket.Forward
{
    public interface IForwardServer
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }
}
