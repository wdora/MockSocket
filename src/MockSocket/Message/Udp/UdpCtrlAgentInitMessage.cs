using MediatR;
using MockSocket.Abstractions.Udp;
using System.Net;

namespace MockSocket.Message.Udp
{
    public class UdpCtrlAgentInitMessage : UdpBaseMessage, IRequest
    {
        public int AppServerPort { get; set; }
    }

    public class UdpCtrlAgentNewClientMessage : UdpBaseMessage, IRequest
    {
        public byte[] RequestData { get; set; } = default!;

        public string UserClientId { get; set; } = default!;
    }

    public class UdpDataAgentMessage : UdpBaseMessage, IRequest
    {
        public byte[] ResponseData { get; set; } = default!;

        public string UserClientId { get; set; } = default!;
    }

    public class UdpDataAgentHandle : IRequestHandler<UdpDataAgentMessage>
    {
        IUdpConnection udpClientConnection;

        public UdpDataAgentHandle(IUdpConnection udpClientConnection)
        {
            this.udpClientConnection = udpClientConnection;
        }

        public async Task<Unit> Handle(UdpDataAgentMessage request, CancellationToken cancellationToken)
        {
            var clientEP = IPEndPoint.Parse(request.UserClientId);

            await udpClientConnection.SendAsync(clientEP, request.ResponseData, cancellationToken);

            return Unit.Value;
        }
    }
}
