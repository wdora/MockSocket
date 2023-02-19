namespace MockSocket.Core.Commands
{
    public record class CreateAppServerCmd(int Port, string Type) : ICmd;
}