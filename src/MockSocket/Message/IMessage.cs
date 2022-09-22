using MockSocket.Abstractions.Serializer;
using MockSocket.Core.Tcp;
using MockSocket.Message.Tcp;
using System;
using System.Buffers;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        static readonly Dictionary<string, Type> cacheDict;

        static BaseMessage()
        {
            cacheDict = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(x => x.BaseType == typeof(TcpBaseMessage))
                .ToDictionary(x => x.FullName!, x => x);
        }

        public static T CreateMessageByDict<T>(string raw)
            where T : class, IMessage
        {

            var messageType = JsonService.Deserialize(raw, new { MessageType = "" }).MessageType;

            if (messageType == null)
                throw new ArgumentException(nameof(messageType));

            if (!cacheDict.TryGetValue(messageType, out var type))
                return default;

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
