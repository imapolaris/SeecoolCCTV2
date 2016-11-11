using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVStreamCmd.Hikvision.HikFrameProcess
{
    public class HikStreamInfo
    {
        public DateTime Time { get; private set; }
        public byte[] Buffer { get; private set; }
        public HikStreamInfo(DateTime time, byte[] buffer)
        {
            Time = time;
            Buffer = buffer;
        }
    }
}
