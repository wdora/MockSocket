using MockSocket.Abstractions.Udp;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Connection.Udp
{
    public class UdpServerConnection : IUdpServerConnection
    {
        private Socket socket;

        private IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, IPEndPoint.MinPort);

        public UdpServerConnection(Socket socket)
        {
            this.socket = socket;
        }

        public UdpServerConnection()
            : this(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {

        }

        public ValueTask ListenAsync(IPEndPoint localEP)
        {
            socket.Bind(localEP);

            return ValueTask.CompletedTask;
        }

        public ValueTask<SocketReceiveFromResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return socket.ReceiveFromAsync(buffer, SocketFlags.None, ep, cancellationToken);
        }

        public ValueTask<int> SendAsync(EndPoint remoteEP, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return socket.SendToAsync(buffer, SocketFlags.None, remoteEP, cancellationToken);
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
