// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using MockSocket.Core.Commands;
using MockSocket.Core.Services;

[MemoryDiagnoser]
public class EncodeBencmarks
{
    Memory<byte> buffer = new byte[1024];

    HeartBeatCmd model = new HeartBeatCmd(DateTime.Now.ToLongTimeString());

    [Benchmark(Baseline = true)]
    public void JsonEncode()
    {
        JsonEncodeService.Instance.Encode(model, buffer);
    }

    [Benchmark]
    public void FastJsonDecode()
    {
        new MyJsonEncodeService().Encode(model, buffer);
    }
}