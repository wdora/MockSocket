using MediatR;
using MockSocket.Abstractions.Udp;
using MockSocket.Message;
using System.Net;
using System.Text;

namespace MockSocket.HoleServer
{
    public class UdpHoleServer : IHoleServer
    {
        private readonly IUdpServerConnection server;
        private readonly HoleServerOption option;
        private readonly IMediator mediator;

        public UdpHoleServer(IUdpServerConnection server, HoleServerOption option, IMediator mediator)
        {
            this.server = server;
            this.option = option;
            this.mediator = mediator;
        }

        public void Dispose()
        {
            server.Dispose();
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await server.ListenAsync(option.ListenPort);

            var buffer = new Memory<byte>(new byte[4096]);

            while (true)
            {
                var result = await server.ReceiveAsync(buffer, cancellationToken);

                var json = Encoding.UTF8.GetString(buffer.Slice(0, result.ReceivedBytes).Span);

                var message = BaseMessage.CreateMessage<UdpBaseMessage>(json);

                message.AgentEP = (IPEndPoint)result.RemoteEndPoint;

                _ = mediator.Send(message, cancellationToken);
            }
        }
    }
}
