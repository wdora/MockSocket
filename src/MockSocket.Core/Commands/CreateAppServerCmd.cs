namespace MockSocket.Core.Commands
{
    public record class CreateAppServerCmd(int Port, string Type) : ICmd;

    public record class CreateUdpAppServerCmd(int Port) : ICmd;
    
    public record class UdpDataClientCmd(string UserClientId) : ICmd;
}