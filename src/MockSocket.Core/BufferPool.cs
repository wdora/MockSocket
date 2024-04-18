using System.Buffers;

namespace MockSocket.Core
{
    public class BufferPool
    {
        public const int BUFFER_SIZE = 1024;
        public const int UDP_BUFFER_SIZE = 65536;

        public static BufferPool Instance { get; } = new BufferPool();

        public async ValueTask Run(Func<Memory<byte>, ValueTask> func, int bufferSize = BUFFER_SIZE)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                await func(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public async ValueTask<T> Run<T>(Func<Memory<byte>, ValueTask<T>> func, int bufferSize = BUFFER_SIZE)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                var model = await func(buffer);

                return model;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}