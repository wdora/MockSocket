namespace MockSocket.Core.Commands
{
    public record class CreateAppServerCmd(int port, string type) : ICmd;
}