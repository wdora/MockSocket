// See https://aka.ms/new-console-template for more information

using System.ServiceProcess;
using Topshelf;

var rc = HostFactory.Run(x =>
{
    x.Service<MockHost>(s =>
    {
        s.ConstructUsing(_ => new MockHost(args));
        s.WhenStarted(h => h.Start());
        s.WhenStopped(h => h.Stop());
    });

    x.RunAsLocalSystem().StartAutomatically();

    var serviceName = nameof(MockSocket);

#if DEBUG
    // 避免与Release版本 svcName 冲突，无法直接调试
    serviceName = $"{serviceName}Debug";
#endif

    x.SetServiceName(serviceName);
    x.SetDisplayName("MockSocket Agent");
    x.SetDescription("MockSocket Agent is Powered by .NET 7.0");

    // Run a callback after installation completes to start the service.
    x.AfterInstall(() =>
    {
        using var controller = new ServiceController(serviceName);
        controller.Start();
    });
});