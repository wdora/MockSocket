using Microsoft.Extensions.DependencyInjection;
using MockSocket.Tcp.Interfaces;
using System.Net.Sockets;

namespace MockSocket.Tcp.Services;
public class TcpClientFactory
{
    readonly IServiceProvider serviceProvider;

    public TcpClientFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ITcpClient Create(Socket socket)
    {
        return (Create() as TcpClient)!.WithSocket(socket);
    }

    public ITcpClient Create()
    {
        return serviceProvider.GetService<ITcpClient>()!;
    }
}
