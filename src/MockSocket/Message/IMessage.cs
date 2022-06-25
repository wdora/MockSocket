using MockSocket.Abstractions.Serializer;
using MockSocket.Core.Tcp;
using System.Net;

namespace MockSocket.Message
{
    public interface IMessage
    {
        string MessageType { get; }

    }

    public class BaseMessage : IMessage
    {
        public string MessageType => GetType().FullName!;

        public static T CreateMessage<T>(string raw)
            where T : class, IMessage
        {
            var messageType = JsonService.Deserialize(raw, new { MessageType = "" }).MessageType;

            if (messageType == null)
                throw new ArgumentException(nameof(messageType));

            var type = Type.GetType(messageType)!;

            var realObj = JsonService.Deserialize(raw, type) as T;

            return realObj!;
        }
    }

    public class TcpBaseMessage : BaseMessage
    {
        public ITcpConnection Connection { get; set; }
    }

    public class UdpBaseMessage : BaseMessage
    {
        public IPEndPoint AgentEP { get; set; }
    }
}
