﻿// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using MockSocket;
using MockSocket.Message;

[MemoryDiagnoser]
public class EncodeBencmarks
{
    Memory<byte> buffer = new byte[1024];
    Memory<byte> buffer2 = new byte[1024];

    string data = Guid.NewGuid().ToString();

    IEncoder encoder = new FastEncoder();

    [Benchmark(Baseline = true)]
    public void EncodeAndDecode()
    {
        MessageEncoding.Encode<string>(data, buffer);
        MessageEncoding.Decode<string>(buffer.Span);
    }

    [Benchmark]
    public void FastEncodeAndDecode()
    {
        MessageEncoding.FastEncode(data, buffer);
        MessageEncoding.FastDecode(buffer.Span);
    }

    [Benchmark]
    public void Encoder_Decoder()
    {
        var span = buffer2.Span;
        encoder.Encode(data, span);
        encoder.Decode<string>(span);
    }
}