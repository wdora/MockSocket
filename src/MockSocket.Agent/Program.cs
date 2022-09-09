// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Forward;
using MockSocket.HoleClient;

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
    await sp.GetService<IHoleClient>()!.ConnectAsync();
else
    await sp.GetService<IForwardServer>()!.StartAsync();
