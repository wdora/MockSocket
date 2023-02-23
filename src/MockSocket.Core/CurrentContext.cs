using MockSocket.Core.Interfaces;

namespace MockSocket.Server
{
    public class CurrentContext
    {
        static AsyncLocal<IMockTcpClient> LocalAgent = new AsyncLocal<IMockTcpClient>();

        public static IMockTcpClient Agent
        {
            get => LocalAgent.Value!;

            set => LocalAgent.Value = value;
        }
    }
}
