// See https://aka.ms/new-console-template for more information
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MockServer : IStartup
{
    private EndPoint ep;

    public MockServer(EndPoint ep)
    {
        this.ep = ep;
    }

    public async ValueTask StartAsync()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.Bind(ep);

        socket.Listen(10);

        Console.WriteLine($"Listening on {ep}");

        while (true)
        {
            var client = await socket.AcceptAsync();

            Console.WriteLine($"Connect from {client.RemoteEndPoint}");

            _ = LoopClient(client);
        }
    }

    async Task LoopClient(Socket client)
    {
        var bytes = ArrayPool<byte>.Shared.Rent(1024);

        var memory = bytes.AsMemory();

        while (true)
        {
            try
            {
                var dataLen = await client.ReceiveAsync(memory);

                if (dataLen == 0)
                {
                    Console.WriteLine($"Disconnect from {client.RemoteEndPoint}");
                    break;
                }

                Console.Write(Encoding.UTF8.GetString(memory.Slice(0, dataLen).Span));
            }
            catch (Exception)
            {
                Console.WriteLine($"Abort from {client.RemoteEndPoint}");
                break;
            }
        }

        ArrayPool<byte>.Shared.Return(bytes);
    }
}