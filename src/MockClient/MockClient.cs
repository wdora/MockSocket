// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MockClient : IStartup
{
    private EndPoint remoteEP;

    public MockClient(EndPoint ep)
    {
        this.remoteEP = ep;
    }

    public async ValueTask StartAsync()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(remoteEP);

        var buffer = new byte[1024].AsMemory();

        while (true)
        {
            var message = Console.ReadLine();

            var count = Encoding.UTF8.GetBytes(message, buffer.Span);

            await socket.SendAsync(buffer.Slice(0, count));
        }
    }
}
