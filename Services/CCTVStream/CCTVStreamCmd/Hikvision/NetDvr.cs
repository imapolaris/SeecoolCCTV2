using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVStreamCmd.Hikvision
{
    public class NetDvr : IDisposable
    {
        private class Engine
        {
            protected Engine()
            {
                NetDvrDll.NET_DVR_Init();
                UsntDvrDll.USNTDVR_Init();
            }

            ~Engine()
            {
                NetDvrDll.NET_DVR_Cleanup();
                UsntDvrDll.USNTDVR_Cleanup();
            }

            public void Init()
            {
            }

            public static readonly Engine Instance = new Engine();
        }

        public int Handle = -1;
        public bool IsHik = true;
        internal NET_DVR_DEVICEINFO_V30 DeviceInfo = new NET_DVR_DEVICEINFO_V30();

        public NetDvr()
        {
            Engine.Instance.Init();
        }

        public bool Login(string ip, int port, string user, string pass)
        {
            Logout();

            Handle = NetDvrDll.NET_DVR_Login_V30(ip, (ushort)port, user, pass, out DeviceInfo);
            if (Handle >= 0)
            {
                IsHik = true;
                Console.WriteLine("Handle : " + Handle);
                return true;
            }
            else
            {
                Handle = UsntDvrDll.USNTDVR_Login_V30(ip, (ushort)port, user, pass, out DeviceInfo);
                if (Handle >= 0)
                {
                    IsHik = false;
                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            if (Handle >= 0)
            {
                if (IsHik)
                    NetDvrDll.NET_DVR_Logout_V30(Handle);
                else
                    UsntDvrDll.USNTDVR_Logout_V30(Handle);
                Console.WriteLine("Logout Handle: " + Handle);
            }
            Handle = -1;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Logout();
        }

        #endregion
        public class Serial : IDisposable
        {
            public NetDvr Dvr;

            public Serial(string hikHost, int port = 8000, string user = "admin", string pass = "12345")
            {
                var dvr = new NetDvr();
                if (dvr.Login(hikHost, port, user, pass))
                {
                    Dvr = dvr;
                }
                else
                    throw new CanNotLoginException("设备登录失败！");
            }

            internal int RealPlay_V30(ref NET_DVR_CLIENTINFO lpClientInfo, RealDataCallBack_V30 realDataCallBack, IntPtr pUser, int bBlocked)
            {
                if(Dvr.IsHik)
                    return NetDvrDll.NET_DVR_RealPlay_V30(Dvr.Handle, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
                else
                    return UsntDvrDll.USNTDVR_RealPlay_V30(Dvr.Handle, ref lpClientInfo, realDataCallBack, pUser, bBlocked);
            }

            internal void StopRealPlay(int playHandle)
            {
                if(Dvr.IsHik)
                    NetDvrDll.NET_DVR_StopRealPlay(playHandle);
                else
                    UsntDvrDll.USNTDVR_StopRealPlay(playHandle);
            }


            internal void GetError()
            {
                if (Dvr.Handle < 0)
                    throw new CanNotLoginException("设备登陆失败！");
                if (Dvr.IsHik)
                {
                    int errorNo = NetDvrDll.NET_DVR_GetLastError();
                    if(errorNo != 0)
                        throw new InvalidOperationException("Hik Error NO." + errorNo);
                }
                else
                {
                    int errorNo = UsntDvrDll.USNTDVR_GetLastError();
                    if (errorNo != 0)
                        throw new InvalidOperationException("Usnt Error NO." + errorNo);
                }
            }

            public void Dispose()
            {
                Dvr.Dispose();
            }
        }
        }
}
