namespace MockSocket.Server
{
    public interface IMockServer
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }
}
