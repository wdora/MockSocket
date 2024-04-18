using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Commands
{
    public record class HeartBeatCmd(string Message) : ICmd;
}
