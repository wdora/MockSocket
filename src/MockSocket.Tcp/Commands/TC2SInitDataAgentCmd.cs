using MockSocket.Common.Interfaces;

namespace MockSocket.Tcp.Commands;

public record class TC2SInitDataAgentCmd(string UserClientId) : ICmd;
