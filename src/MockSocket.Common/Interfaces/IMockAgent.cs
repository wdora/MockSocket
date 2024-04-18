namespace MockSocket.Common.Interfaces;

public interface IMockAgent
{
    ValueTask StartAsync(CancellationToken cancellationToken);

    void Stop();
}
