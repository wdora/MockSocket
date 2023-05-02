using MediatR;
using MockSocket.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Udp.Commands;

public record class UserDataCommand(BufferResult Buffer, int Length, IPEndPoint UserClientEP, IPEndPoint AgentEP, MockClient.Udp.Interfaces.IUdpServer udpAppServer) : IRequest;