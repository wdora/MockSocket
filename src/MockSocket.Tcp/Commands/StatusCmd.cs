using MockSocket.Common.Interfaces;

namespace MockSocket.Tcp.Commands;
public record class StatusCmd(bool IsOk) : ICmd;
