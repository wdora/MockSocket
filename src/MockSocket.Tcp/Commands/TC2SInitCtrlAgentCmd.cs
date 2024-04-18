using MockSocket.Common.Interfaces;

namespace MockSocket.Tcp.Commands;
public record class TC2SInitCtrlAgentCmd(int Port) : ICmd;