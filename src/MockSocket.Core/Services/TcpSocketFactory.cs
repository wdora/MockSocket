using MockSocket.Core.Interfaces;
using System.Net.Sockets;

namespace MockSocket.Core.Services
{
    public class TcpSocketFactory
    {
        public static async ValueTask<IMockTcpClient> Create(string host, int port)
        {
            var so = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await so.ConnectAsync(host, port);

                return Create(so);
            }
            catch (Exception)
            {
                so.Dispose();
                throw;
            }
        }

        public static IMockTcpClient Create(Socket client)
        {
            return new MockTcpClient(client);
        }
    }
}