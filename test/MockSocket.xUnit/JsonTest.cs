using System.Buffers;
using System.Text;
using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
using MockSocket.Common.Services;
using Shouldly;

namespace MockSocket.xUnit;

public class JsonTest
{
    [Theory]
    [InlineData("123")]
    public void Simple(string input)
    {
        using var writer = new ArrayPoolBufferWriter<byte>();

        using var utf8Writer = new Utf8JsonWriter(writer);

        JsonSerializer.Serialize(utf8Writer, input);

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input));

        // 实际使用 不要ToArray
        var spanBuffer = writer.WrittenSpan.ToArray();

        bytes.ShouldBe(spanBuffer);
    }

    [Fact]
    public void Serialize()
    {
        var obj = new Demo { Id = 1, Name = "hell" };

        var ms = new MemorySerializer();

        Span<byte> buffer = ArrayPool<byte>.Shared.Rent(4 * 1024);

        var len = ms.Serialize(obj, buffer);

        var data1 = buffer[..len].ToArray();

        using var writer = new ArrayPoolBufferWriter<byte>();
        
        ms.Serialize(obj, writer);

        var data2 = writer.WrittenSpan.ToArray();

        data1.ShouldBe(data2);
    }

    class Demo
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}