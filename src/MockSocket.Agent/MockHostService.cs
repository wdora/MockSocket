﻿using Microsoft.Extensions.Hosting;
using MockSocket.Agent;

public class MockHostService : IHostedService
{
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

    public MockHostService(IMockAgent agent)
    {
        this.agent = agent;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await agent.StartAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Console.WriteLine("dsads");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}