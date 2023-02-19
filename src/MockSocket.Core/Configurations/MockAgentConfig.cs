﻿namespace MockSocket.Core.Configurations
{
    public class MockAgentConfig
    {
        public ServerEndpoint RemoteServer { get; set; } = new("wdora.com", 12312);

        public ServerEndpoint RealServer { get; set; } = new("localhost", 80);

        public AppServer AppServer { get; set; } = new(3390, "tcp");

        public int HeartInterval { get; set; } = 30;

        public int RetryInterval { get; set; } = 3;
    }

    public record ServerEndpoint(string Host, int Port);

    public record AppServer(int Port, string Protocal)
    {
        public override string ToString()
        {
            // net.tcp://0.0.0.0:8080
            return $"net.{Protocal}://0.0.0.0:{Port}";
        }
    }
}