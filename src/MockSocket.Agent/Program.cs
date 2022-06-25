// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockSocket.HoleClient;

var config = new ConfigurationBuilder()
        .AddCommandLine(args)
        .Build();

var sp = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddHoleClient(config)
                .BuildServiceProvider();

await sp.GetService<IHoleClient>()!.ConnectAsync();