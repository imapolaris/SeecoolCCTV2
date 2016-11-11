using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public enum ErrorTypes
    {
        Receive,
        Send,
        SocketConnect,
        SocketAccept,
        Other
    }
}
