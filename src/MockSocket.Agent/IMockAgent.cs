namespace MockSocket.Agent
{
    public interface IMockAgent
    {
        ValueTask StartAsync(CancellationToken cancellationToken = default);
    }
}