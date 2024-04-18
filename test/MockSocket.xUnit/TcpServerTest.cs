using MockSocket.Tcp.Extensions;
using Shouldly;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.xUnit;
public class TcpServerTest
{
    private readonly int anyPort = 0;

    [Fact]
    public async Task IsConnected_ShouldBeFalse_When_Close_Connection()
    {
        // arrange
        var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var ep = new IPEndPoint(IPAddress.Loopback, anyPort);

        server.Bind(ep);

        server.Listen();

        var c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await c.ConnectAsync(server.LocalEndPoint!);

        var client = await server.AcceptAsync();

        client.Connected.ShouldBeTrue();
        client.IsConnected().ShouldBeTrue();

        // act
        c.Close();

        // assert
        client.Connected.ShouldBeTrue();
        client.IsConnected().ShouldBeFalse();
    }
}
