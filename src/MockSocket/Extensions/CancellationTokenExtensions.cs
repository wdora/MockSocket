using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static (CancellationTokenSource cts, CancellationToken cancellationToken) CreateChildToken(this CancellationToken token)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            return (cts, cts.Token);
        }
    }
}
