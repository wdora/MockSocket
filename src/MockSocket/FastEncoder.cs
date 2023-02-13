using System.Text;
using System.Text.Json;

namespace MockSocket
{
    public class FastEncoder : IEncoder
    {
        public static IEncoder Instance { get; } = new FastEncoder();

        public T Decode<T>(ReadOnlySpan<byte> bytes)
            where T : class
        {
            var len = BitConverter.ToInt32(bytes.Slice(0, 4));

            return JsonSerializer.Deserialize<T>(bytes.Slice(4, len))!;
        }

        public int Encode<T>(T model, Span<byte> bytes)
        {
            var json = JsonSerializer.Serialize(model);

            var count = Encoding.UTF8.GetBytes(json, bytes.Slice(4));

            BitConverter.TryWriteBytes(bytes, count);

            return count + 4;
        }
    }
}
