using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Commands
{
    // US2CCreateDataClient -> udp, server to client: create dataClient

    public record class US2CCreateDataClient(string UserClientId) : ICmd;

    public record class US2CResult(bool IsOk) : ICmd;

    public record class UC2SInitCtrlAgent(int Port) : ICmd;

    public record class UC2SCtrlAgentHeartBeat() : ICmd;

    public record class UC2SInitDataClient(string UserClientId) : ICmd;
}
