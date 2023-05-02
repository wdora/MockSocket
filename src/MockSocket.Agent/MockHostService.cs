using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;

public class MockHostService : IHostedService
{
    // 没有Stop方法
    //foreach (IHostedService hostedService in _hostedServices)
    //{
    //    // Fire IHostedService.Start
    //    await hostedService.StartAsync(combinedCancellationToken).ConfigureAwait(false);

    //    if (hostedService is BackgroundService backgroundService)
    //    {
    //        _ = TryExecuteBackgroundServiceAsync(backgroundService);
    //    }
    //}

    readonly IMockAgent agent;
    readonly ILogger logger;

    public MockHostService(IMockAgent agent, ILogger<MockHostService> logger)
    {
        this.agent = agent;
        this.logger = logger;

        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        logger.LogError(e.Exception, "UnobservedTaskException");
        e.SetObserved();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = agent.StartAsync(cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        //await agent.StopAsync();
    }
}