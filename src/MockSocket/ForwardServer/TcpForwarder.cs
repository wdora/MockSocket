using MockSocket.Core.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.ForwardServer
{
    public class TcpForwarder
    {
        public async ValueTask ForwardAsync(ITcpConnection srcConn, ITcpConnection dstConn)
        {
            var buffer = new Memory<byte>(new byte[1024]);

            var len = await srcConn.ReceiveAsync(buffer, default);

            var sendBuffer = buffer.Slice(0, len);

            await dstConn.SendAsync(sendBuffer, default);
        }
    }
}
