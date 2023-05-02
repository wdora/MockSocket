using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Interfaces
{
    public interface IUdpPairService
    {
        ValueTask PairAsync(IPEndPoint realClient, IPEndPoint dataClient, CancellationToken cancellationToken);

        ValueTask PairAsync((IUdpClient udpClient, IPEndPoint serverEP) remote, (IUdpClient udpClient, IPEndPoint serverEP) local, CancellationToken cancellationToken);
    }
}
