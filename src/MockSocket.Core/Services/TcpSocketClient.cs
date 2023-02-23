using System.Net.Sockets;
using MockSocket.Core.Interfaces;

namespace MockSocket.Core.Services
{
    public abstract class TcpSocketClient : ITcpClient
    {
        protected Socket _socket;

        protected TcpSocketClient(Socket socket)
        {
            _socket = socket;
        }

        ~TcpSocketClient()
        {
            Dispose(false);
        }

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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int _disposed; // 0 == false, anything else == true

        private void Dispose(bool disposing)
        {
            // Make sure we're the first call to Dispose
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
                return;

            _socket?.Dispose();
        }
    }
}