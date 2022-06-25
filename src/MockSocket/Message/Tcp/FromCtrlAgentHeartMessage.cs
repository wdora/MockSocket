using MediatR;

namespace MockSocket.Message.Tcp
{
    public class FromCtrlAgentHeartMessage : TcpBaseMessage, IRequest
    {

    }

    class CtrlAgentHeartHandle : IRequestHandler<FromCtrlAgentHeartMessage>
    {
        public Task<Unit> Handle(FromCtrlAgentHeartMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
