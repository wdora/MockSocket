using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Interfaces
{
    public interface IMockTcpClient : ITcpClient, IDisposable
    {
        string Id { get; }

        ValueTask SendCmdAsync<T>(T model, CancellationToken cancellationToken = default);

        ValueTask<T> ReceiveCmdAsync<T>(CancellationToken cancellationToken = default);
        
        CancellationToken Register(CancellationToken cancellationToken);
    }
}
