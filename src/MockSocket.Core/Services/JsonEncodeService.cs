using System.Text;
using System.Text.Json;
using MockSocket.Core.Interfaces;

namespace MockSocket.Core.Services
{
    public class JsonEncodeService : IEncodeService
    {
        public static IEncodeService Instance { get; } = new JsonEncodeService();

        public T? Decode<T>(ReadOnlyMemory<byte> bytes, int len)
            where T : class
        {
            if (len == 0)
                return default;

            return JsonSerializer.Deserialize<T>(bytes.Slice(0, len).Span);
        }

        public object? Decode(ReadOnlyMemory<byte> bytes, int len, Type type)
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
}
