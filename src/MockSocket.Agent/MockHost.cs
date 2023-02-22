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
        // for host config
        SetEnv();

        host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var config = hostContext.Configuration;

                services
                    .AddHostedService<MockHostService>()
                    .AddAgent(config.GetSection("MockSocket"))
                    .AddSingleton<IMockAgent, MockAgent>()
                    .AddLogging(builder =>
                    {
                        builder.ClearProviders();

                        builder.AddNLog();
                        NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
                    });
            })
            .Build();
    }

    [Conditional("DEBUG")]
    private void SetEnv()
    {
        Environment.SetEnvironmentVariable("DOTNET_" + HostDefaults.EnvironmentKey, Environments.Development);
    }

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}
