// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using MockSocket.Message;

[MemoryDiagnoser]
public class EncodeBencmarks
{
    Memory<byte> buffer = new byte[1024];

    string data = Guid.NewGuid().ToString();

    [Benchmark(Baseline = true)]
    public void EncodeAndDecode()
    {
        MessageEncoding.Encode(data, buffer);
        MessageEncoding.Decode(buffer.Span);
    }

    [Benchmark]
    public void FastEncodeAndDecode()
    {
        MessageEncoding.FastEncode(data, buffer);
        MessageEncoding.FastDecode(buffer.Span);
    }
}