// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;
using NLog.Extensions.Logging;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        services
            .AddUdpMockServer()
            .AddMemoryCache()
            .AddLogging(builder => builder.ClearProviders().AddNLog(config));
    })
    .Build();

await host.Services.GetService<IMockServer>()!.StartAsync(default);