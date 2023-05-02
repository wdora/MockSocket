using MockSocket.Core.Interfaces;
using System.Net;

namespace MockSocket.Server
{
    public class CurrentContext
    {
        static AsyncLocal<IMockTcpClient> LocalAgent = new AsyncLocal<IMockTcpClient>();

        public static IMockTcpClient Agent
        {
            get => LocalAgent.Value!;

            set => LocalAgent.Value = value;
        }

        public static IMockUdpClient UdpAgent { get; }

        public static IMockTcpClient CtrlAgent4Udp { get; set; }
        
        /// <summary>
        /// 非AppServer
        /// </summary>
        public static IUdpServer UdpMockServer { get; set; }

        public static IPEndPoint UdpEndPoint { get; set; }
    }

    public interface IMockUdpClient
    {
        string Id { get; }
    }
}
