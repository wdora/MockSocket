using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Common.Exceptions;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Extensions;
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

    Lazy<string> sender;
    Lazy<string> receiver;

    public string SendId => sender.Value;
    public string ReceiveId => receiver.Value;

    bool isAccepted;

    public TcpClient(ILogger<TcpClient> logger, IBufferService bufferService, IMemorySerializer memorySerializer, IOptions<CommonConfig> config)
    {
        this.logger = logger;
        this.bufferService = bufferService;
        this.memorySerializer = memorySerializer;
        this.config = config.Value;

        sender = new Lazy<string>(() => $"{client.LocalEndPoint} -> {client.RemoteEndPoint}");
        receiver = new Lazy<string>(() => $"{client.RemoteEndPoint} -> {client.LocalEndPoint}");
    }

    public TcpClient WithSocket(Socket socket)
    {
        client = socket;
        isAccepted = true;
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

        logger.LogInformation("{id} received data: {model}", ReceiveId, model);

        return model;
    }

    public async ValueTask<int> ReceiveBytesAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var len = await client.ReceiveAsync(buffer, cancellationToken);

        if (len == 0)
            throw new ConnectionAbortedException(ReceiveId!);

        logger.LogDebug("{id} received bytes: {len}", ReceiveId, len);

        return len;
    }

    public async ValueTask SendAsync<T>(T model, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var len = memorySerializer.Serialize(model, buffer);

        await client.SendAsync(buffer.SliceTo(len));

        logger.LogInformation("{id} sent data: {model}", SendId, model);
    }

    public async ValueTask SendBytesAsync(ReadOnlyMemory<byte> readOnlyMemory, CancellationToken cancellationToken)
    {
        await client.SendAsync(readOnlyMemory, cancellationToken);

        logger.LogDebug("{id} sent bytes: {len}", SendId, readOnlyMemory.Length);
    }

    public void EnableKeepAlive()
    {
        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, config.HeartInterval);
        client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3);

        logger.LogInformation("{id} enable KeepAlive", SendId);
    }

    public override string ToString()
    {
        return isAccepted ? ReceiveId : SendId;
    }

    public void RegisterClosed(Action dispose, CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(100);

                var isConnect = IsConnected(client);

                if (!isConnect)
                {
                    logger.LogInformation("The connection({id}) was found to be closed", this);

                    dispose();
                    return;
                }
            }
        }, cancellationToken);
    }

    bool IsConnected(Socket so)
    {
        try
        {
            return so.IsConnected();
        }
        catch (Exception)
        {
            // 如：System.ObjectDisposedException: Cannot access a disposed object
            return false;
        }
    }
}
