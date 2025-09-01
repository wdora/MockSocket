using System.Buffers;
using MockSocket.Common.Interfaces;
using System.Text;
using System.Text.Json;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace MockSocket.Common.Services;

/// <summary>
/// typeLength-typeName-dataLength-data
/// 4-x-4-y
/// </summary>
public class MemorySerializer : IMemorySerializer
{
    public T Deserialize<T>(ReadOnlySpan<byte> buffer)
    {
        var index = 0;

        var typeLength = BitConverter.ToInt32(buffer.Slice(index, 4));

        index += sizeof(int);

        var type = Type.GetType(Encoding.UTF8.GetString(buffer.Slice(index, typeLength)))!;

        index += typeLength;

        var dataLength = BitConverter.ToInt32(buffer.Slice(index, 4));

        index += sizeof(int);

        return (T)JsonSerializer.Deserialize(buffer.Slice(index, dataLength), type)!;
    }

    public int Serialize<T>(T obj, Span<byte> buffer)
    {
        var typeName = typeof(T).AssemblyQualifiedName!;
        var typeLength = Encoding.UTF8.GetByteCount(typeName);

        using var writer = new ArrayPoolBufferWriter<byte>(1024 * 4);

        using var utf8Writer = new Utf8JsonWriter(writer);

        JsonSerializer.Serialize(utf8Writer, obj);

        var dataLength = writer.WrittenCount;

        var totalLength = sizeof(int) + typeLength + sizeof(int) + dataLength;

        if (buffer.Length < totalLength)
            throw new ArgumentException(
                $"The buffer size is not enough. Expected: {totalLength}, actual: {buffer.Length}");

        var offset = 0;
        BitConverter.TryWriteBytes(buffer.Slice(offset, sizeof(int)), typeLength);
        offset += sizeof(int);

        Encoding.UTF8.GetBytes(typeName, buffer.Slice(offset, typeLength));
        offset += typeLength;

        BitConverter.TryWriteBytes(buffer.Slice(offset, sizeof(int)), dataLength);
        offset += sizeof(int);

        writer.WrittenSpan.CopyTo(buffer.Slice(offset));

        return totalLength;
    }

    public void Serialize<T>(T obj, ArrayPoolBufferWriter<byte> writer)
    {
        var encoding = Encoding.UTF8;

        var typeName = typeof(T).AssemblyQualifiedName!;

        var typeLength = Encoding.UTF8.GetByteCount(typeName);

        writer.Write(typeLength);

        encoding.GetBytes(typeName, writer);

        var index = writer.WrittenCount;
        
        var size = sizeof(int);
        
        writer.Advance(size);

        using var utf8Writer = new Utf8JsonWriter(writer);

        JsonSerializer.Serialize(utf8Writer, obj);

        var datalength = writer.WrittenCount - index - size;

        var array = writer.DangerousGetArray();
        
        var span = new Span<byte>(array.Array).Slice(index, size);
        // 回写dataLength
        BitConverter.TryWriteBytes(span, datalength);
    }
}