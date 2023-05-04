using System.Net;

namespace MockSocket.Tcp.Configurations;
public class MockAgentConfig
{
    public string MockServerAddress { get; set; } = "127.0.0.1";

    public int MockServerPort { get; set; } = 9090;
    public int RealServerPort { get; set; } = 3389;
        
    public IPEndPoint MockServerEP => IPEndPoint.Parse($"{MockServerAddress}:{MockServerPort}");

    public int AppServerPort { get; set; } = 10011;

    public int HeartInterval { get; internal set; } = 30;

    public IPEndPoint RealServerEP => IPEndPoint.Parse($"127.0.0.1:{RealServerPort}");
}
