using System.Net;
using System.Net.Sockets;

namespace MockSocket.Abstractions.Udp
{
    public interface IUdpConnection : IDisposable
    {
        ValueTask<SocketReceiveFromResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        ValueTask<int> SendAsync(EndPoint remoteEP, Memory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
