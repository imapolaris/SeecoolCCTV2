using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CCTVClient
{
    public class ImageGrabber : IDisposable
    {
        CCTVInfo _info;
        bool _ownInfo = true;

        ManualResetEventSlim _infoConnectedEvent = new ManualResetEventSlim(false);

        public int Bandwidth = 2000000;

        public ImageGrabber(string host)
        {
            _ownInfo = true;
            _info = new CCTVInfo(host);
            _info.ConnectedEvent += _info_ConnectedEvent;
            _info.DisconnectedEvent += _info_DisconnectedEvent;
            _info.AuthenticationEvent += _info_AuthenticationEvent;
            _info.Start();
        }

        public ImageGrabber(CCTVInfo info)
        {
            _info = info;
            _ownInfo = false;
            _info.ConnectedEvent += _info_ConnectedEvent;
            _info.DisconnectedEvent += _info_DisconnectedEvent;
            _info.AuthenticationEvent += _info_AuthenticationEvent;
        }

        private void _info_AuthenticationEvent(bool success)
        {
            if (success)
                _infoConnectedEvent.Set();
        }

        private void _info_DisconnectedEvent()
        {
            _infoConnectedEvent.Reset();
        }

        private void _info_ConnectedEvent()
        {
        }

        private int getWaitTime(DateTime expireTime)
        {
            if (expireTime == DateTime.MaxValue)
                return -1;
            else
            {
                int waitTime = (int)(expireTime - DateTime.Now).TotalMilliseconds;
                return Math.Max(0, waitTime);
            }
        }

        public Image GrabImage(ulong videoId, TimeSpan timeout)
        {
            DateTime expireTime = DateTime.MaxValue;
            if (timeout != TimeSpan.MaxValue)
                expireTime = DateTime.Now + timeout;

            if (_infoConnectedEvent.Wait(getWaitTime(expireTime)))
            {
                CCTVVideo video = new CCTVVideo(_info, videoId, Bandwidth);
                using (ManualResetEventSlim frameEvent = new ManualResetEventSlim(false))
                {
                    Image result = null;
                    video.VideoFrameEvent += (int width, int height, byte[] data, int timeStamp) =>
                    {
                        result = GetImageFromYv12Data(width, height, data);
                        frameEvent.Set();
                    };
                    video.Start();
                    frameEvent.Wait(getWaitTime(expireTime));
                    video.Stop();
                    return result;
                }
            }

            return null;
        }

        public static Image GetImageFromYv12Data(int width, int height, byte[] data)
        {
            byte[] rgb32 = YV12ToRGB.Convert(width, height, data);
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = width * 4;
            int src = 0;
            IntPtr dest = bmpData.Scan0;
            for (int i = 0; i < height; i++)
            {
                Marshal.Copy(rgb32, src, dest, stride);
                src += stride;
                dest += bmpData.Stride;
            }
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        public void Dispose()
        {
            if (_ownInfo)
                _info.Stop();
            _info.ConnectedEvent -= _info_ConnectedEvent;
            _info.DisconnectedEvent -= _info_DisconnectedEvent;
            _info.AuthenticationEvent -= _info_AuthenticationEvent;

            _infoConnectedEvent.Reset();
        }
    }
}
