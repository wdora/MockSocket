using Microsoft.Extensions.Logging;
using MockClient.Udp.Interfaces;
using MockSocket.Common.Interfaces;
using MockSocket.Udp.Exceptions;
using System;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MockClient.Udp.Services;
public class UdpClient : IUdpClient
{
    Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    IMemorySerializer memorySerializer;
    IBufferService bufferService;
    ILogger logger;
    IPEndPoint serverEP;

    public UdpClient(IMemorySerializer memorySerializer, IBufferService bufferService, ILogger<UdpClient> logger)
    {
        this.memorySerializer = memorySerializer;
        this.bufferService = bufferService;
        this.logger = logger;
    }

    static IPEndPoint anyEP = new IPEndPoint(IPAddress.Any, 0);

    public void Connect(IPEndPoint serverEP)
    {
        this.serverEP = serverEP;
        //udpClient.Bind(anyEP);
    }

    public async ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        try
        {
            var length = await ReceiveFromAsync(serverEP, buffer, cancellationToken);

            var model = memorySerializer.Deserialize<T>(buffer.SliceTo(length).Span);

            logger.LogDebug("Received data from {IP}: {Data}", serverEP, model);

            return model;
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10054)
                throw new ServiceUnavailableException(serverEP);
            throw;
        }
    }

    public async ValueTask<int> ReceiveFromAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var len = await ReceiveFromAsync(serverEP, buffer, cancellationToken);

        logger.LogDebug("Received data from {srcIP} to {dstIP}, data size:{size}", udpClient.LocalEndPoint, serverEP, len);

        return len;
    }

    public async ValueTask<int> ReceiveFromAsync(IPEndPoint serverEP, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var res = await udpClient.ReceiveFromAsync(buffer, serverEP, cancellationToken);

        return res.ReceivedBytes;
    }

    public async ValueTask SendAsync<T>(T model, CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var len = memorySerializer.Serialize(model, buffer);

        await SendToAsync(serverEP, buffer.SliceTo(len), cancellationToken);

        logger.LogDebug("Received data from {IP}: {Data}", serverEP, model);
    }

    public async ValueTask SendToAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        await SendToAsync(serverEP, buffer, cancellationToken);

        logger.LogDebug("Sent data from {srcIP} to {dstIP}, data size:{size}", udpClient.LocalEndPoint, serverEP, buffer.Length);
    }

    async ValueTask SendToAsync(IPEndPoint serverEP, ReadOnlyMemory<byte> readOnlyMemory, CancellationToken cancellation)
    {
        await udpClient.SendToAsync(readOnlyMemory, SocketFlags.None, serverEP, cancellation);
    }

    public override string ToString()
    {
        return $"{udpClient.LocalEndPoint}->{serverEP}";
    }
}
