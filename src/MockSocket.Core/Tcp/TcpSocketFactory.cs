namespace MockSocket.Core.Tcp
{
    public class TcpSocketFactory
    {
        public static async ValueTask<MockTcpClient> Create(string host, int port)
        {
            var client = new MockTcpClient();
            try
            {
                await client.ConnectAsync(host, port);

                return client;
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
        }
    }
}