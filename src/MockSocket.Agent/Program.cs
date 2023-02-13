// See https://aka.ms/new-console-template for more information

using MockSocket.Agent;
using Topshelf;

var rc = HostFactory.Run(x =>                                   //1
{
    x.Service<HoleClientService>(s =>                                   //2
    {
        s.ConstructUsing(name => new HoleClientService());                //3
        s.WhenStarted(tc => tc.Start(args));                         //4
        s.WhenStopped(tc => tc.Stop());                          //5
    });

    x.RunAsLocalSystem();                                       //6

    x.SetDescription("MockSocket Agent");                   //7
    x.SetDisplayName("MockSocket Agent");                                  //8
    x.SetServiceName("MockSocket Agent");                                  //9
});                                                             //10

var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
Environment.ExitCode = exitCode;