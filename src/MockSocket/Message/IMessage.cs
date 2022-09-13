using MockSocket.Abstractions.Serializer;
using MockSocket.Core.Tcp;
using System;
using System.Net;
using System.Text;

namespace MockSocket.Message
{
    public interface IMessage
    {
        string MessageType { get; }
    }

    public class MessageEncoding
    {
        public const string TAG = ":";

        public readonly static Encoding Default = Encoding.UTF8;

        public static byte TAGByte = Default.GetBytes(TAG).First();

        public static string Decode(Span<byte> sourceBytes)
        {
            var (tagIndex, length) = GetTagIndex(sourceBytes);

            return Default.GetString(sourceBytes.Slice(tagIndex + 1, length).ToArray());
        }

        public static (int index, int length) GetTagIndex(Span<byte> sourceBytes)
        {
            int index = -1, length = 0;

            for (int i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] == TAGByte)
                {
                    index = i;
                    break;
                }
            }

            var lengthBytes = sourceBytes.Slice(0, index).ToArray();

            var lengthStr = Default.GetString(lengthBytes);

            length = int.Parse(lengthStr);

            return (index, length);
        }

        public static int GetTagLength(Span<byte> sourceBytes)
        {
            var lengthStr = Default.GetString(sourceBytes);

            var length = int.Parse(lengthStr);

            return length;
        }

        public static byte[] Encode(string srcStr)
        {
            var str = Default.GetByteCount(srcStr) + TAG + srcStr;

            return Default.GetBytes(str);
        }

        public static byte[] Encode<T>(T model)
            => Encode(JsonService.Serialize(model));
    }

    public class BaseMessage : IMessage
    {
        public string MessageType => GetType().FullName!;

        public static T CreateMessage<T>(string raw)
            where T : class, IMessage
        {
            try
            {
                var messageType = JsonService.Deserialize(raw, new { MessageType = "" }).MessageType;

                if (messageType == null)
                    throw new ArgumentException(nameof(messageType));

                var type = Type.GetType(messageType)!;

                var realObj = JsonService.Deserialize(raw, type) as T;

                return realObj!;
            }
            catch (Exception e)
            {
                Console.WriteLine("###" + raw + "###!");
                throw;
            }
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
