using System.Net.Sockets;

namespace MockSocket.Tcp.Extensions;
public static class SocketExtensions
{
    private const int MaxWaitMicroSeconds = 1000;

    /// <summary>
    /// Socket 自带的Connected 属性只是表示最后一次 socket 的状态，并非真实的状态
    /// </summary>
    /// <param name="so"></param>
    /// <returns></returns>
    public static bool IsConnected(this Socket so) => !(so.Poll(MaxWaitMicroSeconds, SelectMode.SelectRead) && so.Available == 0);
}
