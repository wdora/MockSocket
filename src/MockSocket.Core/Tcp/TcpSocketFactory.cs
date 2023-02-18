namespace MockSocket.Core.Tcp
{
    public class TcpSocketFactory
    {
        public static async ValueTask<MockTcpClient> Create(string host, int port)
        {
            var client = new MockTcpClient();

            await client.ConnectAsync(host, port);

            return client;
        }
    }
}