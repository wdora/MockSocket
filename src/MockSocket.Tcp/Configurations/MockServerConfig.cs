namespace MockSocket.Tcp.Configurations;

public record class MockServerConfig : CommonConfig
{
    public int Port { get; set; } = 9090;
}