using Media.Container;
using Media.Rtp;
using Media.Rtsp;
using Media.Rtsp.Server;
using Media.Rtsp.Server.MediaTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd
{
    public class StreamRtspServer : IDisposable
    {
        RtspServer _rtspServer;
        VideoRtspMedia _media;
        FileWriter _writer;
        public StreamRtspServer()
        {
            _rtspServer = new RtspServer(IPAddress.Any, 554);
            
            //_writer = new FileWriter(@"d:\baseframe.txt");
        }

        public void UpdateHeader(byte[] header)
        {
            _writer?.SavetoTxt(header);
            _media = new VideoRtspMedia(1920, 1080, "stream");
            _media.UpdateHeader(header);
            _rtspServer.TryAddMedia(_media);
            _rtspServer.Start();
            //_media.Stop();
            //_media.Start();
        }

        public void UpdateStream(byte[] stream)
        {
            _writer?.SavetoTxt(stream.ToList().GetRange(0, Math.Min(100, stream.Length)).ToArray());//前100个字符
            if (_media != null && _media.State == SourceMedia.StreamState.Started)
                _media.AddFrame(stream);
        }

        public void UpdateRtpFrame(RtpFrame frame)
        {
            if (_media != null && _media.State == SourceMedia.StreamState.Started)
                _media.AddFrame(frame);
        }

        public void Dispose()
        {
            if (_rtspServer != null && _rtspServer.IsRunning)
            {
                _rtspServer.Stop();
                _rtspServer.Dispose();
            }
            _rtspServer = null;
            _media?.Dispose();
            _media = null;
            _writer?.Dispose();
            _writer = null;
        }
    }
}
