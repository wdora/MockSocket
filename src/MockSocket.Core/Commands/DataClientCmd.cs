namespace MockSocket.Core.Commands
{
    public record class DataClientCmd(string userClientId) : ICmd;
}