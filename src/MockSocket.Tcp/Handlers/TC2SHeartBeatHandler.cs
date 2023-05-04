using MediatR;
using MockSocket.Tcp.Commands;

namespace MockSocket.Tcp.Handlers;
public class TC2SHeartBeatHandler : IRequestHandler<TC2SHeartBeatCmd>
{
    public Task Handle(TC2SHeartBeatCmd request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
