using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.State
{
    internal class SendState
    {
        public int Handle { get; set; }
        public Socket WorkSocket{ get; set; }
    }
}
