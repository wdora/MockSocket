using MockSocket.Common.Interfaces;

namespace MockSocket.Udp.Commands;

// US2CCreateDataClient -> udp, server to client: create dataClient

public record class US2CCreateDataClient(string UserClientId) : ICmd;

public record class US2CResult(bool IsOk) : ICmd;

public record class UC2SInitCtrlAgent(int Port, int Interval) : ICmd;

public record class UC2SHeartBeat(int Interval) : ICmd;

public record class US2CHeartBeat() : ICmd;

public record class UC2SInitDataClient(string UserClientId) : ICmd;