using MockSocket.Common.Interfaces;
using MockSocket.Common.Models;
using System.Buffers;

namespace MockSocket.Common.Services;

public class BufferService : IBufferService
{
    public BufferResult Rent(int bufferSize = 1024)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        return new BufferResult(buffer, () => ArrayPool<byte>.Shared.Return(buffer));
    }
}
