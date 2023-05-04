using MockSocket.Common.Models;

namespace MockSocket.Udp.Models;
public class ByteArray
{
    public ByteArray(BufferResult buffer, int length)
    {
        Buffer = buffer;
        Length = length;
    }

    public BufferResult Buffer { get; }
    public int Length { get; }
}
