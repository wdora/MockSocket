using MockClient.Udp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Utilities;

public class CurrentContext
{
    //private static AsyncLocal<IPEndPoint> clientEP = new AsyncLocal<IPEndPoint>();

    //public static IPEndPoint ClientEP { get => clientEP.Value!; set => clientEP.Value = value; }
    public static IPEndPoint ClientEP { get; set; }

    public static IUdpServer MockServer { get; internal set; }

    public static IUdpClient MockAgent { get; internal set; }
}
