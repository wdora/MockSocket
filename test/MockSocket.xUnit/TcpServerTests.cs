using MockSocket.Abstractions.Tcp;
using MockSocket.Connection.Tcp;
using System.Net;

namespace MockSocket.xUnit
{
    public class TcpServerTests
    {
        [Theory]
        [InlineData(BaseTest.ListenPort)]
        public async Task should_get_not_null_socket_when_socket_close(int port)
        {
            // arrange
            var server = new TcpServerConnection();
            await server.ListenAsync(port);

            using (var client = new TcpClientConnection())
                await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, port));

            // act
            var clientSo = await server.AcceptAsync();

            // assert
            Assert.NotNull(clientSo);
            Assert.False(clientSo.IsConnected);

            var client2 = new TcpClientConnection();
            await client2.ConnectAsync(new IPEndPoint(IPAddress.Loopback, port));
            server.Dispose();

            Assert.False(client2.IsConnected);
        }
    }
}
