using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.State
{
    internal class ReceiveState
    {
        public const int BufferSize = 1024 * 1024;
        public const string CRLF = "\r\n";

        private byte[] _buffer = new byte[BufferSize];
        private byte[] _received = new byte[0];

        public ReceiveState()
        {
            TotalBytes = -1;
        }

        public Socket WorkSocket { get; set; }
        public int TotalBytes { get; set; }
        /// <summary>
        /// 上次未完全处理的字节数组。
        /// </summary>
        public byte[] UnhandledBytes { get; set; }
        public byte[] Buffer { get { return _buffer; } }
        public byte[] Received { get { return _received; } }
        public int ReceivedBytes { get { return _received.Length; } }
        public bool IsNew { get { return TotalBytes < 0; } }
        public bool Completed { get { return (!IsNew) && (ReceivedBytes >= TotalBytes); } }

        public void AppendBytes(byte[] bytes, int startIndex, int len)
        {
            if (TotalBytes < ReceivedBytes + len)
                throw new IndexOutOfRangeException(string.Format("解析数据出错，所需字节数:{0}，实际字节数:{1}", TotalBytes, ReceivedBytes + len));
            byte[] temp = new byte[Received.Length + len];
            Array.Copy(Received, 0, temp, 0, Received.Length);
            Array.Copy(bytes, startIndex, temp, Received.Length, len);
            _received = temp;
        }
    }
}
