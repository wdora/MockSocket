// See https://aka.ms/new-console-template for more information

using Topshelf;

var rc = HostFactory.Run(x =>
{
    x.Service<MockHost>(s =>
    {
        s.ConstructUsing(_ => new MockHost(args));
        s.WhenStarted(h => h.Start());
        s.WhenStopped(h => h.Stop());
    });

    x.StartAutomatically().RunAsLocalSystem();

    x.SetServiceName("MockSocket");
    x.SetDisplayName("MockSocket Agent");
    x.SetDescription("MockSocket Agent is Powered by .NET 7.0");
});