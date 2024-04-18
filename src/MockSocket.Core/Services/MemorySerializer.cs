using MockSocket.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MockSocket.Core.Services
{
    /// <summary>
    /// typeLength-typeName-dataLength-data
    /// 4-x-4-y
    /// </summary>
    public class MemorySerializer : IMemorySerializer
    {
        public T Deserialize<T>(ReadOnlySpan<byte> buffer)
        {
            var index = 0;

            var typeLength = BitConverter.ToInt32(buffer.Slice(index, 4));

            index += sizeof(int);

            var type = Type.GetType(Encoding.UTF8.GetString(buffer.Slice(index, typeLength)))!;

            index += typeLength;

            var dataLength = BitConverter.ToInt32(buffer.Slice(index, 4));

            index += sizeof(int);

            return (T)JsonSerializer.Deserialize(buffer.Slice(index, dataLength), type)!;
        }

        public int Serialize<T>(T obj, Span<byte> buffer)
        {
            var typeName = typeof(T).AssemblyQualifiedName!;
            var typeLength = Encoding.UTF8.GetByteCount(typeName);
            var data = JsonSerializer.SerializeToUtf8Bytes(obj);
            var dataLength = data.Length;
            var totalLength = sizeof(int) + typeLength + sizeof(int) + dataLength;

            if (buffer.Length < totalLength)
                throw new ArgumentException($"The buffer size is not enough. Expected: {totalLength}, actual: {buffer.Length}");

            var offset = 0;
            BitConverter.TryWriteBytes(buffer.Slice(offset, sizeof(int)), typeLength);
            offset += sizeof(int);

            Encoding.UTF8.GetBytes(typeName, buffer.Slice(offset, typeLength));
            offset += typeLength;

            BitConverter.TryWriteBytes(buffer.Slice(offset, sizeof(int)), dataLength);
            offset += sizeof(int);

            data.CopyTo(buffer.Slice(offset));

            return totalLength;
        }
    }
}
