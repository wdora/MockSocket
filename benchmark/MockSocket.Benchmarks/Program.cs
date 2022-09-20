// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MockSocket.Message;

BenchmarkRunner.Run<ReflectionBenchmarks>();

[MemoryDiagnoser]
public class ReflectionBenchmarks
{
    private readonly string jsonStr = "{\"UserClientId\":\"25efffe6-bd2f-4eb3-9cd4-010de508afc0\",\"Connection\":null,\"MessageType\":\"MockSocket.Message.Tcp.DataAgent_HoleServer_Init_Message\"}";

    [Benchmark]
    public TcpBaseMessage CreateMessage() => BaseMessage.CreateMessage<TcpBaseMessage>(jsonStr);

    [Benchmark]
    public TcpBaseMessage CreateMessageByDict() => BaseMessage.CreateMessageByDict<TcpBaseMessage>(jsonStr);
}