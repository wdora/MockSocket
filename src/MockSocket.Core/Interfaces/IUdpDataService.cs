using System.Net;

namespace MockSocket.Core.Interfaces
{
    public interface IUdpDataService
    {
        ValueTask FromAppServer(IUdpServer appServer, IPEndPoint userEP, Memory<byte> data, CancellationToken cancellationToken = default);

        ValueTask ToAppServer(IPEndPoint dataEP, Memory<byte> data, CancellationToken cancellationToken = default);

        bool IsDataClient(IPEndPoint dataEP);

        ValueTask RegisterDataClient(IPEndPoint dataEP, IPEndPoint userEP);
    }
}
