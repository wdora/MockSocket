using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.xUnit
{
    public class UdpServerTests
    {
        [Theory]
        [InlineData("10.129.3.97", 10000)]
        public async Task Listen(string ip, int port)
        {
            var udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpServer.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            // 绑定相同的端口
            // System.Net.Sockets.SocketException : 提供了一个无效的参数。
            //udpServer.Bind(new IPEndPoint(IPAddress.Parse(ip), port)); 

            // udp不需要listen操作，bind即占用并监听端口
            // System.Net.Sockets.SocketException : 参考的对象类型不支持尝试的操作。
            //udpServer.Listen();

            // udp 直接receive package，package中有endpoint
            // System.InvalidOperationException : You must call the Listen method before performing this operation.
            //await udpServer.AcceptAsync();

            // 单个udp包最大 65507
            // 发送端，不能超过，需要应用层自己拆包
            // 接收端，不能用小于实际包大小的buffer去循环接收
            var buffer = new byte[10000].AsMemory();
            EndPoint clientSo = new IPEndPoint(IPAddress.Loopback, 61597);

            // buffer 小于 实际包大小会抛出异常
            // System.Net.Sockets.SocketException : 一个在数据报套接字上发送的消息大于内部消息缓冲区或其他一些网络限制，或该用户用于接收数据报的缓冲区比数据报小。
            var result = await udpServer.ReceiveFromAsync(buffer, SocketFlags.None, clientSo);
        }
    }
}
