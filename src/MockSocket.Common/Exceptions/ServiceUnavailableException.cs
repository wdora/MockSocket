using System;
using System.Net;

namespace MockSocket.Common.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(IPEndPoint serverEP) : base(serverEP.ToString())
    {

    }
}
