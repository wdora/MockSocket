// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

[MemoryDiagnoser]
public class SerializerBencmarks
{
    private object obj = new DemoUser(1, 3, "Jack");

    [Benchmark]
    public string SerilizeByNewtonsoft()
    {
        return JsonConvert.SerializeObject(obj);
    }

    [Benchmark]
    public string SerilizeBySystemJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }
}

record DemoUser(int Id, int Age, string Name);