namespace MockSocket.Tcp.Configurations;

public class MockServerConfig
{
    public int Port { get; set; } = 9090;

    public int HeartInterval { get; internal set; } = 30;

    public int ExpireUdpTime { get; internal set; } = 30;
}