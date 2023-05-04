using MockSocket.Common.Constants;
using MockSocket.Common.Models;

namespace MockSocket.Common.Interfaces;

public interface IBufferService
{
    BufferResult Rent(int bufferSize = BufferSizes.Tcp);
}
