namespace MockSocket.HoleClient
{
    public interface IHoleClient : IDisposable
    {
        ValueTask ConnectAsync(CancellationToken cancellationToken = default);
    }
}
