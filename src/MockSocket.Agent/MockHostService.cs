using Microsoft.Extensions.Hosting;
using MockSocket.Agent;

public class MockHostService : IHostedService
{
    readonly IMockAgent agent;

    public MockHostService(IMockAgent agent)
    {
        this.agent = agent;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await agent.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}