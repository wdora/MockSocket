using MockSocket.Core.Interfaces;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Services
{
    public class BufferService : IBufferService
    {
        public BufferResult Rent(int bufferSize = 1024)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            return new BufferResult(buffer);
        }

        public async Task RunAsync(Func<Memory<byte>, Task> func, int bufferSize = 1024)
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
    }
}
