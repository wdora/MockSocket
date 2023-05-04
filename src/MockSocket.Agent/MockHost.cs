﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Common.Interfaces;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

class MockHost
{
    IHost host;

    IMockAgent agent;

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public MockHost(string[] args)
    {
        SetEnv();

        host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var config = hostContext.Configuration;

                services
                    .AddTcpMockAgent()
                    .AddMemoryCache()
                    .AddLogging(builder => builder.ClearProviders().AddNLog(config))
                    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            })
            .Build();

        agent = host.Services.GetService<IMockAgent>()!;
    }

    [Conditional("DEBUG")]
    private void SetEnv()
    {
        // for host config：from "DOTNET_" prefixed environment variables
        // for app config：from environment variables
        Environment.SetEnvironmentVariable("DOTNET_" + HostDefaults.EnvironmentKey, Environments.Development);
    }

    public void Start() => agent.StartAsync(tokenSource.Token).AsTask().Wait();

    public void Stop() => tokenSource.Cancel();
}
