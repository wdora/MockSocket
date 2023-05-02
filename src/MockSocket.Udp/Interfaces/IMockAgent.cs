using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MockClient.Udp.Interfaces;

public interface IMockAgent
{
    ValueTask StartAsync(CancellationToken cancellationToken);
}
