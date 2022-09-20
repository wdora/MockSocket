using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Tcp;

namespace MockSocket.Message
{
    public static class TcpConnectionExtensions
    {
        public static async ValueTask<IMessage> GetMessageAsync(this ITcpConnection connection, CancellationToken cancellationToken = default)
        {
            var raw = await connection.GetStringAsync(cancellationToken);

            var message = BaseMessage.CreateMessageByDict<TcpBaseMessage>(raw);

            message.Connection = connection;

            return message;
        }
    }
}
