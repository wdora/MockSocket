using MockSocket.Common.Interfaces;

namespace MockSocket.Tcp.Commands;
public record class TS2CInitUserClientCmd(string UserClientId) : ICmd;
