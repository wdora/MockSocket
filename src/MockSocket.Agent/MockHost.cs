using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Agent;
using MockSocket.Core.Extensions;
using NLog.Extensions.Logging;
using System.Diagnostics;

class MockHost
{
    IHost host;

    public MockHost(string[] args)
    {
        SetEnv();

        host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var config = hostContext.Configuration;

                services
                    .AddHostedService<MockHostService>()
                    .AddAgent(config)
                    .AddSingleton<IMockAgent, MockAgent>()
                    .AddLogging(builder => builder.ClearProviders().AddNLog(config));
            })
            .Build();
    }

    [Conditional("DEBUG")]
    private void SetEnv()
    {
        // for host config：from "DOTNET_" prefixed environment variables
        // for app config：from environment variables
        Environment.SetEnvironmentVariable("DOTNET_" + HostDefaults.EnvironmentKey, Environments.Development);
    }

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}
