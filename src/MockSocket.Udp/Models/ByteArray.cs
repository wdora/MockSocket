using MockSocket.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
