using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockSocket.Core.Extensions;
using MockSocket.Core.Interfaces;
using MockSocket.Server;
using System.Collections.Concurrent;
using System.Net;

namespace MockSocket.Core.Services
{
    public class UdpPairService : IUdpPairService
    {
        IMemoryCache memoryCache;
        ILogger logger;

        public UdpPairService(IMemoryCache memoryCache, ILogger<UdpPairService> logger)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        public async ValueTask PairAsync(IPEndPoint realClient, IPEndPoint dataClient, CancellationToken cancellationToken)
        {
            logger.LogInformation($"udp 交换连接开始：{realClient.Id} <=> {dataClient.Id}");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                using var task1 = SwapAsync(realClient, dataClient, cts.Token);

                using var task2 = SwapAsync(dataClient, realClient, cts.Token);

                var anyTask = await Task.WhenAny(task1, task2);

                cts.Cancel();

                await Task.WhenAll(task1, task2);
            }
            catch
            {

            }
            finally
            {
                logger.LogInformation($"udp 交换连接结束：{realClient.Id} <=> {dataClient.Id}");
            }
        }

        public async ValueTask PairAsync((IUdpClient udpClient, IPEndPoint serverEP) remote, (IUdpClient udpClient, IPEndPoint serverEP) local, CancellationToken cancellationToken)
        {
            logger.LogInformation($"udp 交换连接开始：{remote.serverEP} <=> {local.serverEP}");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                var remoteTask = ExchangeAsync(remote, local, cancellationToken);

                var localTask = ExchangeAsync(local, remote, cancellationToken);

                var anyTask = await Task.WhenAny(remoteTask, localTask);

                cts.Cancel();

                await Task.WhenAll(remoteTask, localTask);
            }
            catch
            {

            }
            finally
            {
                logger.LogInformation($"udp 交换连接结束：{remote.serverEP} <=> {local.serverEP}");
            }
        }

        private Task ExchangeAsync((IUdpClient udpClient, IPEndPoint serverEP) remote, (IUdpClient udpClient, IPEndPoint serverEP) local, CancellationToken cancellationToken)
        {
            return BufferPool
                .Instance
                .Run(async memory =>
                {
                    while (true)
                    {
                        var result = await remote.udpClient.ReceiveFromAsync(memory, remote.serverEP, cancellationToken);

                        await local.udpClient.SendToAsync(memory.Slice(0, result.ReceivedBytes), local.serverEP, cancellationToken);
                    }
                }, BufferPool.UDP_BUFFER_SIZE)
                .AsTask();
        }

        async Task SwapAsync(IPEndPoint userClientEP, IPEndPoint dataClientEP, CancellationToken cancellationToken)
        {
            var userClientId = userClientEP.Id();

            while (true)
            {
                var queue = memoryCache.Get<ConcurrentQueue<byte[]>>(userClientId);

                if (queue == null)
                {
                    logger.LogDebug($"{userClientId} 已离线");
                    return;
                }

                while (queue.TryDequeue(out var bytes))
                    await CurrentContext.UdpMockServer.SendToAsync(bytes, dataClientEP, cancellationToken);
            }
        }
    }
}