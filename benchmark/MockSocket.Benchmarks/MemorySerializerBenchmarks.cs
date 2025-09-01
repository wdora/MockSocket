using System.Buffers;
using BenchmarkDotNet.Attributes;
using CommunityToolkit.HighPerformance.Buffers;
using MockSocket.Common.Services;

// | Method                           | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
// |--------------------------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
// | SerializeToSpan                  | 287.4 ns | 5.70 ns | 6.10 ns |  1.00 |    0.03 | 0.0315 |     400 B |        1.00 |
// | SerializeToArrayPoolBufferWriter | 230.1 ns | 3.52 ns | 3.12 ns |  0.80 |    0.02 | 0.0315 |     400 B |        1.00 |
    
[SimpleJob]
[MemoryDiagnoser]
public class MemorySerializerBenchmarks
{
    MemorySerializer ms = new();

    private object obj = new DemoUser(1, 3, "Jack");

    [Benchmark(Baseline = true)]
    public int SerializeToSpan()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024 * 4);
        try
        {
            return ms.Serialize(obj, buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    [Benchmark]
    public int SerializeToArrayPoolBufferWriter()
    {
        using var writer = new ArrayPoolBufferWriter<byte>(1024 * 4);
        ms.Serialize(obj, writer);
        return writer.WrittenCount;
    }
}