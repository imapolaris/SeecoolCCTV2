using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public class SendEventArgs : EventArgs
    {
        private int _sendHandle;
        public SendEventArgs(int handle)
        {
            _sendHandle = handle;
        }

        public int SendHandle { get { return _sendHandle; } }
    }
}
