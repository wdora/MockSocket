using System.Text;
using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
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
}