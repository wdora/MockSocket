using Microsoft.Extensions.Hosting;

class MockHost
{
    IHost host;

    public MockHost(string[] args)
    {
        host = Host.CreateDefaultBuilder(args)
           .Build();
    }

    public void Start() => host.Start();

    public void Stop() => host.StopAsync().Wait();
}