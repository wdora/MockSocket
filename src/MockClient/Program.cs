// See https://aka.ms/new-console-template for more information
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

var epStr = args.LastOrDefault();

if (string.IsNullOrWhiteSpace(epStr))
{
    Console.WriteLine("You must specify a host to connect to");
    return;
}

var ep = IPEndPoint.Parse(epStr);

if (IsServerMode(args))
{
    await StartServerAsync(ep);
    return;
}

bool IsServerMode(string[] args)
{
    return args.Any(x => x.Contains('l'));
}

await StartClientAsync(remoteEP: ep);

async Task StartClientAsync(EndPoint remoteEP)
{
    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    await socket.ConnectAsync(remoteEP);

    var buffer = new byte[1024].AsMemory();

    while (true)
    {
        var message = Console.ReadLine();

        var count = Encoding.UTF8.GetBytes(message, buffer.Span);

        await socket.SendAsync(buffer.Slice(0, count));
    }
}

static async Task LoopClient(Socket client)
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

static async Task StartServerAsync(EndPoint ep)
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