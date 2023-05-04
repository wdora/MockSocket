using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Tcp.Interfaces;
public interface ITcpPairService
{
    ValueTask PairAsync(ITcpClient agentClient, ITcpClient userClient, CancellationToken cancellationToken);
}
