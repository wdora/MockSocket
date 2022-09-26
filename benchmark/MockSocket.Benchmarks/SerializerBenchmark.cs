// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Text.Json;

[MemoryDiagnoser]
public class SerializerBenchmark
{
    private static object obj = new DemoUser { Age = 50, Id = Guid.NewGuid(), RefId = Guid.NewGuid() };
    byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);

    byte[] bytesData = default;
    string strData = JsonSerializer.Serialize(obj);

    public SerializerBenchmark()
    {
        using var stream = new MemoryStream(buffer);
        JsonSerializer.Serialize(stream, obj);
        stream.SetLength(stream.Position);

        bytesData = stream.ToArray();
    }


    [Benchmark]
    public void StringSerialize()
    {
        var json = JsonSerializer.Serialize(obj);
    }

    [Benchmark]
    public void StringDeserialize()
    {
        var user = JsonSerializer.Deserialize<DemoUser>(strData);
    }

    [Benchmark]
    public void MemorySerialize()
    {
        using var stream = new MemoryStream(buffer);
        JsonSerializer.Serialize(stream, obj);
    }

    [Benchmark]
    public void MemoryDeserialize()
    {
        using var stream = new MemoryStream(bytesData);
        var user = JsonSerializer.Deserialize<DemoUser>(stream);
    }

    [Benchmark]
    public void Utf8ReaderDeserialize()
    {
        var reader = new Utf8JsonReader(bytesData);
        var user = JsonSerializer.Deserialize<DemoUser>(ref reader);
    }

    internal class DemoUser
    {
        public int Age { get; set; }
        public Guid Id { get; set; }
        public Guid RefId { get; internal set; }
    }
}

