using MediatR;
using MockSocket.Common.Models;

namespace MockSocket.Udp.Commands;
public record class DataClientToUserClientCmd(string userClientId, BufferResult buffer, int length) : IRequest;
