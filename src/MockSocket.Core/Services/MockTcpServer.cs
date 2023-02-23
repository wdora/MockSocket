using Microsoft.Extensions.Logging;
using MockSocket.Core.Interfaces;
using MockSocket.Core.Services;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Server
{
    public class MockTcpServer : IMockTcpServer
    {
        protected readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        ILogger logger;
        IPEndPoint localEP = null!;

        public MockTcpServer(ILogger<MockTcpServer> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<MockTcpClient> AcceptAsync(CancellationToken cancellationToken)
        {
            var client = await _socket.AcceptAsync(cancellationToken);

            return new MockTcpClient(client);
        }

        public void Dispose()
        {
            // this's endpoint couldn't visit after dispose 
            logger.LogInformation("服务停止监听:" + this);

            _socket.Dispose();
        }

        public ValueTask ListenAsync(int listenPort)
        {
            localEP = new IPEndPoint(IPAddress.Any, listenPort);

            _socket.Bind(localEP);

            _socket.Listen();

            logger.LogInformation("服务开始监听:" + this);

            return default;
        }


        public override string ToString()
        {
            return localEP?.ToString() ?? "";
        }
    }
}
