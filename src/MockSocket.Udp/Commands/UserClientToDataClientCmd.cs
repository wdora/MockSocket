using MediatR;

namespace MockSocket.Udp.Commands;

public record class UserClientToDataClientCmd(string userClientId) : IRequest;