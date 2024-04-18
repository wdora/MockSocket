using MediatR;

namespace MockSocket.Message.Tcp
{
    public class CtrlAgent_HoleServer_Heart_Message : TcpBaseMessage, IRequest
    {

    }

    class CtrlAgentHeartHandle : IRequestHandler<CtrlAgent_HoleServer_Heart_Message>
    {
        public Task<Unit> Handle(CtrlAgent_HoleServer_Heart_Message request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
