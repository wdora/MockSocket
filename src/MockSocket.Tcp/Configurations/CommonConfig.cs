namespace MockSocket.Tcp.Configurations;

public record class CommonConfig
{
    public bool Enable { get; set; } = true;

    public int HeartInterval { get; set; } = 45;
}
