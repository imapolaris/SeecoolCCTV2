using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CCTVStreamCmd.Hikvision
{
    static class USNTDvrDll64
    {
        const string _dllPath = @"x64\USNT_SDK.dll";

        [DllImport(_dllPath)]
        public static extern int USNTDVR_Init();
        [DllImport(_dllPath)]
        public static extern int USNTDVR_Cleanup();

        [DllImport(_dllPath)]
        public static extern int USNTDVR_Login_V30(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, out NET_DVR_DEVICEINFO_V30 lpDeviceInfo);
        [DllImport(_dllPath)]
        public static extern int USNTDVR_Logout_V30(int lUserID);

        //2016-06-12
        [DllImport(_dllPath)]
        public static extern int USNTDVR_GetLastError();

        //2016-08-05
        [DllImport(_dllPath)]
        public static extern int USNTDVR_RealPlay_V30(int lUserID, ref NET_DVR_CLIENTINFO lpClientInfo, RealDataCallBack_V30 realDataCallBack, IntPtr pUser, int bBlocked);

        [DllImport(_dllPath)]
        public static extern int USNTDVR_StopRealPlay(int playHandle);
    }
}
