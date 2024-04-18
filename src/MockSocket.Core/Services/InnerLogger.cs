using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Core.Services
{
    public class InnerLogger
    {
        public static void Log(string message, Exception? exception = default)
        {
            File.AppendAllText("logs\\inner.txt", $"{DateTime.Now.ToLongTimeString()}{message} {exception}\r\n");
        }
    }
}
