using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.ValueObject
{
    public class ConnectionId
    {
        public ConnectionId(string id)
        {
            var ipArr = id.Split('-');
            LocalEP = IPEndPoint.Parse(ipArr[0]);
            RemoteEP = IPEndPoint.Parse(ipArr[1]);
        }

        public IPEndPoint LocalEP { get; set; }

        public IPEndPoint RemoteEP { get; set; }
    }
}
