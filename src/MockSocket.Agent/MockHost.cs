using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Agent;
using MockSocket.Core.Extensions;
using NLog.Extensions.Logging;

class MockHost
{
    IHost host;

    public MockHost(string[] args)
    {
        host = Host
            .CreateDefaultBuilder(args)
#if DEBUG
            .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?> { { HostDefaults.EnvironmentKey, Environments.Development } }))
#endif
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

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}
