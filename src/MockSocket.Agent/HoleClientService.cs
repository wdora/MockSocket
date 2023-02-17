using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.HoleClient;

namespace MockSocket.Agent
{
    public class HoleClientService
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public void Start(string[] args) => StartAsync(args).Wait();

        public async Task StartAsync(string[] args)
        {
            var switchMappings = new Dictionary<string, string>
            {
                { "-p", "HoleAppServerPort" },
                { "-rs", "RealServer" },
                { "-rsp", "RealServerPort" },
                { "-hs", "HoleServer" },
                { "-hsp", "HoleServerPort" },
                { "-t", "ClientType" },
            };

            var config = new ConfigurationBuilder()
                    .AddCommandLine(args, switchMappings)
                    .Build();

            Host.CreateDefaultBuilder(args)
                .Build()
                .Start();

            var sp = new ServiceCollection()
                            .AddLogging(builder => builder.AddConsole())
                            .AddHoleClient(config)
                            .BuildServiceProvider();

            await sp.GetService<IMockAgent>()!.StartAsync(cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
