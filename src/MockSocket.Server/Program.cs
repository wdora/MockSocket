// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockSocket.Server;
using System.Reflection;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddMemoryCache()
            .AddSingleton<IMockServer, MockServer>()
            .AddTransient<IMockTcpServer, MockTcpServer>()
            .AddHostedService<MockHostService>()
            .AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
                );
    })
    .Build();

await host.StartAsync();