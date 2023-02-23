using MockSocket.Core.Tcp;

namespace MockSocket.Server
{
    public class CurrentContext
    {
        static AsyncLocal<MockTcpClient> LocalAgent = new AsyncLocal<MockTcpClient>();

        public static MockTcpClient Agent
        {
            get => LocalAgent.Value!;

            set => LocalAgent.Value = value;
        }
    }
}
