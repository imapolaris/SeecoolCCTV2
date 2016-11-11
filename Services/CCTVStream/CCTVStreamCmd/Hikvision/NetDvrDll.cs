using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CCTVStreamCmd.Hikvision
{
    static class NetDvrDll
    {
        static bool _isX64 = IntPtr.Size == 8;

        public static int NET_DVR_Init()
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_Init();
            else
                return NetDvrDll32.NET_DVR_Init();
        }

        public static int NET_DVR_Cleanup()
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_Cleanup();
            else
                return NetDvrDll32.NET_DVR_Cleanup();
        }

        public static int NET_DVR_Login_V30(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, out NET_DVR_DEVICEINFO_V30 lpDeviceInfo)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_Login_V30(sDVRIP, wDVRPort, sUserName, sPassword, out lpDeviceInfo);
            else
                return NetDvrDll32.NET_DVR_Login_V30(sDVRIP, wDVRPort, sUserName, sPassword, out lpDeviceInfo);
        }

        public static int NET_DVR_Logout_V30(int lUserID)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_Logout_V30(lUserID);
            else
                return NetDvrDll32.NET_DVR_Logout_V30(lUserID);
        }

        public static int NET_DVR_RealPlay_V30(int lUserID, ref NET_DVR_CLIENTINFO lpClientInfo, RealDataCallBack_V30 realDataCallBack, IntPtr pUser, int bBlocked)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_RealPlay_V30(lUserID, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
            else
                return NetDvrDll32.NET_DVR_RealPlay_V30(lUserID, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
        }

        public static int NET_DVR_StopRealPlay(int nPlayHandle)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_StopRealPlay(nPlayHandle);
            else
                return NetDvrDll32.NET_DVR_StopRealPlay(nPlayHandle);
        }

        #region Serial Info

        public delegate void SerialDataCallBack(int lSerialHandle, IntPtr pRecvDataBuffer, uint dwBufSize, IntPtr dwUser);

        public static int NET_DVR_SerialStart(int lUserID, int lSerialPort, SerialDataCallBack fSerialDataCallBack, IntPtr dwUser)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_SerialStart(lUserID, lSerialPort, fSerialDataCallBack, dwUser);
            else
                return NetDvrDll32.NET_DVR_SerialStart(lUserID, lSerialPort, fSerialDataCallBack, dwUser);
        }

        public static int NET_DVR_SerialSend(int lSerialHandle, int lChannel, IntPtr pSendBuf, uint dwBufSize)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_SerialSend(lSerialHandle, lChannel, pSendBuf, dwBufSize);
            else
                return NetDvrDll32.NET_DVR_SerialSend(lSerialHandle, lChannel, pSendBuf, dwBufSize);
        }

        public static int NET_DVR_SerialStop(int lSerialHandle)
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_SerialStop(lSerialHandle);
            else
                return NetDvrDll32.NET_DVR_SerialStop(lSerialHandle);
        }

        public static int NET_DVR_GetLastError()
        {
            if (_isX64)
                return NetDvrDll64.NET_DVR_GetLastError();
            else
                return NetDvrDll32.NET_DVR_GetLastError();
        }

        #endregion Serial Info
    }
}
