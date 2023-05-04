// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Common.Interfaces;
using NLog.Extensions.Logging;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        services
            .AddTcpMockServer(config)
            .AddUdpMockServer(config)
            .AddMemoryCache()
            .AddLogging(builder => builder.ClearProviders().AddNLog(config));
    })
    .Build();

var servers = host.Services.GetServices<IMockServer>();

await Task.WhenAny(servers.Select(x => x.StartAsync(default).AsTask()));