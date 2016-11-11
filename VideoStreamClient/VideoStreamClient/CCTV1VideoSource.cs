using CCTVClient;
using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;
using CCTVInfoHub;
using CCTVModels;

namespace VideoStreamClient
{
    internal class CCTV1VideoSource : VideoSource
    {
        private CCTVDefaultInfoSync _clientHub;
        public string VideoId { get; private set; }
        public string Url { get; private set; }
        CCTVVideo _video;

        public HikM4Header HikM4Header { get; private set; }
        public FfmpegHeader FfmpegHeader { get; private set; }

        static bool getCctv1UrlInfo(string url, out ulong id, out int stream)
        {
            id = 0;
            stream = 1;
            Uri uri = new Uri(url);
            if (uri.Scheme.ToLower() == "cctv1")
            {
                if (uri.Segments.Length >= 2 && ulong.TryParse(uri.Segments[1].Trim('/'), out id))
                {
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    int.TryParse(query["stream"], out stream);
                    return true;
                }
            }
            return false;
        }

        internal CCTV1VideoSource(CCTVDefaultInfoSync clientHub, CCTVInfo info, string videoId, string url)
        {
            _clientHub = clientHub;
            VideoId = videoId;
            Url = url;

            ulong id = 0;
            int stream = 1;
            if (getCctv1UrlInfo(Url, out id, out stream))
            {
                //_flashback = VideoFlashbackManager.Instance.GetChannel(videoId);

                _video = new CCTVVideo(info, id, getStreamBandwidth(stream));
                _video.VideoStreamEvent += video_VideoStreamEvent;
                _video.FFMPEGFormatEvent += video_FFMPEGFormatEvent;
                _video.FFMPEGFrameEvent += _video_FFMPEGFrameEvent;
                _video.UniviewDataEvent += _video_UniviewDataEvent;
                _video.VideoLevelEvent += video_VideoLevelEvent;
            }
        }

        private void _video_UniviewDataEvent(string decodeTag, byte[] data)
        {
            UniviewPackage up = new UniviewPackage()
            {
                DecodeTag = decodeTag,
                Data = data
            };
            //数据通知
            if (_notifyUniviewData != null)
                _notifyUniviewData(up);
            onUniviewStreamReceived(new UniviewStreamEventArgs(up));
        }

        private void _video_VideoFrameEvent(int width, int height, byte[] data, int timeStamp)
        {
            VideoFrame vf = new VideoFrame(width, height, timeStamp, data);
            onVideoFrameParsed(new VideoFrameEventArgs(vf));
        }

        private void video_VideoLevelEvent(int level)
        {
            if (level > 0)
            {
                int actualStream = 4 - level;
                StreamInfo si = findStreamInfo(VideoId, actualStream);
                if (si != null)
                    onStreamTypeChanged(new StreamTypeEventArgs(si));
            }
        }

        private StreamInfo findStreamInfo(string videoId, int actualStream)
        {
            CCTVStaticInfo staticInfo = _clientHub.GetStaticInfo(videoId);
            if (staticInfo != null)
            {
                foreach (StreamInfo streamInfo in staticInfo.Streams)
                {
                    ulong id = 0;
                    int stream = 1;
                    if (getCctv1UrlInfo(streamInfo.Url, out id, out stream))
                    {
                        if (stream == actualStream)
                            return streamInfo;
                    }
                }
            }
            return null;
        }

        private void video_VideoStreamEvent(int frameType, byte[] frameData)
        {
            var package = new HikM4Package()
            {
                Type = frameType,
                Data = frameData,
            };
            //_flashback.InputHikM4Package(package);
            //数据通知
            if (_notifyHikm4Data != null)
                _notifyHikm4Data(package);

            if (frameType == HikM4Decoder.HeaderType)
            {
                HikM4Header header = new HikM4Header()
                {
                    Type = frameType,
                    Data = frameData
                };
                HikM4Header = header;
                onHikM4HeaderReceived(new HikM4HeaderEventArgs(header));
            }
            else
            {
                if (HikM4Header != null)
                    onHikM4StreamReceived(new HikM4StreamEventArgs(package));
            }
        }

        Constants.AVCodecID _ffmpegCodecId = Constants.AVCodecID.AV_CODEC_ID_NONE;
        int _ffmpegWidth = 0;
        int _ffmpegHeight = 0;

        private void video_FFMPEGFormatEvent(Constants.AVCodecID codecID, int width, int height)
        {
            _ffmpegCodecId = codecID;
            _ffmpegWidth = width;
            _ffmpegHeight = height;
            FfmpegHeader header = new FfmpegHeader()
            {
                CodecID = codecID,
                Width = width,
                Height = height
            };
            FfmpegHeader = header;
            onFfmpegHeaderReceived(new FfmpegHeaderEventArgs(header));
        }

        private void _video_FFMPEGFrameEvent(int type, ulong pts, byte[] data)
        {
            var package = new FfmpegPackage()
            {
                CodecID = _ffmpegCodecId,
                Width = _ffmpegWidth,
                Height = _ffmpegHeight,
                Type = type,
                Pts = pts,
                Data = data,
            };
            //_flashback.InputFfmpegPackage(package);
            //数据通知
            if (_notifyFfmpegData != null)
                _notifyFfmpegData(package);
            if (FfmpegHeader != null)
                onFfmpegStreamReceived(new FfmpegStreamEventArgs(package));
        }

        private static int getStreamBandwidth(int stream)
        {
            switch (stream)
            {
                case 1:
                    return 2000000;
                case 2:
                    return 512000;
                case 3:
                    return 128000;
                default:
                    return 2000000;
            }
        }

        private Action<FfmpegPackage> _notifyFfmpegData;
        private Action<HikM4Package> _notifyHikm4Data;
        private Action<UniviewPackage> _notifyUniviewData;

