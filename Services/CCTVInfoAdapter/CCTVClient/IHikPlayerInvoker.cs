using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    interface IHikPlayerInvoker
    {
        bool GetPort(ref int nPort);
        bool FreePort(int nPort);
        bool SetStreamOpenMode(int nPort, int nMode);
        bool OpenStream(int nPort, IntPtr pFileHeadBuf, int nSize, int nBufPoolSize);
        bool CloseStream(int nPort);
        bool InputData(int nPort, IntPtr pBuf, int nSize);
        bool Play(int nPort, IntPtr hWnd);
        bool Stop(int nPort);
        bool Pause(int nPort, int nPause);
        bool Fast(int nPort);
        bool Slow(int nPort);
        bool SetDecCallBack(int nPort, HikPlayerDefine.DecCallback callback);
        uint GetPlayedTimeEx(int nPort);
        uint GetSpecialData(int nPort);
        bool GetSystemTime(int nPort, ref HikPlayerDefine.PLAYM4_SYSTEM_TIME sysTime);
    }
}
