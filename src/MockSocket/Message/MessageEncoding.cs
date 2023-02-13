using MockSocket.Abstractions.Serializer;
using System.Text;

namespace MockSocket.Message
{
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

        public static int GetTagIndex(ReadOnlySpan<byte> sourceBytes)
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

        public static int FastGetTagLength(Span<byte> span)
        {
            return BitConverter.ToInt32(span);
        }

        public static int FastEncode(string srcStr, Memory<byte> memory)
        {
            var strCount = Default.GetByteCount(srcStr);

            BitConverter.TryWriteBytes(memory.Span, strCount);

            var len = sizeof(int);

            memory.Span[len] = TAGByte;
            len++;

            len += Default.GetBytes(srcStr, memory.Slice(len).Span);

            return len;
        }

        public static string FastDecode(ReadOnlySpan<byte> sourceBytes)
        {
            var tagIndex = GetTagIndex(sourceBytes);

            var length = BitConverter.ToInt32(sourceBytes.Slice(0, tagIndex));

            return Default.GetString(sourceBytes.Slice(tagIndex + 1, length));
        }

        public static int Encode<T>(T model, Memory<byte> memory)
            => FastEncode(JsonService.Serialize(model), memory);

        public static T Decode<T>(ReadOnlySpan<byte> span)
            => JsonService.Deserialize<T>(FastDecode(span));
    }
}
