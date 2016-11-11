using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CCTVClient
{
    public class CCTVVideo
    {
        public CCTVInfo Info { get; private set; }
        public ulong VideoID { get; private set; }
        public int Bandwidth { get; set; }

        public CCTVVideo(CCTVInfo info, ulong id, int bandwidth = 2000000)
        {
            Info = info;
            VideoID = id;
            Bandwidth = bandwidth;

            _hikm4Decoder.VideoFrameEvent += _hikm4Decoder_VideoFrameEvent;
            _univiewDecoder.VideoFrameEvent += _univiewDecoder_VideoFrameEvent;
        }

        public delegate void OnVideoPacket(int frameType, byte[] frameData);
        public event OnVideoPacket VideoStreamEvent;
        private void fireOnVideoStream(int frameType, byte[] frameData)
        {
            OnVideoPacket callback = VideoStreamEvent;
            if (callback != null)
                callback(frameType, frameData);
        }

        public delegate void OnFFMPEGFormat(Constants.AVCodecID codecID, int width, int height);
        public event OnFFMPEGFormat FFMPEGFormatEvent;
        private void fireOnFFMPEGFormat(Constants.AVCodecID codecID, int width, int height)
        {
            OnFFMPEGFormat callback = FFMPEGFormatEvent;
            if (callback != null)
                callback(codecID, width, height);
        }

        public delegate void OnFFMPEGFrame(int type, ulong pts, byte[] data);
        public event OnFFMPEGFrame FFMPEGFrameEvent;
        private void fireOnFFMPEGFrame(int type, ulong pts, byte[] data)
        {
            OnFFMPEGFrame callback = FFMPEGFrameEvent;
            if (callback != null)
                callback(type, pts, data);
        }

        public delegate void OnUniviewData(string decodeTag, byte[] data);
        public event OnUniviewData UniviewDataEvent;
        private void fireOnUniviewData(string decodeTag, byte[] data)
        {
            var callback = UniviewDataEvent;
            if (callback != null)
                callback(decodeTag, data);
        }

        public delegate void OnVideoFrame(int width, int height, byte[] data, int timeStamp);
        public event OnVideoFrame VideoFrameEvent
        {
            add
            {
                lock (_videoFrameEventLockObj)
                {
                    if (_videoFrameEvent == null)
                        _decode = true;
                    _videoFrameEvent += value;
                }
            }
            remove
            {
                lock (_videoFrameEventLockObj)
                {
                    _videoFrameEvent -= value;
                    if (_videoFrameEvent == null)
                        _decode = false;
                }
            }
        }
        private object _videoFrameEventLockObj = new object();
        private event OnVideoFrame _videoFrameEvent;
        private void fireOnVideoFrame(int width, int height, byte[] data, int timeStamp)
        {
            OnVideoFrame callback = _videoFrameEvent;
            if (callback != null)
                callback(width, height, data, timeStamp);
        }

        public event Action VideoConnectedEvent;
        private void fireOnVideoConnected()
        {
            Action callback = VideoConnectedEvent;
            if (callback != null)
                callback();
        }
        public event Action VideoDisconnetedEvent;
        private void fireOnVideoDisconnected()
        {
            Action callback = VideoDisconnetedEvent;
            if (callback != null)
                callback();
        }

        public delegate void OnResponseData(byte[] data);
        public event OnResponseData ResponseDataEvent;
        private void fireOnResponseData(byte[] data)
        {
            var callback = ResponseDataEvent;
            if (callback != null)
                callback(data);
        }

        public delegate void OnVideoLevel(int level);
        public event OnVideoLevel VideoLevelEvent;
        private void fireOnVideoLevel(int level)
        {
            var callback = VideoLevelEvent;
            if (callback != null)
                callback(level);
        }

        private ManualResetEvent _event = new ManualResetEvent(false);
        private Thread _thread;
        private CCTVConnection _connection;
        private int _port;
        private bool _gotPort;
        private ManualResetEvent _portEvent = new ManualResetEvent(false);
        private ManualResetEvent _videoEvent = new ManualResetEvent(false);
        private DateTime _lastRecvTime = DateTime.MinValue;
        private int _videoLevel;
        private bool _decode = false;

        void setVideoLevel(int level)
        {
            _videoLevel = level;
            fireOnVideoLevel(_videoLevel);
        }

        public void Start()
        {
            _event.Reset();
            _thread = new Thread(new ThreadStart(thread));
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _event.Set();
            _thread.Join();
        }

        private void thread()
        {
            Info.VideoPortEvent += Info_VideoPortEvent;
            Info.VideoMissEvent += Info_VideoMissEvent;

            do
            {
                _gotPort = false;
                _portEvent.Reset();
                if (query() && waitForVideoPort())
                    videoRun();
            }
            while (!_event.WaitOne(5000));

            Info.VideoPortEvent -= Info_VideoPortEvent;
            Info.VideoMissEvent -= Info_VideoMissEvent;
        }

        private void videoRun()
        {
            _videoEvent.Reset();
            _connection = new CCTVConnection(Info.ServerHost, _port);

            _connection.ConnectEvent += _connection_ConnectEvent;
            _connection.DisconnectEvent += _connection_DisconnectEvent;
            _connection.MessageEvent += _connection_MessageEvent;
            _connection.Start();

            WaitHandle[] handles = new WaitHandle[] { _event, _videoEvent };
            _lastRecvTime = DateTime.Now;
            while (true)
            {
                int wait = WaitHandle.WaitAny(handles, 1000);
                if (wait == WaitHandle.WaitTimeout)
                {
                    if (DateTime.Now - _lastRecvTime > TimeSpan.FromSeconds(30))
                        break;
                }
                else
                    break;
            }

            _connection.ConnectEvent -= _connection_ConnectEvent;
            _connection.DisconnectEvent -= _connection_DisconnectEvent;
            _connection.MessageEvent -= _connection_MessageEvent;
            _connection.Stop();

            _hikm4Decoder.Dispose();
            _ffmpegDecoder.Release();
            _univiewDecoder.Dispose();
        }

        private void _connection_ConnectEvent()
        {
            _lastRecvTime = DateTime.Now;
            MessageBuilder mb = new MessageBuilder(0x11023);
            mb.Writer.Write(VideoID);
            _connection.Send(mb.ToMessage());
        }

        private void _connection_DisconnectEvent()
        {
            _videoEvent.Set();
            fireOnVideoDisconnected();
        }

        private void _connection_MessageEvent(byte[] message)
        {
            MessageReader mr = new MessageReader(message);
            //Console.WriteLine(mr.MessageID.ToString("X"));
            switch (mr.MessageID)
            {
                case 0x11024: // Msg_Video_Login_Accept
                    getVideo();
                    fireOnVideoConnected();
                    break;
                case 0x11025: // Msg_Video_Login_Reject
                    _videoEvent.Set();
                    break;
                case 0x12012: // Msg_Video_Bandwidth
                    {
                        int band1 = mr.Reader.ReadInt32();
                        int band2 = mr.Reader.ReadInt32();
                        int band3 = mr.Reader.ReadInt32();
                        bool urgent = mr.Reader.ReadInt32() != 0;
                        if (band3 > 0)
                            setVideoLevel(3);
                        else if (band2 > 0)
                            setVideoLevel(2);
                        else if (band1 > 0)
                            setVideoLevel(1);
                        else
                            setVideoLevel(0);
                    }
                    break;
                case 0x12123: // Msg_HikM4_Package2
                    {
                        int level = mr.Reader.ReadInt32();
                        if (level == _videoLevel)
                        {
                            int frameType = mr.Reader.ReadInt32();
                            int size = mr.Reader.ReadInt32();
                            byte[] frameData = mr.Reader.ReadBytes(size);

                            _lastRecvTime = DateTime.Now;
                            fireOnVideoStream(frameType, frameData);

                            if (_decode)
                                _hikm4Decoder.InputData(frameType, frameData);
                        }
                    }
                    break;
                case 0x12025: // Msg_Ffmpeg_Format
                    {
                        int level = mr.Reader.ReadInt32();
                        if (level == _videoLevel)
                        {
                            _lastRecvTime = DateTime.Now;
                            Constants.AVCodecID codecID = (Constants.AVCodecID)mr.Reader.ReadInt32();
                            int width = mr.Reader.ReadInt32();
                            int height = mr.Reader.ReadInt32();

                            fireOnFFMPEGFormat(codecID, width, height);

                            if (_decode)
                                _ffmpegDecoder.Init(codecID, width, height);
                        }
                    }
                    break;
                case 0x12026: // Msg_Ffmpeg_Frame
                    {
                        int level = mr.Reader.ReadInt32();
                        if (level == _videoLevel)
                        {
                            _lastRecvTime = DateTime.Now;
                            int type = mr.Reader.ReadInt32();
                            ulong pts = mr.Reader.ReadUInt64();
                            int size = mr.Reader.ReadInt32();
                            byte[] bytes = new byte[size];
                            mr.Reader.Read(bytes, 0, size);

                            fireOnFFMPEGFrame(type, pts, bytes);

                            if (_decode)
                            {
                                var frame = _ffmpegDecoder.Decode(type, pts, bytes);
                                if (frame != null)
                                    fireOnVideoFrame(frame.Width, frame.Height, frame.Data, (int)frame.Pts);
                            }
                        }
                    }
                    break;
                case 0x12027: // Msg_Uniview_Package
                    {
                        string decodeTag = mr.Reader.ReadString();
                        int size = mr.Reader.ReadInt32();
                        byte[] data = mr.Reader.ReadBytes(size);

                        fireOnUniviewData(decodeTag, data);

                        if (_decode)
                            _univiewDecoder.InputData(decodeTag, data);
                    }
                    break;
                case 0x12511: // Msg_Transparent_Recv
                    {
                        int length = mr.Reader.ReadInt32();
                        byte[] data = new byte[length];
                        mr.Reader.Read(data, 0, length);
                        fireOnResponseData(data);
                    }
                    break;
            }
        }

        private void getVideo()
        {
            setVideoLevel(0);

            int band1 = 0, band2 = 0, band3 = 0;
            if (Bandwidth < 512000)
                band1 = Bandwidth;
            else if (Bandwidth < 2000000)
                band2 = Bandwidth;
            else
                band3 = Bandwidth;

            MessageBuilder mb = new MessageBuilder(0x12010);
            mb.Writer.Write((int)704);
            mb.Writer.Write((int)576);
            mb.Writer.Write((int)1);
            mb.Writer.Write((int)0);
            mb.Writer.Write(band1);
            mb.Writer.Write(band2);
            mb.Writer.Write(band3);
            mb.Writer.Write((int)0);
            mb.Writer.Write((int)0);
            mb.Writer.Write((int)0);
            mb.Writer.Write((int)0);
            _connection.Send(mb.ToMessage());
        }

        private bool waitForVideoPort()
        {
            WaitHandle[] handles = new WaitHandle[] { _event, _portEvent };
            if (WaitHandle.WaitAny(handles, 10000) == 1)
                return _gotPort;
            return false;
        }

        void Info_VideoMissEvent(ulong videoID)
        {
            if (videoID == VideoID)
            {
                _gotPort = false;
                _portEvent.Set();
            }
        }

        void Info_VideoPortEvent(ulong videoID, int port, int bandwidth)
        {
            if (videoID == VideoID)
            {
                _port = port;
                _gotPort = true;
                _portEvent.Set();
            }
        }

        private bool query()
        {
            if (Info.Ready)
            {
                Info.QueryVideo(VideoID);
                return true;
            }

            return false;
        }

        private FfmpegDecoder _ffmpegDecoder = new FfmpegDecoder();
        private HikM4Decoder _hikm4Decoder = new HikM4Decoder();
        private UniviewDecoder _univiewDecoder = new UniviewDecoder();

        private void _hikm4Decoder_VideoFrameEvent(HikM4Decoder.VideoFrame frame)
        {
            fireOnVideoFrame(frame.Width, frame.Height, frame.Data, frame.Timestamp);
        }

        private void _univiewDecoder_VideoFrameEvent(UniviewDecoder.VideoFrame frame)
        {
            fireOnVideoFrame(frame.Width, frame.Height, frame.Data, (int)frame.Timestamp);
        }

        public void TransparentSend(byte[] data, int offset, int length)
        {
            MessageBuilder mb = new MessageBuilder(0x12510); // Msg_Transparent_Send
            mb.Writer.Write(length);
            mb.Writer.Write(data, offset, length);
            _connection.Send(mb.ToMessage());
        }
    }
}
