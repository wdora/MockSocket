// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockSocket.HoleServer;

var switchMappings = new Dictionary<string, string>
{
    { "-p", "ListenPort" }
};

var config = new ConfigurationBuilder()
        .AddCommandLine(args, switchMappings)
        .Build();

var sp = new ServiceCollection()
    .AddHoleServer(config)
    .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
    .BuildServiceProvider();

var tasks = sp.GetServices<IHoleServer>().Select(x => x.StartAsync().AsTask());

await Task.WhenAll(tasks);

Console.ReadLine();