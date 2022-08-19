using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Tcp;
using System.Net;
using System.Net.Sockets;

namespace MockSocket.Connection.Tcp
{
    public class TcpConnection : ITcpConnection, IDisposable
    {
        internal readonly Socket socket;

        public TcpConnection()
            : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        public TcpConnection(Socket socket)
        {
            this.socket = socket;

            LingerOption lingerOption = new LingerOption(false, 0);

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);
        }

        public bool IsConnected => socket.IsConnected();

        public virtual void Dispose()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                // todo
            }
            finally
            {
                socket.Close();
                socket.Dispose();
            }
        }

        public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
        }

        public async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await socket.SendAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
        }

        public override string ToString()
        {
            return $"tcp:{socket.LocalEndPoint}-{socket.RemoteEndPoint}";
        }
    }

    public class TcpClientConnection : TcpConnection, ITcpClientConnection
    {
        public ValueTask ConnectAsync(EndPoint remoteEP, CancellationToken cancellationToken = default)
        {
            return socket.ConnectAsync(remoteEP, cancellationToken);
        }
    }

    public class TcpServerConnection : TcpConnection, ITcpServerConnection
    {
        public async ValueTask<ITcpConnection> AcceptAsync(CancellationToken cancellation = default)
        {
            var socket = await base.socket.AcceptAsync(cancellation);

            return new TcpConnection(socket);
        }

        public ValueTask ListenAsync(IPEndPoint localEP)
        {
            socket.Bind(localEP);

            socket.Listen();

            return ValueTask.CompletedTask;
        }
    }
}
