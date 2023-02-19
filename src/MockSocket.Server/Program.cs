// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSocket.Server;
using NLog.Extensions.Logging;
using System.Reflection;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

        services
            .AddMemoryCache()
            .AddSingleton<IMockServer, MockServer>()
            .AddTransient<IMockTcpServer, MockTcpServer>()
            .AddHostedService<MockHostService>()
            .AddLogging(builder => builder.ClearProviders().AddNLog())
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    })
    .Build();

await host.StartAsync();