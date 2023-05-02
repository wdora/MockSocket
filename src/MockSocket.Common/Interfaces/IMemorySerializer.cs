using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Common.Interfaces;

public interface IMemorySerializer
{
    T Deserialize<T>(ReadOnlySpan<byte> buffer);

    int Serialize<T>(T obj, Span<byte> buffer);
}
