using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.H264
{
    public class PSFragment
    {
        public NaluHeader Header { get; private set; }
        public byte[] Data { get; private set; }
        public bool IsFrameStart { get { return Header != null; } }
        public bool IsFrameEnd { get; set; } = false;

        public PSFragment(NaluHeader header, byte[] data)
        {
            Header = header;
            Data = data;
        }
    }
}
