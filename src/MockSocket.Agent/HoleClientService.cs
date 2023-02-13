using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Forward;
using MockSocket.HoleClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Agent
{
    public class HoleClientService
    {
        CancellationTokenSource ctx = new CancellationTokenSource();

        public async Task Start(string[] args)
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

            var sp = new ServiceCollection()
                            .AddLogging(builder => builder.AddConsole())
                            .AddHoleClient(config)
                            .BuildServiceProvider();

            var isAgent = sp.GetService<IOptions<ClientOptions>>()!.Value.ClientType == ClientType.Agent;

            if (isAgent)
                await sp.GetService<IHoleClient>()!.ConnectAsync(ctx.Token);
            else
                await sp.GetService<IForwardServer>()!.StartAsync();

        }

        public void Stop()
        {
            ctx.Cancel();
        }
    }
}
