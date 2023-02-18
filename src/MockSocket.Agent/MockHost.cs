using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockSocket.Agent;
using Serilog;

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

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                services.Configure<MockAgentConfig>(config.GetSection("mocksocket"));

                services
                    .AddHostedService<MockHostService>()
                    .AddSingleton<IMockAgent, MockAgent>()
                    .AddSingleton<IPairService, PairService>();

            })
            .UseSerilog()
            .Build();
    }

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}
