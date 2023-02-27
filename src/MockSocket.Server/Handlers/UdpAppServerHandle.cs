using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MockSocket.Core.Commands;
using System.Net;

namespace MockSocket.Server.Handlers
{
    public class UdpAppServerHandle : IRequestHandler<CreateUdpAppServerCmd>
    {
        IMockUdpServer mockUdpServer;

        IMemoryCache memoryCache;

        public UdpAppServerHandle(IMockUdpServer mockUdpServer, IMemoryCache memoryCache)
        {
            this.mockUdpServer = mockUdpServer;
            this.memoryCache = memoryCache;
        }

        public async Task Handle(CreateUdpAppServerCmd request, CancellationToken cancellationToken)
        {
            mockUdpServer.Listen(request.Port);

            await CurrentContext.Agent.SendCmdAsync(true);

            _ = LoopServerAsync(cancellationToken);

            throw new NotImplementedException();
        }

        private async ValueTask LoopServerAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024].AsMemory();

            while (true)
            {
                var realLength = await mockUdpServer.ReceiveFromAsync(buffer, out var endPoint, cancellationToken);

                if (realLength == 0)
                    continue;

                var userClientId = endPoint.ToString();

                // send agent message that new client is comming
                var queue = await memoryCache.GetOrCreateAsync(userClientId, async entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(30));

                    await CurrentContext.Agent.SendCmdAsync(new UdpDataClientCmd(userClientId));

                    return new Queue<byte[]>();
                });

                queue!.Enqueue(buffer.Slice(0, realLength).ToArray());
            }
        }
    }

    public interface IMockUdpServer
    {
        void Listen(int port);

        ValueTask<int> ReceiveFromAsync(Memory<byte> buffer, out IPEndPoint endPoint, CancellationToken cancellationToken);
    }
}
