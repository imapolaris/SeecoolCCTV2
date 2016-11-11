using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVStreamCmd.Hikvision
{
    static class UsntDvrDll
    {
        static bool _isX64 = IntPtr.Size == 8;

        public static int USNTDVR_Init()
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_Init();
            else
                return USNTDvrDll32.USNTDVR_Init();
        }

        public static int USNTDVR_Cleanup()
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_Cleanup();
            else
                return USNTDvrDll32.USNTDVR_Cleanup();
        }

        public static int USNTDVR_Login_V30(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, out NET_DVR_DEVICEINFO_V30 lpDeviceInfo)
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_Login_V30(sDVRIP, wDVRPort, sUserName, sPassword, out lpDeviceInfo);
            else
                return USNTDvrDll32.USNTDVR_Login_V30(sDVRIP, wDVRPort, sUserName, sPassword, out lpDeviceInfo);
        }

        public static int USNTDVR_Logout_V30(int lUserID)
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_Logout_V30(lUserID);
            else
                return USNTDvrDll32.USNTDVR_Logout_V30(lUserID);
        }

        
        public static int USNTDVR_GetLastError()
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_GetLastError();
            else
                return USNTDvrDll32.USNTDVR_GetLastError();
        }

        public static int USNTDVR_RealPlay_V30(int lUserID, ref NET_DVR_CLIENTINFO lpClientInfo, RealDataCallBack_V30 realDataCallBack, IntPtr pUser, int bBlocked)
        {
            if(_isX64)
                return USNTDvrDll64.USNTDVR_RealPlay_V30(lUserID, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
            else
                return USNTDvrDll32.USNTDVR_RealPlay_V30(lUserID, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
        }

        internal static int USNTDVR_StopRealPlay(int playHandle)
        {
            if (_isX64)
                return USNTDvrDll64.USNTDVR_StopRealPlay(playHandle);
            else
                return USNTDvrDll32.USNTDVR_StopRealPlay(playHandle);
        }
    }
}
