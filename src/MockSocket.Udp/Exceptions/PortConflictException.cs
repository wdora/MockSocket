using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Udp.Exceptions;
public class PortConflictException : Exception
{
    public PortConflictException(int appPort) : base(appPort.ToString())
    {
    }
}
