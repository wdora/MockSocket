using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Common.Exceptions;
using MockSocket.Common.Interfaces;
using MockSocket.Common.Models;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Tcp.Services;
public class TcpClient : ITcpClient
{
    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    ILogger logger;

    IBufferService bufferService;

    IMemorySerializer memorySerializer;

    CommonConfig config;

    public TcpClient(ILogger<TcpClient> logger, IBufferService bufferService, IMemorySerializer memorySerializer, IOptions<CommonConfig> config)
    {
        this.logger = logger;
        this.bufferService = bufferService;
        this.memorySerializer = memorySerializer;
        this.config = config.Value;
    }

    public TcpClient WithSocket(Socket socket)
    {
        client = socket;

        return this;
    }

    public async ValueTask ConnectAsync(EndPoint serverEP)
    {
        try
        {
            await client.ConnectAsync(serverEP);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                throw new ServiceUnavailableException(serverEP);
            throw;
        }
    }

    public void Dispose() => client.Dispose();

    public async ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var len = await client.ReceiveAsync(buffer, cancellationToken);

        var model = memorySerializer.Deserialize<T>(buffer.SliceTo(len).Span);
        
        logger.LogDebug("{id} 收到数据: {model}", this, model);

        return model;
    }

    public async ValueTask<int> ReceiveBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var len = await client.ReceiveAsync(buffer, cancellationToken);

        if (len == 0)
            throw new ConnectionAbortedException(ToString());

        logger.LogDebug("Received bytes: {len}", len);

        return len;
    }

    public async ValueTask SendAsync<T>(T model, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var len = memorySerializer.Serialize(model, buffer);

        await client.SendAsync(buffer.SliceTo(len));
    }

    public async ValueTask SendBytesAsync(ReadOnlyMemory<byte> readOnlyMemory, CancellationToken cancellationToken)
    {
        await client.SendAsync(readOnlyMemory, cancellationToken);

        logger.LogDebug("Sent bytes: {len}", readOnlyMemory.Length);
    }

    public override string ToString()
    {
        return $"{client.LocalEndPoint}->{client.RemoteEndPoint}";
    }

    public void EnableKeepAlive()
    {
        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, config.HeartInterval);
        client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3);
    }
}
