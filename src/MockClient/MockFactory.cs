// See https://aka.ms/new-console-template for more information
using System.Net;

public class MockFactory
{
    internal ValueTask StartAsync(IPEndPoint ep, bool isServerMode)
    {
        IStartup mock = isServerMode ? new MockServer(ep) : new MockClient(ep);

        return mock.StartAsync();
    }

    internal ValueTask StartAsync(string[] args)
    {
        var epStr = args.LastOrDefault();

        if (string.IsNullOrWhiteSpace(epStr))
        {
            Console.WriteLine("You must specify a host to connect to");
            return default;
        }

        var ep = IPEndPoint.Parse(epStr);

        return StartAsync(ep, IsServerMode(args));
    }

    bool IsServerMode(string[] args)
    {
        return args.Any(x => x.StartsWith('-') && x.Contains('l'));
    }
}