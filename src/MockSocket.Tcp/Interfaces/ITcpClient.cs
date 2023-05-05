using MockSocket.Common.Models;
using System.Net;

namespace MockSocket.Tcp.Interfaces;

public interface ITcpClient : IDisposable
{
    ValueTask ConnectAsync(EndPoint serverEP);

    void EnableKeepAlive();

    ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken);
    
    ValueTask<int> ReceiveBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken);

    ValueTask SendAsync<T>(T model, CancellationToken cancellationToken);
    
    ValueTask SendBytesAsync(ReadOnlyMemory<byte> readOnlyMemory, CancellationToken cancellationToken);
}