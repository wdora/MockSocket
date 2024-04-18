using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Interfaces
{
    public interface IBufferService
    {
        public const int TCP_BUFFER_SIZE = 1024;

        public const int UDP_BUFFER_SIZE = 65536;

        Task RunAsync(Func<Memory<byte>, Task> func, int bufferSize = TCP_BUFFER_SIZE);

        BufferResult Rent(int bufferSize = TCP_BUFFER_SIZE);
    }

    public class BufferResult : IDisposable
    {
        byte[] buffer;

        public BufferResult(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        public ReadOnlyMemory<byte> SliceTo(int len)
        {
            Memory<byte> memory = buffer;

            return memory.Slice(0, len);
        }

        public static implicit operator Memory<byte>(BufferResult result) => result.buffer;

        public static implicit operator ReadOnlyMemory<byte>(BufferResult result) => result.buffer;

        public static implicit operator Span<byte>(BufferResult result) => result.buffer;
    }
}
