using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Interfaces;

public interface IMockServer
{
    ValueTask StartAsync(CancellationToken cancellationToken);

    ValueTask StopAsync(CancellationToken cancellationToken);
}
