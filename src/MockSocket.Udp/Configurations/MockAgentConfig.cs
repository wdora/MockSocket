using System.Net;

namespace MockClient.Udp.Interfaces;

public class MockAgentConfig
{
    public string MockServerAddress { get; set; } = "127.0.0.1";

    public int MockServerPort { get; set; } = 9090;

    public int RealServerPort { get; set; } = 3389;

    public int AppServerPort { get; set; } = 10000;

    public IPEndPoint MockServerEP => IPEndPoint.Parse($"{MockServerAddress}:{MockServerPort}");

    public int HeartInterval { get; internal set; } = 30;

    public IPEndPoint RealServerEP => IPEndPoint.Parse($"127.0.0.1:{RealServerPort}");
}