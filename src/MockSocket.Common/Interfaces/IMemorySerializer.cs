using CommunityToolkit.HighPerformance.Buffers;

namespace MockSocket.Common.Interfaces;

public interface IMemorySerializer
{
    T Deserialize<T>(ReadOnlySpan<byte> buffer);

    int Serialize<T>(T obj, Span<byte> buffer);

    void Serialize<T>(T obj, ArrayPoolBufferWriter<byte> writer);
}
