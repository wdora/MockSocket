using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            .ConfigureServices((hostContext, services) =>
            {
                var config = hostContext.Configuration;

                services
                    .AddHostedService<MockHostService>()
                    .AddAgent(config.GetSection("MockSocket"))
                    .AddSingleton<IMockAgent, MockAgent>();

                services.AddLogging(builder => builder.AddNLog());

                NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            })
            .Build();
    }

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}
