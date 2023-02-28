// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Core.Extensions;
using MockSocket.Server;
using NLog.Extensions.Logging;
using System.Reflection;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        services
            .AddMemoryCache()
            .AddHostedService<MockHostService>()
            .AddSingleton<IMockServer, MockServer>()
            .AddMockServer(config)
            .AddLogging(builder => builder.ClearProviders().AddNLog(config))
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    })
    .Build();

await host.StartAsync();