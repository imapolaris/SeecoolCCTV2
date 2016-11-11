using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVClient;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels;
using FFmpeg;
using SocketHelper.Events;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;
using VideoStreamModels.Model;

namespace VideoStreamClient
{
    internal class CCTV2VideoSource : VideoSource
    {
        public FfmpegHeader FfmpegHeader { get; private set; }
        public HikM4Header HikM4Header { get; private set; }
        public string Url { get; private set; }
        public string VideoId { get; private set; }

        CCTV2StreamPipe _streamPipe;
        private bool _headerReceived = false;
        private VideoDeviceType _headerType;
        public CCTV2VideoSource(CCTVServerInfo curServer, CCTVServerInfo preferServer, string videoId, string url)
        {
            Url = url;
            VideoId = videoId;
            initDecoder();
            //生成StreamPipe
            createStreamPipe(curServer, preferServer, url);
        }

        private void createStreamPipe(CCTVServerInfo curServer, CCTVServerInfo preferServer, string url)
        {
            _streamPipe = new CCTV2StreamPipe(new Uri(url), curServer.StreamServerIp, curServer.StreamServerPort,
                preferServer.StreamServerIp, preferServer.StreamServerPort);

            _streamPipe.StreamHeaderReceived += streamPipe_StreamHeaderReceived;
            _streamPipe.StreamDataReceived += streamPipe_StreamDataReceived;
            _streamPipe.ErrorOccurred += streamPipe_ErrorOccurred;
        }

        private void initDecoder()
        {
            _ffmpegDecoder = new FfmpegDecoder();
            _hikm4Decoder = new HikM4Decoder();
            _univiewDecoder = new UniviewDecoder();

            _hikm4Decoder.VideoFrameEvent += hikm4Decoder_VideoFrameEvent;
            _univiewDecoder.VideoFrameEvent += univiewDecoder_VideoFrameEvent;
        }

        private void disposeDecoder()
        {
            _hikm4Decoder.VideoFrameEvent -= hikm4Decoder_VideoFrameEvent;
            _univiewDecoder.VideoFrameEvent -= univiewDecoder_VideoFrameEvent;

            _ffmpegDecoder.Dispose();
            _hikm4Decoder.Dispose();
            _univiewDecoder.Dispose();
        }

        private void univiewDecoder_VideoFrameEvent(UniviewDecoder.VideoFrame obj)
        {
            VideoFrame vf = new VideoFrame(obj.Width, obj.Height, obj.Timestamp, obj.Data);
            onVideoFrameParsed(new VideoFrameEventArgs(vf));
        }

        private void hikm4Decoder_VideoFrameEvent(HikM4Decoder.VideoFrame obj)
        {
            VideoFrame vf = new VideoFrame(obj.Width, obj.Height, obj.Timestamp, obj.Data);
            onVideoFrameParsed(new VideoFrameEventArgs(vf));
        }

        private void streamPipe_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.ErrorMessage);
            if (e.InnerException != null)
                Console.WriteLine(e.InnerException);
            Stop();
        }

        private void streamPipe_StreamDataReceived(object sender, StreamData e)
        {
            if (_headerReceived)
            {
                switch (_headerType)
                {
                    case VideoDeviceType.Hikv:
                        {
                            if (HikM4Header != null)
                            {
                                var package = new HikM4Package()
                                {
                                    Type = 0,
                                    Data = e.Buffer,
                                };
                                //数据通知
                                if (_notifyHikm4Data != null)
                                    _notifyHikm4Data(package);
                                onHikM4StreamReceived(new HikM4StreamEventArgs(package));
                                //视频流解码
                                if (_decode)
                                    _hikm4Decoder.InputData(package.Type, package.Data);
                            }
                        }
                        break;
                    case VideoDeviceType.Ffmpeg:
                        {
                            if (FfmpegHeader != null)
                            {
                                var package = new FfmpegPackage()
                                {
                                    CodecID = FfmpegHeader.CodecID,
                                    Width = 0,
                                    Height = 0,
                                    Type = 0,
                                    Pts = (ulong)e.Time.Ticks,
                                    Data = e.Buffer,
                                };
                                //数据通知
                                if (_notifyFfmpegData != null)
                                    _notifyFfmpegData(package);
                                onFfmpegStreamReceived(new FfmpegStreamEventArgs(package));
                                //视频流解码
                                if (_decode)
                                {
                                    var frame = _ffmpegDecoder.Decode(package.Type, package.Pts, package.Data);
                                    if (frame != null)
                                    {
                                        VideoFrame vf = new VideoFrame(frame.Width, frame.Height, (long)frame.Pts, frame.Data);
                                        onVideoFrameParsed(new VideoFrameEventArgs(vf));
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void streamPipe_StreamHeaderReceived(object sender, IStreamHeader e)
        {
            _headerType = e.DeviceType;
            switch (e.DeviceType)
            {
                case VideoDeviceType.Hikv:
                    {
                        _headerReceived = true;
                        HikM4Header header = new HikM4Header()
                        {
                            Type = HikM4Decoder.HeaderType,
                            Data = (e as HikvStreamHeader).Buffer
                        };
                        HikM4Header = header;
                        onHikM4HeaderReceived(new HikM4HeaderEventArgs(header));
                        //初始化包头
                        if (_decode)
                            _hikm4Decoder.InputData(header.Type, header.Data);
                    }
                    break;
                case VideoDeviceType.Ffmpeg:
                    {
                        _headerReceived = true;
                        FfmpegStreamHeader tH = e as FfmpegStreamHeader;
                        FfmpegHeader header = new FfmpegHeader()
                        {
                            CodecID = (Constants.AVCodecID)tH.CodecID,
                            Width = 0,
                            Height = 0
                        };
                        FfmpegHeader = header;
                        onFfmpegHeaderReceived(new FfmpegHeaderEventArgs(header));
                        //初始化包头
                        if (_decode)
                            _ffmpegDecoder.Init(header.CodecID, header.Width, header.Height);
                    }
                    break;
                default:
                    break;
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
            lock (_videoObj)
            {
                if (!_started)
                {
                    _started = true;
                    _streamPipe.Start();
                }
            }
        }

        private void Stop()
        {
            lock (_videoObj)
            {
                if (_started)
                {
                    _started = false;

                    _streamPipe.Stop();
                    FfmpegHeader = null;
                    HikM4Header = null;

                    _ffmpegDecoder.Release();
                    _hikm4Decoder.ReleasePlayer();
                    _univiewDecoder.ReleasePlayer();
                }
            }
        }

        private void update()
        {
            if (_streamPipe != null)
            {
                bool shouldRun = !IsDisposed && (_hikm4StreamReceived != null || _ffmpegStreamReceived != null || _streamTypeChanged != null || _univiewStreamReceived != null);
                if (shouldRun)
                    Start();
                else
                    Stop();
            }
        }

        private FfmpegDecoder _ffmpegDecoder = new FfmpegDecoder();
        private HikM4Decoder _hikm4Decoder = new HikM4Decoder();
        private UniviewDecoder _univiewDecoder = new UniviewDecoder();

        private bool _decode = false;
        private void checkToUpdateFrameEvent()
        {
            if (_videoFrameReceived != null)
            {
                _decode = true;
            }
            else
            {
                _decode = false;
            }
        }

        public bool IsDisposed { get; private set; } = false;
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Stop();
                disposeDecoder();
            }
        }

        ~CCTV2VideoSource()
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
