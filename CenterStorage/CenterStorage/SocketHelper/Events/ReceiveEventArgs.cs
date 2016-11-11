using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public class ReceiveEventArgs : EventArgs
    {
        private byte[] _buffer;

        public ReceiveEventArgs(byte[] bytes)
        {
            _buffer = bytes;
        }

        public byte[] ReceivedBytes { get { return _buffer; } }

        public int ByteLength { get { return _buffer == null ? 0 : _buffer.Length; } }
    }
}
