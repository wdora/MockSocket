using System.Net;

namespace MockSocket.Core.Extensions
{
    public static class IPEndPointExtension
    {
        public static string Id(this IPEndPoint ep)
        {
            return ep.ToString();
        }
    }
}