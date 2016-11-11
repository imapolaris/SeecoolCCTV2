using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public interface IStreamPacket
    {
        byte[] Buffer { get; }
        CCTVFrameType FrameType { get; }
        DateTime Time{get;}
    }
}
