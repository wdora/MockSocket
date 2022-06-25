namespace MockSocket.HoleServer
{
    public interface IHoleServer : IDisposable
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }
}
