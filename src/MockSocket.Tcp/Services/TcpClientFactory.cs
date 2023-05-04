using Microsoft.Extensions.Logging;
using MockSocket.Common.Interfaces;
using MockSocket.Tcp.Interfaces;
using System.Net.Sockets;

namespace MockSocket.Tcp.Services;
public class TcpClientFactory
{
    readonly ILogger<TcpClient> logger;
    readonly IBufferService bufferService;
    readonly IMemorySerializer memorySerializer;

    public TcpClientFactory(ILogger<TcpClient> logger, IBufferService bufferService, IMemorySerializer memorySerializer)
    {
        this.logger = logger;
        this.bufferService = bufferService;
        this.memorySerializer = memorySerializer;
    }

    public ITcpClient Create(Socket socket)
    {
        return new TcpClient(logger, bufferService, memorySerializer).WithSocket(socket);
    }
}
