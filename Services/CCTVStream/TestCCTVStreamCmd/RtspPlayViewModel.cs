using AopUtil.WpfBinding;
using CCTVStreamCmd.Rtsp;
using Common.Util;
using FFmpeg;
using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoRender;
using Media.Rtp;

namespace TestCCTVStreamCmd
{
    public class RtspPlayViewModel : ObservableObject, IDisposable
    {
        RtspStream _rtsp;
        private VideoDecoder _decoder;
        IRenderSource _renderSource;

        [AutoNotify]
        public ImageSource ImageSrc { get; set; }

        int _width;
        int _height;
        StreamRtspServer _rtspServer;

        public RtspPlayViewModel()
        {
            _rtspServer = new StreamRtspServer();
            _renderSource = new D3DImageSource();
            _renderSource.ImageSourceChanged += () => updateImageSource(_renderSource.ImageSource);

            //_rtsp = new RtspStream("rtsp://127.0.0.1/live/stream");
            //_rtsp = new RtspStream("rtsp://admin:12345@192.168.9.98/h264/ch1/main/av_stream");
            //_rtsp = new RtspStream("rtsp://admin:admin12345@192.168.9.155/h264/ch1/main/av_stream");
            _rtsp = new RtspStream(@"rtsp://root:pass@192.168.9.155/axis-media/media.amp?streamprofile=Quality");

            _rtsp.HeaderEvent += onHeader;
            _rtsp.StreamEvent += onStreamEvent;
            _rtsp.RtpFrameEvent += onRtpFrame;
            if (_rtsp.Header != null)
                onHeader(_rtsp.Header);
            //_writer = new FileWriter(@"d:\rtsp.txt");
        }

        private void onRtpFrame(RtpFrame frame, CCTVFrameType frameType)
        {
            _rtspServer?.UpdateRtpFrame(frame);
        }

        private void onHeader(IHeaderPacket packet)
        {
            var header = packet as StandardHeaderPacket;
            _rtspServer.UpdateHeader(header.Buffer);

            Console.WriteLine();
            string rtspstr = $"rtsp header {header.Buffer.Length}:";
            for (int i = 0; i < header.Buffer.Length; i++)
                rtspstr += string.Format("{0:X2}, ", header.Buffer[i]);
            Console.WriteLine(rtspstr);

            _decoder = new VideoDecoder();
            _decoder.Create((Constants.AVCodecID)header.CodecID);
            int width = 0;
            int height = 0;
            byte[] frame = _decoder.Decode(header.Buffer, out width, out height);
        }
        private void onStreamEvent(IStreamPacket packet)
        {
            //savetoTxt(packet.Buffer);
            //Console.WriteLine();
            //string rtspstr = $"rtsp stream {packet.Buffer.Length}:";
            //for (int i = 0; i < Math.Min(100, packet.Buffer.Length); i++)
            //    rtspstr += string.Format("{0:X2}, ", packet.Buffer[i]);
            //Console.WriteLine(rtspstr);

            int width = 0;
            int height = 0;
            byte[] frameData = _decoder.Decode(packet.Buffer, out width, out height);
            if (frameData != null)
            {
                if (width != _width || height != _height)
                {
                    _width = width;
                    _height = height;
                    _renderSource.SetupSurface(width, height);
                }
                renderFrame(frameData, width, height);
            }
        }
        FileWriter _writer;
        private void savetoTxt(byte[] buffer)
        {
            _writer?.SavetoTxt(buffer);
        }

        void renderFrame(byte[] frame, int width, int height)
        {
            _renderSource.Render(frame);
        }

        void updateImageSource(ImageSource imgSrc)
        {
            //Console.WriteLine("Image Source!");
            this.ImageSrc = imgSrc;
        }

        public void Dispose()
        {
            if (_rtsp != null)
            {
                _rtsp.HeaderEvent -= onHeader;
                _rtsp.StreamEvent -= onStreamEvent;
                _rtsp.Dispose();
            }
            _rtsp = null;
            if(_renderSource != null)
                _renderSource.Dispose();
            _renderSource = null;
            _writer?.Dispose();
            _writer = null;

            _rtspServer?.Dispose();
            _rtspServer = null;
        }
    }
}
