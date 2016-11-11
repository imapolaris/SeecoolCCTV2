using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CCTVClient
{
    public class HikPlayer : IDisposable
    {
        IHikPlayerInvoker _invoker = null;
        int _port = -1;

        public HikPlayer()
        {
            if (IntPtr.Size == 8)
                _invoker = new HikPlayerInvoker64();
            else
                _invoker = new HikPlayerInvoker32();

            _invoker.GetPort(ref _port);
        }

        public void Dispose()
        {
            if (_port >= 0)
                _invoker.FreePort(_port);
            _port = -1;
        }

        ~HikPlayer()
        {
            Dispose();
        }

        public enum StreamMode { Realtime = 0, File = 1 };
        public bool SetStreamMode(StreamMode mode)
        {
            return _invoker.SetStreamOpenMode(_port, (int)mode);
        }

        public const int SOURCE_BUF_MAX = 1024 * 100000;
        public const int SOURCE_BUF_MIN = 1024 * 50;
        public bool OpenStream(byte[] header, int start, int len, int bufPoolSize)
        {
            IntPtr mem = Marshal.AllocHGlobal(len);
            Marshal.Copy(header, start, mem, len);
            bool ret = _invoker.OpenStream(_port, mem, len, bufPoolSize);
            Marshal.FreeHGlobal(mem);
            return ret;
        }

        public bool CloseStream()
        {
            return _invoker.CloseStream(_port);
        }

        public bool InputData(byte[] data, int start, int len)
        {
            IntPtr mem = Marshal.AllocHGlobal(len);
            Marshal.Copy(data, start, mem, len);
            bool ret = _invoker.InputData(_port, mem, len);
            Marshal.FreeHGlobal(mem);
            return ret;
        }

        public bool Play(IntPtr windowHandle)
        {
            return _invoker.Play(_port, windowHandle);
        }

        public bool Stop()
        {
            return _invoker.Stop(_port);
        }

        public bool Pause(bool pause)
        {
            return _invoker.Pause(_port, pause ? 1 : 0);
        }

        public bool Fast()
        {
            return _invoker.Fast(_port);
        }

        public bool Slow()
        {
            return _invoker.Slow(_port);
        }

        public uint GetPlayedTimeEx()
        {
            return _invoker.GetPlayedTimeEx(_port);
        }

        public DateTime GetSystemTime()
        {
            int time = (int)_invoker.GetSpecialData(_port);
            try
            {
                return new DateTime((((time) >> 26) + 2000), (((time) >> 22) & 15), (((time) >> 17) & 31), (((time) >> 12) & 31), (((time) >> 6) & 63), (((time) >> 0) & 63));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public DateTime GetSystemTimeEx()
        {
            HikPlayerDefine.PLAYM4_SYSTEM_TIME sysTime = new HikPlayerDefine.PLAYM4_SYSTEM_TIME();
            if (_invoker.GetSystemTime(_port, ref sysTime))
            {
                try
                {
                    return new DateTime(sysTime.dwYear, sysTime.dwMon, sysTime.dwDay, sysTime.dwHour, sysTime.dwMin, sysTime.dwSec, sysTime.dwMs);
                }
                catch
                {
                }
            }
            return DateTime.MinValue;
        }

        public enum FrameType { Audio16 = 101, Audio8 = 100, UYVY = 1, YV12 = 3, RGB32 = 7 };
        public delegate void DecFrameCallback(byte[] frame, int width, int height, int stamp, FrameType type, int frameRate);
        HikPlayerDefine.DecCallback _decCallback;
        private event DecFrameCallback _decframeEvent;
        public event DecFrameCallback DecFrameEvent
        {
            add
            {
                bool isEmpty = _decframeEvent == null;
                _decframeEvent += value;
                if (isEmpty)
                {
                    _decCallback = new HikPlayerDefine.DecCallback(onDec);
                    _invoker.SetDecCallBack(_port, _decCallback);
                }
            }
            remove
            {
                _decframeEvent -= value;
                if (_decframeEvent == null)
                    _invoker.SetDecCallBack(_port, null);
            }
        }

        private void onDec(int nPort, IntPtr pBuf, int nSize, ref HikPlayerDefine.FRAME_INFO pFrameInfo, int nUser, int nReserved2)
        {
            DecFrameCallback callback = _decframeEvent;
            if (callback != null)
            {
                byte[] frame = new byte[nSize];
                Marshal.Copy(pBuf, frame, 0, nSize);
                callback(frame, pFrameInfo.nWidth, pFrameInfo.nHeight, pFrameInfo.nStamp, (FrameType)pFrameInfo.nType, pFrameInfo.nFrameRate);
            }
        }
    }
}
