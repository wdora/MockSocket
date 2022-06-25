using Microsoft.Extensions.Logging;
using MockSocket.Core.Tcp;
using System.IO.Pipelines;

namespace MockSocket.Core.Exchange
{
    /// <summary>
    /// Pipe
    /// GC: pipepool
    /// </summary>
    public class PipeExchangeConnection : ExchangeConnection
    {
        public PipeExchangeConnection(ILogger<PipeExchangeConnection> logger) : base(logger)
        {
        }

        public override async Task SwapMessageAsync(ITcpConnection send, ITcpConnection receive, CancellationToken cancellationToken)
        {
            var pipe = new Pipe();

            cancellationToken.Register(() =>
            {
                pipe.Reader.Complete();
                pipe.Writer.Complete();
                pipe.Reset();
            });

            var reader = async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var buffer = pipe.Writer.GetMemory();

                    var realSize = await receive.ReceiveAsync(buffer, cancellationToken);

                    if (realSize == 0)
                        return;

                    pipe.Writer.Advance(realSize);

                    await pipe.Writer.FlushAsync(cancellationToken);
                }
            };

            var writer = async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await pipe.Reader.ReadAsync(cancellationToken);

                    var buffer = result.Buffer;

                    foreach (var segment in buffer)
                        await send.SendAsync(segment, cancellationToken);

                    pipe.Reader.AdvanceTo(buffer.Start, buffer.End);
                }
            };

            await Task.WhenAny(reader(), writer());
        }
    }
}
