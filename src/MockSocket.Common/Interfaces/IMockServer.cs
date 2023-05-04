namespace MockSocket.Common.Interfaces;

public interface IMockServer
{
    ValueTask StartAsync(CancellationToken cancellationToken);

    ValueTask StopAsync(CancellationToken cancellationToken);
}
