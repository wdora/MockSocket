using MockClient.Udp.Interfaces;
using System.Collections.Concurrent;
using System.Net;

namespace MockSocket.Udp.Models;

public class UserClientContext
{
    public UserClientContext(IUdpServer appServer, IPEndPoint userClientEP)
    {
        AppServer = appServer;
        UserClientEP = userClientEP;
    }

    public IPEndPoint UserClientEP { get; set; }

    public ConcurrentQueue<ByteArray> Queue { get; set; } = new ConcurrentQueue<ByteArray>();

    public IUdpServer AppServer { get; set; }

    public IPEndPoint? DataClientEP { get; set; }
}