        /// <summary>
        /// 设置接收FFMPEG数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        public void SetNotifyDataAction(Action<FfmpegPackage> notify)
        {
            _notifyFfmpegData = notify;
        }

        /// <summary>
        /// 设置接收Hikm4数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        public void SetNotifyDataAction(Action<HikM4Package> notify)
        {
            _notifyHikm4Data = notify;
        }

        /// <summary>
        /// 设置接收Uniview数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        public void SetNotifyDataAction(Action<UniviewPackage> notify)
        {
            _notifyUniviewData = notify;
        }

        bool _started = false;
        private object _videoObj = new object();
        private void Start()
        {
            if (!_started)
            {
                _started = true;
                lock (_videoObj)
                {
                    _video.Start();
                }
            }
        }

        private void Stop()
        {
            if (_started)
            {
                _started = false;
                lock (_videoObj)
                {
                    _video.Stop();
                }
                FfmpegHeader = null;
                HikM4Header = null;
            }
        }

        private void update()
        {
            if (_video != null)
            {
                bool shouldRun = !IsDisposed && (_hikm4StreamReceived != null || _ffmpegStreamReceived != null || _streamTypeChanged != null || _univiewStreamReceived != null || _videoFrameReceived != null);
                if (shouldRun)
                    Start();
                else
                    Stop();
            }
        }

        private bool _frameEventInstalled = false;
        private void checkToUpdateFrameEvent()
        {
            if (_video != null)
            {
                if (_videoFrameReceived != null)
                {
                    if (!_frameEventInstalled)
                    {
                        _video.VideoFrameEvent += _video_VideoFrameEvent;
                        _frameEventInstalled = true;
                    }
                }
                else
                {
                    if (_frameEventInstalled)
                    {
                        _video.VideoFrameEvent -= _video_VideoFrameEvent;
                        _frameEventInstalled = false;
                    }
                }
            }
        }

        public bool IsDisposed { get; private set; } = false;
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Stop();
            }
        }

        ~CCTV1VideoSource()
        {
            Dispose();
        }
        #region 【事件定义】
        private event EventHandler<VideoFrameEventArgs> _videoFrameReceived;
        public event EventHandler<VideoFrameEventArgs> VideoFrameReceived
        {
            add
            {
                _videoFrameReceived += value;
                checkToUpdateFrameEvent();
                update();
            }
            remove
            {
                _videoFrameReceived -= value;
                checkToUpdateFrameEvent();
                update();
            }
        }
        private event EventHandler<HikM4HeaderEventArgs> _hikm4HeaderReceived;
        public event EventHandler<HikM4HeaderEventArgs> Hikm4HeaderReceived
        {
            add
            {
                _hikm4HeaderReceived += value;
                update();
                if (HikM4Header != null)
                    value(this, new HikM4HeaderEventArgs(HikM4Header));
            }
            remove
            {
                _hikm4HeaderReceived -= value;
                update();
            }
        }
        private event EventHandler<HikM4StreamEventArgs> _hikm4StreamReceived;
        public event EventHandler<HikM4StreamEventArgs> Hikm4StreamReceived
        {
            add
            {
                _hikm4StreamReceived += value;
                update();
            }
            remove
            {
                _hikm4StreamReceived -= value;
                update();
            }
        }
        private event EventHandler<FfmpegHeaderEventArgs> _ffmpegHeaderReceived;
        public event EventHandler<FfmpegHeaderEventArgs> FfmpegHeaderReceived
        {
            add
            {
                _ffmpegHeaderReceived += value;
                update();
                if (FfmpegHeader != null)
                    value(this, new FfmpegHeaderEventArgs(FfmpegHeader));
            }
            remove
            {
                _ffmpegHeaderReceived -= value;
                update();
            }
        }
        private event EventHandler<FfmpegStreamEventArgs> _ffmpegStreamReceived;
        public event EventHandler<FfmpegStreamEventArgs> FfmpegStreamReceived
        {
            add
            {
                _ffmpegStreamReceived += value;
                update();
            }
            remove
            {
                _ffmpegStreamReceived -= value;
                update();
            }
        }

        private event EventHandler<StreamTypeEventArgs> _streamTypeChanged;
        public event EventHandler<StreamTypeEventArgs> StreamTypeChanged
        {
            add
            {
                _streamTypeChanged += value;
                update();
            }
            remove
            {
                _streamTypeChanged -= value;
                update();
            }
        }

        private event EventHandler<UniviewStreamEventArgs> _univiewStreamReceived;
        public event EventHandler<UniviewStreamEventArgs> UniviewStreamReceived
        {
            add
            {
                _univiewStreamReceived += value;
                update();
            }
            remove
            {
                _univiewStreamReceived -= value;
                update();
            }
        }

        private void onVideoFrameParsed(VideoFrameEventArgs args)
        {
            var handler = _videoFrameReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onHikM4HeaderReceived(HikM4HeaderEventArgs args)
        {
            var handler = _hikm4HeaderReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onHikM4StreamReceived(HikM4StreamEventArgs args)
        {
            var handler = _hikm4StreamReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onFfmpegHeaderReceived(FfmpegHeaderEventArgs args)
        {
            var handler = _ffmpegHeaderReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onFfmpegStreamReceived(FfmpegStreamEventArgs args)
        {
            var handler = _ffmpegStreamReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onUniviewStreamReceived(UniviewStreamEventArgs args)
        {
            var handler = _univiewStreamReceived;
            if (handler != null)
                handler(this, args);
        }

        private void onStreamTypeChanged(StreamTypeEventArgs args)
        {
            var handler = _streamTypeChanged;
            if (handler != null)
                handler(this, args);
        }
        #endregion 【事件定义】
    }
}
