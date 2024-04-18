using System.Net;

namespace MockSocket.Core.Interfaces
{
    public interface IUdpClient
    {
        ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken = default);

        ValueTask<ReceiveFromResult> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, IPEndPoint remoteEP, CancellationToken cancellationToken = default);
    }
}
