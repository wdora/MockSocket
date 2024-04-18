using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Exceptions;
using MockSocket.Udp.Utilities;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Services;
public class UdpServer : IUdpServer
{
    Socket udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    IMemorySerializer serializer;
    IBufferService bufferService;
    ILogger logger;

    public void Listen(int port)
    {
        try
        {
            localEndPoint = new IPEndPoint(IPAddress.Any, port);

            udpServer.Bind(localEndPoint);

            logger.LogDebug("Port {port} is now listening.", localEndPoint);
        }
        catch (Exception)
        {
            throw new PortConflictException(port);
        }
    }

    static EndPoint anyRemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private IPEndPoint localEndPoint;

    public UdpServer(IMemorySerializer serializer, IBufferService bufferService, ILogger<UdpServer> logger)
    {
        this.serializer = serializer;
        this.bufferService = bufferService;
        this.logger = logger;
    }

    public async ValueTask<(int length, IPEndPoint clientEP)> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var response = await udpServer.ReceiveFromAsync(buffer, SocketFlags.None, anyRemoteEndPoint);
        
        var clientEP = (IPEndPoint)response.RemoteEndPoint;

        CurrentContext.ClientEP = clientEP;

        return (response.ReceivedBytes, clientEP);
    }

    public void Dispose()
    {
        udpServer?.Dispose();

        logger.LogDebug("Port {port} is now released.", localEndPoint);
    }

    public async ValueTask SendToAsync(IPEndPoint toEP, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await udpServer.SendToAsync(buffer, toEP, cancellationToken);
    }

    public async ValueTask SendAsync<T>(IPEndPoint toEP, T model, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var len = serializer.Serialize(model, buffer);

        await SendToAsync(toEP, buffer.SliceTo(len), cancellationToken);

        logger.LogDebug("Sent data to {IP}: {Data}", toEP, model);
    }

    public async ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var (length, clientEP) = await ReceiveFromAsync(buffer, cancellationToken);

        var model = serializer.Deserialize<T>(buffer.SliceTo(length).Span);

        logger.LogDebug("Received data from {IP}: {Data}", clientEP, model);

        return model;
    }
}
