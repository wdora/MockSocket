using System.Net;
using System.Net.Sockets;

namespace MockSocket.Core.Configurations
{
    public class MockAgentConfig
    {
        public ServerEndpoint MockServer { get; set; } = new("wdora.com", 9090);

        public IPEndPoint UdpMockServer { get; set; } = GetIPEndpoint("localhost:9090");

        public IPEndPoint UdpRealServer { get; set; } = GetIPEndpoint("localhost:3389");

        private static IPEndPoint GetIPEndpoint(string hostOrAddressWithPort)
        {
            var arr = hostOrAddressWithPort.Split(':');

            var ip = Dns.GetHostAddresses(arr[0]).First(x => x.AddressFamily == AddressFamily.InterNetwork);

            return new IPEndPoint(ip, int.Parse(arr[1]));
        }

        public ServerEndpoint RealServer { get; set; } = new("localhost", 3389);

        public AppServer AppServer { get; set; } = new(3389, "tcp");

        public AppServer UdpAppServer { get; set; } = new(3390, "udp");

        public int HeartInterval { get; set; } = 30;

        public int RetryInterval { get; set; } = 3;
    }

    public record ServerEndpoint(string Host, int Port)
    {
        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }

    public record AppServer(int Port, string Protocal)
    {
        public override string ToString()
        {
            // net.tcp://0.0.0.0:8080
            return $"net.{Protocal}://0.0.0.0:{Port}";
        }
    }
}