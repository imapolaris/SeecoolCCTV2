using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CCTVClient
{
    internal class HikPlayerInvoker32 : IHikPlayerInvoker
    {
        const string _dllPath = @"x86\PlayCtrl.dll";

        [DllImport(_dllPath)]
        private static extern int PlayM4_GetPort(ref int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_FreePort(int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_SetStreamOpenMode(int nPort, int nMode);

        [DllImport(_dllPath)]
        private static extern int PlayM4_OpenStream(int nPort, IntPtr pFileHeadBuf, int nSize, int nBufPoolSize);

        [DllImport(_dllPath)]
        private static extern int PlayM4_CloseStream(int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_InputData(int nPort, IntPtr pBuf, int nSize);

        [DllImport(_dllPath)]
        private static extern int PlayM4_Play(int nPort, IntPtr hWnd);

        [DllImport(_dllPath)]
        private static extern int PlayM4_Stop(int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_Pause(int nPort, int nPause);

        [DllImport(_dllPath)]
        private static extern int PlayM4_SetDecCallBack(int nPort, HikPlayerDefine.DecCallback callback);

        [DllImport(_dllPath)]
        private static extern uint PlayM4_GetPlayedTimeEx(int nPort);

        [DllImport(_dllPath)]
        private static extern uint PlayM4_GetSpecialData(int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_GetSystemTime(int nPort, ref HikPlayerDefine.PLAYM4_SYSTEM_TIME pstSystemTime);
        
        [DllImport(_dllPath)]
        private static extern int PlayM4_Slow(int nPort);

        [DllImport(_dllPath)]
        private static extern int PlayM4_Fast(int nPort);

        public bool SetStreamOpenMode(int nPort, int nMode)
        {
            return PlayM4_SetStreamOpenMode(nPort, nMode) != 0;
        }

        public bool GetPort(ref int nPort)
        {
            return PlayM4_GetPort(ref nPort) != 0;
        }

        public bool FreePort(int nPort)
        {
            return PlayM4_FreePort(nPort) != 0;
        }

        public bool OpenStream(int nPort, IntPtr pFileHeadBuf, int nSize, int nBufPoolSize)
        {
            return PlayM4_OpenStream(nPort, pFileHeadBuf, nSize, nBufPoolSize) != 0;
        }

        public bool CloseStream(int nPort)
        {
            return PlayM4_CloseStream(nPort) != 0;
        }

        public bool InputData(int nPort, IntPtr pBuf, int nSize)
        {
            return PlayM4_InputData(nPort, pBuf, nSize) != 0;
        }

        public bool Play(int nPort, IntPtr hWnd)
        {
            return PlayM4_Play(nPort, hWnd) != 0;
        }

        public bool Stop(int nPort)
        {
            return PlayM4_Stop(nPort) != 0;
        }

        public bool Pause(int nPort, int nPause)
        {
            return PlayM4_Pause(nPort, nPause) != 0;
        }

        public bool Fast(int nPort)
        {
            return PlayM4_Fast(nPort) != 0;
        }
        public bool Slow(int nPort)
        {
            return PlayM4_Slow(nPort) != 0;
        }

        public bool SetDecCallBack(int nPort, HikPlayerDefine.DecCallback callback)
        {
            return PlayM4_SetDecCallBack(nPort, callback) != 0;
        }

        public uint GetPlayedTimeEx(int nPort)
        {
            return PlayM4_GetPlayedTimeEx(nPort);
        }

        public uint GetSpecialData(int nPort)
        {
            return PlayM4_GetSpecialData(nPort);
        }

        public bool GetSystemTime(int nPort, ref HikPlayerDefine.PLAYM4_SYSTEM_TIME sysTime)
        {
            return PlayM4_GetSystemTime(nPort, ref sysTime) != 0;
        }
    }
}
