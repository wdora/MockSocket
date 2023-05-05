using System.Net;

namespace MockSocket.Tcp.Configurations;
public record class MockAgentConfig : CommonConfig
{
    public string MockServerAddress { get; set; } = "127.0.0.1";

    public int MockServerPort { get; set; } = 9090;

    public int RealServerPort { get; set; } = 3389;

    public int AppServerPort { get; set; } = 10011;

    //public IPEndPoint MockServerEP => IPEndPoint.Parse($"{Dns.GetHostByName(MockServerAddress).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)}:{MockServerPort}");
    public DnsEndPoint MockServerEP => new DnsEndPoint(MockServerAddress, MockServerPort);

    public IPEndPoint RealServerEP => IPEndPoint.Parse($"127.0.0.1:{RealServerPort}");
}
