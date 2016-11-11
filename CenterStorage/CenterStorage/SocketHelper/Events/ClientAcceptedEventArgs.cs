using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public class ClientAcceptedEventArgs : EventArgs
    {
        private SocketAdapter _adapter;

        public ClientAcceptedEventArgs(SocketAdapter adapter)
        {
            _adapter = adapter;
        }

        public SocketAdapter Adapter { get { return _adapter; } }
    }
}
