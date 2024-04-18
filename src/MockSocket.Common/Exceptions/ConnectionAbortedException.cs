using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Common.Exceptions;
public class ConnectionAbortedException : Exception
{
    public ConnectionAbortedException(string id) : base($"The connection ({id}) was aborted.")
    {
    }
}
