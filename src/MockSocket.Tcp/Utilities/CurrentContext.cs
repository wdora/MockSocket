using MockSocket.Tcp.Interfaces;

namespace MockSocket.Tcp.Utilities;

public class CurrentContext
{
    public static AsyncLocal<ITcpClient> Agent = new AsyncLocal<ITcpClient>();
}
