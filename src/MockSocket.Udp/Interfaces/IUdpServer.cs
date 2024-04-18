using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Interfaces
{
    public interface IUdpServer : IDisposable
    {
        void Listen(int port);

        ValueTask<(int length, IPEndPoint clientEP)> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        ValueTask SendToAsync(IPEndPoint toEP, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);

        ValueTask SendAsync<T>(IPEndPoint toEP, T model, CancellationToken cancellationToken);

        ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken);
    }

    public struct UdpResponnse
    {
        public int BytesLength { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }
    }
}
