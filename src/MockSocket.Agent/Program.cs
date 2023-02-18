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

    x.SetDescription("MockSocket Agent is Powered by .NET 7.0");
    x.SetDisplayName("MockSocket Agent");
    x.SetServiceName("MockSocket");
});

//var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
//Environment.ExitCode = exitCode;