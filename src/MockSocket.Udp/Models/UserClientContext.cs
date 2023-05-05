using MockClient.Udp.Interfaces;
using System.Net;

namespace MockSocket.Udp.Models;

public class UserClientContext
{
    public UserClientContext(IUdpServer appServer, IPEndPoint userClientEP, IPEndPoint dataClientEP)
    {
        AppServer = appServer;
        UserClientEP = userClientEP;
        DataClientEP = dataClientEP;
    }

    public IPEndPoint UserClientEP { get; set; }

    public IUdpServer AppServer { get; set; }

    public IPEndPoint DataClientEP { get; set; }
}
