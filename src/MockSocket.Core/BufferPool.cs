using System.Buffers;

namespace MockSocket.Core
{
    public class BufferPool
    {
        private readonly int BUFFER_SIZE = 1024;

        public static BufferPool Instance { get; } = new BufferPool();

        public async ValueTask Run(Func<Memory<byte>, ValueTask> func)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            try
            {
                await func(buffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public async ValueTask<T> Run<T>(Func<Memory<byte>, ValueTask<T>> func)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

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