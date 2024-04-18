using Microsoft.Extensions.Logging;
using MockSocket.Core.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Core.Services
{
    public class MockUdpServer : IUdpServer
    {
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        ILogger logger;

        public MockUdpServer(ILogger<MockUdpServer> logger)
        {
            this.logger = logger;
        }

        public void Listen(int port)
        {
            var ep = new IPEndPoint(IPAddress.Any, port);

            socket.Bind(ep);

            logger.LogInformation("服务监听成功: udp://" + ep);
        }

        static IPEndPoint AnyIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public async ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var receiveResult = await socket.ReceiveFromAsync(buffer, SocketFlags.None, AnyIPEndPoint);

            return new ReceiveFromResult
            {
                ReceivedBytes = receiveResult.ReceivedBytes,
                RemoteEndPoint = (receiveResult.RemoteEndPoint as IPEndPoint)!
            };
        }

        public async ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken)
        {
            await socket.SendToAsync(buffer, remoteEP, cancellationToken);
        }
    }

    public class MockUdpClient : IUdpClient
    {
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        static IPEndPoint AnyIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public async ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            if (remoteEP == default)
                remoteEP = AnyIPEndPoint;

            var receiveResult = await socket.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);

            return new ReceiveFromResult
            {
                ReceivedBytes = receiveResult.ReceivedBytes,
                RemoteEndPoint = (receiveResult.RemoteEndPoint as IPEndPoint)!
            };
        }

        public async ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            await socket.SendToAsync(buffer, remoteEP, cancellationToken);
        }

        public ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => ReceiveFromAsync(buffer, default, cancellationToken);
    }
}
