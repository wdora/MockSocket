using Microsoft.Extensions.Logging;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Tcp.Services;
public class TcpServer : ITcpServer
{
    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    IBufferService bufferService;

    IMemorySerializer memorySerializer;

    TcpClientFactory clientFactory;

    ILogger logger;

    public TcpServer(IBufferService bufferService, IMemorySerializer memorySerializer, TcpClientFactory clientFactory, ILogger<TcpServer> logger)
    {
        this.bufferService = bufferService;
        this.memorySerializer = memorySerializer;
        this.clientFactory = clientFactory;
        this.logger = logger;
    }

    public async ValueTask<ITcpClient> AcceptAsync(CancellationToken cancellationToken)
    {
        var client = await server.AcceptAsync(cancellationToken);

        return clientFactory.Create(client);
    }

    public void Dispose()
    {
        logger.LogInformation("Port {port} is now disposing.", server.LocalEndPoint);

        server.Dispose();
    }

    public void Listen(int port)
    {
        try
        {
            var localEP = new IPEndPoint(IPAddress.Any, port);

            server.Bind(localEP);

            server.Listen();

            logger.LogInformation("Port {port} is now listening.", localEP);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Listen error");

            throw;
        }

    }

    public async ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken)
    {
        using var buffer = bufferService.Rent();

        var length = await server.ReceiveAsync(buffer, cancellationToken);

        return memorySerializer.Deserialize<T>(buffer.SliceTo(length).Span);
    }
}
