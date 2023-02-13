using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MockSocket.Abstractions.Tcp;
using MockSocket.Abstractions.Udp;
using MockSocket.Cache;
using MockSocket.Connection.Udp;
using MockSocket.HoleClient;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Message.Udp
{
    public class UdpCtrlAgentInitMessage : TcpBaseMessage, IRequest
    {
        public int AppServerPort { get; set; }
    }

    public class UdpCtrlAgentNewClientMessage : TcpBaseMessage, IRequest
    {
        public string UserClientId { get; set; } = default!;
    }

    class UdpCtrlAgentInitMessageHandle : IRequestHandler<UdpCtrlAgentInitMessage>
    {
        private readonly IMemoryCache memoryCache;

        public UdpCtrlAgentInitMessageHandle(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public async Task<Unit> Handle(UdpCtrlAgentInitMessage request, CancellationToken cancellationToken)
        {
            var port = request.AppServerPort;

            var udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpServer.Bind(new IPEndPoint(IPAddress.Any, port));

            // todo
            // 1. 端口重复 或者 端口号不正确
            //request.Connection.SendAsync()

            var buffer = new byte[4096].AsMemory();
            EndPoint clientSo = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var result = await udpServer.ReceiveFromAsync(buffer, SocketFlags.None, clientSo, cancellationToken);

                var userClient = result.RemoteEndPoint;
                var userClientId = $"net.udp://{userClient}";
                var len = result.ReceivedBytes;

                var pendingData = await memoryCache.GetOrCreateAsync(userClientId, async entry =>
                {
                    // 保活时间
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));

                    // 通知 agent 来接客了
                    await request.Connection.SendAsync(new UdpCtrlAgentNewClientMessage { UserClientId = userClientId });

                    return new ConcurrentQueue<byte[]>();
                });

                pendingData.Enqueue(buffer.Slice(0, len).ToArray());
            }
        }
    }

    public class UdpCtrlAgentNewClientMessageHandle : IRequestHandler<UdpCtrlAgentNewClientMessage>
    {
        ClientOptions clientConfig;

        public UdpCtrlAgentNewClientMessageHandle(ClientOptions clientConfig)
        {
            this.clientConfig = clientConfig;
        }

        public async Task<Unit> Handle(UdpCtrlAgentNewClientMessage request, CancellationToken cancellationToken)
        {
            var userClientId = request.UserClientId;

            var dataAgent = new UdpClientConnection();

            var serverEP = clientConfig.HoleServerEP;

            await dataAgent.SendAsync(serverEP, new UdpDataAgentMessage { UserClientId = userClientId });

            return default;
        }
    }

    public class UdpDataAgentMessage : UdpBaseMessage, IRequest
    {
        public string UserClientId { get; set; } = default!;
    }

    public class UdpDataAgentHandle : IRequestHandler<UdpDataAgentMessage>
    {
        IMemoryCache cache;

        public UdpDataAgentHandle(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public async Task<Unit> Handle(UdpDataAgentMessage request, CancellationToken cancellationToken)
        {
            var dataAgent = request.Connection;
            var userClientId = request.UserClientId;

            if (cache.TryGetValue(userClientId, out Queue<byte[]> pendingData))
            {
                while (true)
                {
                    if (!pendingData.TryDequeue(out var data))
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    await dataAgent.SendAsync(request.AgentEP, data);
                }
            }

            return default;
        }
    }
}
