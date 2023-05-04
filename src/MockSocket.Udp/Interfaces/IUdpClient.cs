using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Interfaces
{
    public interface IUdpClient : IDisposable
    {
        void Connect(IPEndPoint serverEP);

        ValueTask SendAsync<T>(T model, CancellationToken cancellationToken);

        ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken);

        ValueTask<int> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
    }
}
