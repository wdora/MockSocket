// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Topshelf;


HostFactory.Run(x =>
{
    x.Service<IHost>(s =>
    {
        s.ConstructUsing(a => CreateHost());
        s.WhenStarted(tc => tc.Start());
        s.WhenStopped(tc => tc.StopAsync().Wait());
    });

    x.RunAsLocalSystem();

    x.SetDescription("MockSocket Agent is Powered by .NET 7.0");
    x.SetDisplayName("MockSocket Agent");
    x.SetServiceName("MockSocket Agent");
});

IHost CreateHost()
{
    return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args).Build();
}

//var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
//Environment.ExitCode = exitCode;