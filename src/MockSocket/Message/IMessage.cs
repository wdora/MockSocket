using MockSocket.Abstractions.Serializer;
using MockSocket.Core.Tcp;
using MockSocket.Message.Tcp;
using System;
using System.Buffers;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MockSocket.Message
{
    public interface IMessage
    {
        string MessageType { get; }
    }

    public class MessageEncoding
    {
        public const int BUFFER_SIZE = 1024 * 4;
        public const string TAG = ":";

        public readonly static Encoding Default = Encoding.UTF8;
        public static byte TAGByte = Default.GetBytes(TAG).First();

        public static string Decode(Span<byte> sourceBytes)
        {
            var tagIndex = GetTagIndex(sourceBytes);

            var length = GetTagLength(sourceBytes.Slice(0, tagIndex));

            return Default.GetString(sourceBytes.Slice(tagIndex + 1, length).ToArray());
        }

        public static int GetTagIndex(Span<byte> sourceBytes)
        {
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] == TAGByte)
                    return i;
            }

            return -1;
        }

        public static int GetTagLength(Span<byte> sourceBytes)
        {
            var lengthStr = Default.GetString(sourceBytes);

            var length = int.Parse(lengthStr);

            return length;
        }

        public static int Encode(string srcStr, Memory<byte> memory)
        {
            var str = Default.GetByteCount(srcStr) + TAG + srcStr;

            var len = Default.GetBytes(str, memory.Span);

            return len;
        }

        public static int Encode<T>(T model, Memory<byte> memory)
            => Encode(JsonService.Serialize(model), memory);
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
