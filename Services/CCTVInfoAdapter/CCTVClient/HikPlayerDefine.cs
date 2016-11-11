using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    static class HikPlayerDefine
    {
        public struct PLAYM4_SYSTEM_TIME
        {
            public int dwYear;
            public int dwMon;
            public int dwDay;
            public int dwHour;
            public int dwMin;
            public int dwSec;
            public int dwMs;
        };

#pragma warning disable 0649
        public struct FRAME_INFO
        {
            public int nWidth;
            public int nHeight;
            public int nStamp;
            public int nType;
            public int nFrameRate;
        };
#pragma warning restore 0649

        public delegate void DecCallback(int nPort, IntPtr pBuf, int nSize, ref FRAME_INFO pFrameInfo, int nUser, int nReserved2);
    }
}
