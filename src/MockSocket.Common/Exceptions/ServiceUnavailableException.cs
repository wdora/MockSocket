using System;
using System.Net;

namespace MockSocket.Common.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(EndPoint serverEP) : base(serverEP.ToString())
    {

    }
}
