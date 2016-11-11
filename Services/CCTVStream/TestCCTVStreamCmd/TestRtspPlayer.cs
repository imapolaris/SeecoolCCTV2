using CCTVStreamCmd.Rtsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seecool.VideoStreamBase;
using FFmpeg;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TestCCTVStreamCmd
{
    public class TestRtspPlayer: IDisposable
    {
        RtspStream _rtsp;
        VideoDecoder _decoder;
        object _objLock = new object();
        public Action<Image> BitmapEvent;
        public TestRtspPlayer()
        {
            _rtsp = new RtspStream("rtsp://admin:admin12345@192.168.9.155/h264/ch1/main/av_stream");
            //_rtsp = new RtspStream(@"rtsp://root:pass@192.168.9.156/axis-media/media.amp?streamprofile=Quality");
            _rtsp.HeaderEvent += onHeader;
            _rtsp.StreamEvent += onStreamEvent;
            if (_rtsp.Header != null)
                onHeader(_rtsp.Header);
        }

        private void onHeader(IHeaderPacket packet)
        {
            var header = packet as StandardHeaderPacket;
            Console.Write("header {0}:", header.Buffer.Length);
            for (int i = 0; i < header.Buffer.Length; i++)
                Console.Write("{0:X2}, ", header.Buffer[i]);
            Console.WriteLine();

            _decoder = new VideoDecoder();
            _decoder.Create((Constants.AVCodecID)header.CodecID);
            int width = 0;
            int height = 0;
            _decoder.Decode(header.Buffer, out width, out height);
        }

        private void onStreamEvent(IStreamPacket packet)
        {
            Console.WriteLine();
            Console.Write("packet {0}:", packet.Buffer.Length);
            for (int i = 0; i < 30; i++)
                Console.Write("{0:X2}, ", packet.Buffer[i]);
            Console.WriteLine();

            int width = 0;
            int height = 0;
            byte[] frameData = _decoder.Decode(packet.Buffer, out width, out height);
            if (frameData != null)
            {
                Console.WriteLine("Decode frame: {0}", frameData.Length);

                int size = width * 4 * height;
                byte[] rgb = new byte[size];
                for (int i = 0; i < width * height; i++)
                {
                    rgb[i * 4] = rgb[i * 4 + 1] = rgb[i * 4 + 2] = frameData[i];
                    rgb[i * 4 + 3] = 0xFF;
                }
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(rgb, 0, ptr, size);
                using (Bitmap bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, ptr))
                {
                    onBitmap(bmp);
                }
                Marshal.FreeHGlobal(ptr);
            }
        }

        void onBitmap(Bitmap bp)
        {
            var handler = BitmapEvent;
            if (handler != null)
                handler(bp);
        }

        public void Dispose()
        {
            _rtsp?.Dispose();
            _rtsp = null;
        }

        ~TestRtspPlayer()
        {
            Dispose();
        }
    }
}
