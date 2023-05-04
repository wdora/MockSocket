﻿namespace MockSocket.Core.Interfaces
{

    public interface ITcpClient : ITcpConnection, IDisposable
    {
        ValueTask DisconnectAsync();

        ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken = default);
    }
}