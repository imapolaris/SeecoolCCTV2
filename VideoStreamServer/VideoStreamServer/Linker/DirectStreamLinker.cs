using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVStreamCmd;
using CCTVStreamCmd.Hikvision;
using CCTVStreamCmd.Rtsp;
using Seecool.VideoStreamBase;
using VideoStreamModels.Model;
using VideoStreamServer.Model;

namespace VideoStreamServer.Linker
{
    internal class DirectStreamLinker : AbstractStreamLinker
    {
        private HikvUrlInfo _hikInfo;
        private VideoDeviceType _vType;
        private IStream _stream;
        public DirectStreamLinker(Uri uri)
        {
            StreamUri = uri;
            if (uri.Scheme.ToLower().Equals("hikv"))
            {
                _vType = VideoDeviceType.Hikv;
                _hikInfo = new HikvUrlInfo(uri.AbsoluteUri);
            }
            else if (uri.Scheme.ToLower().Equals("rtsp"))
                _vType = VideoDeviceType.Ffmpeg;
            else
                throw new ArgumentException("不能识别的Url模式:" + uri.Scheme);
        }

        protected override void Start()
        {
            if (_stream != null)
                Stop();
            switch (_vType)
            {
                case VideoDeviceType.Hikv:
                    {
                        _stream = new HikStream(_hikInfo.Ip, (ushort)_hikInfo.Port, _hikInfo.User, _hikInfo.Password, _hikInfo.Channel, _hikInfo.IsSubStream, IntPtr.Zero);
                    }
                    break;
                case VideoDeviceType.Ffmpeg:
                    {
                        _stream = new RtspStream(StreamUri.AbsoluteUri);
                    }
                    break;
                default:
                    break;
            }
            _stream.HeaderEvent += Stream_HeaderEvent;
            _stream.StreamEvent += Stream_StreamEvent;
        }

        protected override void Stop()
        {
            if (_stream != null)
            {
                _stream.HeaderEvent -= Stream_HeaderEvent;
                _stream.StreamEvent -= Stream_StreamEvent;
                _stream.Dispose();
            }
            CurrentHeader = null;
            _stream = null;
        }

        private void Stream_HeaderEvent(IHeaderPacket obj)
        {
            switch (_vType)
            {
                case VideoDeviceType.Hikv:
                    {
                        HikHeaderPacket hhp = obj as HikHeaderPacket;
                        if (hhp != null)
                        {
                            CurrentHeader = new HikvStreamHeader(hhp.Buffer);
                            OnStreamHeaderRecieved(CurrentHeader);
                        }
                    }
                    break;
                case VideoDeviceType.Ffmpeg:
                    {
                        FfmpegHeaderPacket fhp = obj as FfmpegHeaderPacket;
                        if (fhp != null)
                        {
                            CurrentHeader = new FfmpegStreamHeader(fhp.CodecID, fhp.Buffer);
                            OnStreamHeaderRecieved(CurrentHeader);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Stream_StreamEvent(IStreamPacket obj)
        {
            OnStreamDataReceived(new StreamData(obj.Time, obj.Buffer));
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }
    }
}
