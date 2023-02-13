using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket
{
    public interface IEncoder
    {
        int Encode<T>(T model, Span<byte> bytes);

        T Decode<T>(ReadOnlySpan<byte> bytes)
            where T : class;
    }
}
