namespace MockSocket.Core.Configurations
{
    public class MockServerConfig
    {
        public int ListenPort { get; set; } = 9090;

        public int ListenUdpPort { get; set; } = 9090;

        public UdpServerConfig Udp { get; set; }
    }

    public class UdpServerConfig
    {
        public TimeSpan SlidingTimeout { get; set; }
    }
}