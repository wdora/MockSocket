using System.Buffers;
using System.Text;
using System.Text.Json;
using MockSocket.Core.Interfaces;

namespace MockSocket.Core.Services
{
    public class JsonEncodeService : IEncodeService
    {
        public static IEncodeService Instance { get; } = new JsonEncodeService();

        public T? Decode<T>(ReadOnlyMemory<byte> bytes, int len)
        {
            if (len == 0)
                return default;

            return JsonSerializer.Deserialize<T>(bytes.Slice(0, len).Span);
        }

        public object? Decode(ReadOnlyMemory<byte> bytes, int len, Type type)
        {
            if (len == 0)
                return default;

            return JsonSerializer.Deserialize(bytes.Slice(0, len).Span, type)!;
        }

        public int Encode<T>(T model, Memory<byte> bytes)
        {
            var json = JsonSerializer.Serialize(model);

            var count = Encoding.UTF8.GetBytes(json, bytes.Span);

            return count;
        }
    }

    public class MyJsonEncodeService : IEncodeService
    {
        public T? Decode<T>(ReadOnlyMemory<byte> bytes, int len)
        {
            throw new NotImplementedException();
        }

        public object? Decode(ReadOnlyMemory<byte> bytes, int len, Type type)
        {
            throw new NotImplementedException();
        }

        public int Encode<T>(T model, Memory<byte> buffer)
        {
            using var stream = new PerformanceStream(buffer);

            JsonSerializer.Serialize(stream, model);

            return (int)stream.Position;
        }
    }

    public class PerformanceStream : Stream
    {
        private Memory<byte> data;

        public PerformanceStream(Memory<byte> buffer)
        {
            this.data = buffer;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => data.Length;

        public override long Position { get; set; }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            data.CopyTo(buffer);

            return (int)Position;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Position = offset;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            buffer.CopyTo(data);

            Position += count;
        }
    }
}
