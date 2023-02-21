using System.Net.Sockets;

namespace MockSocket.Core.Tcp
{
    public abstract class TcpSocketClient : ITcpClient
    {
        protected Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public ValueTask DisconnectAsync()
        {
            return _socket.DisconnectAsync(false);
        }

        public ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            return _socket.ConnectAsync(host, port, cancellationToken);
        }

        public ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default)
        {
            return _socket.ReceiveAsync(data, cancellationToken);
        }

        public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            await _socket.SendAsync(data, cancellationToken);

            return;
        }

        public void Dispose()
        {
            try
            {
                // https://github.com/dotnet/runtime/issues/26957
                // Disposing socket on Linux does not interrupt poll/select
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            finally
            {
                _socket.Dispose();
            }
        }
    }
}