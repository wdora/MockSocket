using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Interfaces
{
    public interface IUdpServer
    {
        void Listen(int port);

        ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken);
    }

    public struct ReceiveFromResult
    {
        public int ReceivedBytes;

        public IPEndPoint RemoteEndPoint;
    }
}
