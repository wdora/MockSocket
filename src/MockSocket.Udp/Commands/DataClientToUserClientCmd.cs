using MediatR;
using MockSocket.Common.Interfaces;

namespace MockSocket.Udp.Commands;
public record class DataClientToUserClientCmd(string userClientId, BufferResult buffer, int length) : IRequest;
