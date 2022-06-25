namespace MockSocket.HoleClient
{
    public class UdpHoleClient : IHoleClient
    {
        public ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
