using CommunityToolkit.HighPerformance;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace MockSocket
{
    public class JsonEncoder : IEncoder
    {
        public static IEncoder Instance { get; } = new JsonEncoder();

        public T? Decode<T>(ReadOnlyMemory<byte> bytes, int len)
            where T : class
        {
            if (len == 0)
                return default;

            return JsonSerializer.Deserialize<T>(bytes.Slice(0, len).Span);
        }

        public object Decode(ReadOnlyMemory<byte> bytes, int len, Type type)
        {
            if (len == 0)
                return default;

            return JsonSerializer.Deserialize(bytes.Slice(0, len).Span, type)!;
        }

        public int Encode<T>(T model, Memory<byte> bytes)
        {
            var json = JsonSerializer.Serialize(model);

            var count = Encoding.UTF8.GetBytes(json, bytes.Span);
            
            return count;
        }
    }

    public class BinaryEncoder //: IEncoder
    {
        BinaryFormatter formmater = new();

        public T Decode<T>(ReadOnlyMemory<byte> bytes, int len) where T : class
        {
            //var len = BitConverter.ToInt32(bytes.Slice(0, HEAD_COUNT).Span);

            if (len == 0)
                return default!;

            using var stream = bytes.Slice(0, len).AsStream();

#pragma warning disable SYSLIB0011 // 类型或成员已过时
            return (T)formmater.Deserialize(stream);
        }

        public int Encode<T>(T model, Memory<byte> bytes)
        {
            if (model == null)
                return 0;

            using var stream = bytes.Slice(0).AsStream();

            formmater.Serialize(stream, model);
#pragma warning restore SYSLIB0011 // 类型或成员已过时

            var count = (int)stream.Position;

            //BitConverter.TryWriteBytes(bytes.Span, count);

            return count + 0;
        }
    }
}
