using MockClient.Udp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MockSocket.Udp.Interfaces;
public interface IUdpPairService
{
    Task PairAsync(IUdpClient dataClient, IUdpClient realClient, CancellationToken cancellationToken);
}
