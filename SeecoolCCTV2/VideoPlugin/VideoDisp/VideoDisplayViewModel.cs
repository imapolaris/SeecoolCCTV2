using CCTVClient;
using AopUtil.WpfBinding;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoNS.VideoInfo;
using UIManipulator;
using VideoNS.VideoDistribute;
using VideoNS.VideoFlashback;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using VideoNS.Thumbnail;
using System.IO;
using System.Threading;
using VideoRender;
using VideoStreamClient.Entity;
using VideoStreamClient;
using VideoStreamClient.Events;
using CCTVModels;

namespace VideoNS.VideoDisp
{
    public class VideoDisplayViewModel : ObservableObject, IDisposable
    {
        #region bindings

        [AutoNotify]
        public TransformParam VideoTransform { get; private set; }

        [AutoNotify]
        public ImageSource ImageSrc { get; private set; }

        [AutoNotify]
        public Stretch StretchMode { get; set; } = Stretch.Fill;

        [AutoNotify]
        public string VideoName { get; set; } = "未知视频";

        [AutoNotify]
        public string StreamName { get; private set; }

        [AutoNotify]
        public TimeSpan Latency { get; private set; }

        [AutoNotify]
        public StreamInfo[] StreamInfoArray { get; private set; }

        [AutoNotify]
        public int Width { get; private set; }

        [AutoNotify]
        public int Height { get; private set; }

        [AutoNotify]
        public double BitsPerSec { get; private set; }

        [AutoNotify]
        public double FramePerSec { get; private set; }

        [AutoNotify]
        public double FluentAverage { get; private set; }

        [AutoNotify]
        public int BufferFrameCount { get; private set; }

        [AutoNotify]
        public string VideoId { get; private set; }

        [AutoNotify]
        public bool FlashbackState { get; set; }

        [AutoNotify]
        public DateTime StartFlashbackTime { get; private set; }

        [AutoNotify]
        public DateTime EndFlashbackTime { get; private set; }

        [AutoNotify]
        public DateTime CurrentFlashbackTime { get; private set; }

        [AutoNotify]
        public double FlashbackSpeed { get; set; } = 1;

        [AutoNotify]
        public bool FlashbackPlaying { get; set; }

        [AutoNotify]
        public DateTime LastImageTime { get; private set; } = DateTime.Now;
        #endregion

        IRenderSource _renderSource;

        public VideoDisplayViewModel()
        {
            VideoTransform = new TransformParam();
            PropertyChanged += VideoDisplayViewModel_PropertyChanged;

            _frameBuffer.VideoFrameEvent += _frameBuffer_VideoFrameEvent;
            _frameBuffer.Start();

            _renderSource = new D3DImageSource();
            _renderSource.ImageSourceChanged += () => updateImageSource(_renderSource.ImageSource);

            UIService.SubscribeExitPoller(() => { Dispose(); return false; });
        }

        private void VideoDisplayViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var player = _flashbackPlayer;
            switch (e.PropertyName)
            {
                case nameof(FlashbackState):
                    onFlashbackChanged(FlashbackState);
                    break;
                case nameof(FlashbackSpeed):
                    if (player != null)
                        player.Speed = FlashbackSpeed;
                    break;
                case nameof(FlashbackPlaying):
                    if (player != null)
                        player.Playing = FlashbackPlaying;
                    break;
            }
        }

        public void SeekFlashback(DateTime time)
        {
            var player = _flashbackPlayer;
            if (player != null)
                player.CurrentTime = time;
        }

        IFlashbackPlayer _flashbackPlayer;
        DispatcherTimer _flashbackTimer;

        void releaseFlashbackPeriod()
        {
            FlashbackPlaying = false;
            if (_flashbackPlayer != null)
            {
                _flashbackPlayer.VideoFrameEvent -= _flashbackPeriod_VideoFrameEvent;
                _flashbackPlayer.Dispose();
            }
            _flashbackPlayer = null;

            if (_flashbackTimer != null)
                _flashbackTimer.Stop();
            _flashbackTimer = null;
        }

        private void onFlashbackChanged(bool flashback)
        {
            if (flashback)
            {
                if (_flashbackPlayer == null)
                {
                    _flashbackPlayer = VideoFlashbackManager.Instance.GetPlayer(VideoId);
                    StartFlashbackTime = _flashbackPlayer.StartTime;
                    EndFlashbackTime = _flashbackPlayer.EndTime;
                    CurrentFlashbackTime = _flashbackPlayer.CurrentTime;
                    _flashbackPlayer.VideoFrameEvent += _flashbackPeriod_VideoFrameEvent;
                    FlashbackPlaying = true;

                    _flashbackTimer = new DispatcherTimer();
                    _flashbackTimer.Interval = TimeSpan.FromMilliseconds(200);
                    _flashbackTimer.Tick += _flashbackTimer_Tick;
                    _flashbackTimer.Start();
                }
            }
            else
                releaseFlashbackPeriod();
        }

        private void _flashbackTimer_Tick(object sender, EventArgs e)
        {
            var player = _flashbackPlayer;
            if (player != null)
                CurrentFlashbackTime = player.CurrentTime;
        }

        private void _flashbackPeriod_VideoFrameEvent(VideoFrame frame)
        {
            _lastFrame = frame;
            //TODO:强制类型转换。有可能导致数据溢出。
            _frameBuffer.InputVideoFrame(frame.Width, frame.Height, frame.Data, (int)frame.Timestamp);
        }

        void updateImageSource(ImageSource imgSrc)
        {
            WindowUtil.BeginInvoke(() =>
            {
                this.ImageSrc = imgSrc;
            });
        }

        void updateStretch()
        {
            WindowUtil.BeginInvoke(() =>
            {
                Stretch stretch = this.StretchMode;
                this.StretchMode = Stretch.None;
                this.StretchMode = stretch;
            });
        }

        private ImageSource imageBytesToImageSource(byte[] bytes)
        {
            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = new MemoryStream(bytes);
            imageSource.EndInit();
            return imageSource;
        }

        VideoFrameBuffer _frameBuffer = new VideoFrameBuffer();
        VideoData _videoData;
        object _videoSourceLockObj = new object();
        XpsCalculator _bpsCalculator = new XpsCalculator();

        public void PlayVideo(string videoId, int streamIndex = -1)
        {
            StopVideo();

            VideoId = videoId;

            _tryTimes = 0;
            playAndRetry(videoId, streamIndex);
        }

        Timer _timer;
        int _tryTimes = 0;
        void playAndRetry(string videoId, int streamIndex = -1)
        {
            if (play(videoId, streamIndex))
            {
                _fluentAverageCalculator = new FluentAverageCalculator();
            }
            else
            {
                if (videoId == VideoId)
                {
                    TimerCallback callback = x =>
                    {
                        WindowUtil.BeginInvoke(() => playAndRetry(videoId, streamIndex));
                    };
                    if (_tryTimes++ >= 10)
                    {
                        string msg = "============已尝试很多次，仍无法播放视频:{0} ========";
                        Console.WriteLine(msg, videoId);
                        Common.Log.Logger.Default.Error(msg, videoId);
                    }
                    else
                        _timer = new Timer(callback, null, 500, Timeout.Infinite);
                }
            }
        }

        ThumbnailInfo _thumbnail = null;
        bool play(string videoId, int streamIndex = -1)
        {
            LastImageTime = DateTime.Now;

            if (videoId == VideoId)
            {
                //同一个VideoID，初始缩略图只需刷新一次，因为这是一个相当耗时的操作。
                //即使play操作失败时，也无需重复设置缩略图。
                if (_thumbnail == null)
                {
                    _thumbnail = ThumbnailsPack.Instance.GetThumbnail(videoId);
                    if (_thumbnail != null)
                    {
                        ImageSource imgSrc = imageBytesToImageSource(_thumbnail.ImageBytes);
                        updateImageSource(imgSrc);
                    }
                }
                //var thumbnail = ThumbnailManager.Default.GetThumbnail(videoId);
                //if (thumbnail != null)
                //{
                //    ImageSource imgSrc = imageBytesToImageSource(thumbnail.ImageBytes);
                //    updateImageSource(imgSrc);
                //}

                CCTVStaticInfo videoInfo = CCTVInfoManager.Instance.GetStaticInfo(videoId);
                if (videoInfo != null && videoInfo.Streams != null && videoInfo.Streams.Length > 0)
                {
                    VideoName = CCTVInfoManager.Instance.GetVideoReadableName(VideoId);
                    //Console.WriteLine(VideoId + "___" + VideoName);
                    StreamInfoArray = videoInfo.Streams;
                    if (streamIndex < 0)
                        streamIndex = videoInfo.Streams[0].Index;
                    StreamInfo streamInfo = videoInfo.Streams.First(x => x.Index == streamIndex);
                    if (streamInfo != null)
                    {
                        StreamName = streamInfo.Name;

                        lock (_videoSourceLockObj)
                        {
                            _videoData = VideoDataManager.Instance.GetVideoData(videoId, streamInfo.Url);
                            _videoData.VideoSource.Hikm4StreamReceived += VideoSource_Hikm4StreamReceived;
                            _videoData.VideoSource.FfmpegStreamReceived += VideoSource_FfmpegStreamReceived;
                            _videoData.VideoSource.UniviewStreamReceived += VideoSource_UniviewStreamReceived;
                            _videoData.VideoSource.StreamTypeChanged += VideoSource_StreamTypeChanged;
                            _videoData.VideoSource.VideoFrameReceived += VideoSource_VideoFrameReceived;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

       

        XpsCalculator _fpsCalculator = new XpsCalculator();
        VideoFrame _lastFrame = null;

        private void VideoSource_VideoFrameReceived(object sender, VideoFrameEventArgs e)
        {
            if (!FlashbackState)
            {
                _lastFrame = e.Frame;
                //TODO:有可能导致数据溢出的强制数据转换。
                _frameBuffer.InputVideoFrame(e.Frame.Width, e.Frame.Height, e.Frame.Data, (int)e.Frame.Timestamp);
            }
            FramePerSec = _fpsCalculator.Calculate(1);
        }

        private void VideoSource_StreamTypeChanged(object sender, StreamTypeEventArgs e)
        {
            StreamName = e.StreamName;
        }

        #region 【互斥方法组，同一视频只会执行一个组内方法】
        private void VideoSource_Hikm4StreamReceived(object sender, HikM4StreamEventArgs e)
        {
            BitsPerSec = _bpsCalculator.Calculate(e.Package.Data.Length * 8);
        }
        private void VideoSource_FfmpegStreamReceived(object sender, FfmpegStreamEventArgs e)
        {
            BitsPerSec = _bpsCalculator.Calculate(e.Package.Data.Length * 8);
        }
        private void VideoSource_UniviewStreamReceived(object sender, UniviewStreamEventArgs e)
        {
            BitsPerSec = _bpsCalculator.Calculate(e.Packet.Data.Length * 8);
        }
        #endregion 【互斥方法组，同一视频只会执行一个组内方法】

        void releaseVideoSource()
        {
            lock (_videoSourceLockObj)
            {
                if (_videoData != null)
                {
                    _videoData.VideoSource.FfmpegStreamReceived -= VideoSource_FfmpegStreamReceived;
                    _videoData.VideoSource.Hikm4StreamReceived -= VideoSource_Hikm4StreamReceived;
                    _videoData.VideoSource.UniviewStreamReceived -= VideoSource_UniviewStreamReceived;
                    _videoData.VideoSource.StreamTypeChanged -= VideoSource_StreamTypeChanged;
                    _videoData.VideoSource.VideoFrameReceived -= VideoSource_VideoFrameReceived;
                }
                _videoData = null;
            }
        }

        public void StopVideo()
        {
            _thumbnail = null;
            VideoName = "未知视频";
            VideoId = string.Empty;
            _timer = null;
            releaseVideoSource();
            _frameBuffer.Clear();
            Width = 0;
            Height = 0;
        }

        public void ClearVideoImage()
        {
            if (_lastFrame != null)
            {
                int width = Width;
                int height = Height;
                if (width == 0 || height == 0)
                {
                    width = 704;
                    height = 576;
                }
                var frame = getClearFrame(width, height, (int)_lastFrame.Timestamp + 1);
                if (frame != null)
                    _frameBuffer.InputVideoFrame(frame.Width, frame.Height, frame.Data, (int)frame.Timestamp);
            }
        }

        FluentAverageCalculator _fluentAverageCalculator = new FluentAverageCalculator();
        private void _frameBuffer_VideoFrameEvent(int width, int height, byte[] frameData, int timeStamp)
        {
            updateFrame(frameData, width, height);
            FluentAverage = _fluentAverageCalculator.Calculate();
            BufferFrameCount = _frameBuffer.Count;
        }

        void updateFrame(byte[] frame, int width, int height)
        {
            if (width != Width || height != Height)
            {
                Width = width;
                Height = height;
                _renderSource.SetupSurface(width, height);
                updateStretch();
            }
            renderFrame(frame, width, height);
        }

        void renderFrame(byte[] frame, int width, int height)
        {
            _renderSource.Render(frame);

            LastImageTime = DateTime.Now;
        }

        public void Dispose()
        {
            StopVideo();
            releaseFlashbackPeriod();

            _frameBuffer.VideoFrameEvent -= _frameBuffer_VideoFrameEvent;
            _frameBuffer.Stop();
        }

        ~VideoDisplayViewModel()
        {
            Dispose();
        }

        public System.Drawing.Image GetSnapshot()
        {
            var frame = _lastFrame;
            if (frame != null)
                return ImageGrabber.GetImageFromYv12Data(frame.Width, frame.Height, frame.Data);
            else
                return null;
        }

        static VideoFrame getClearFrame(int width, int height, int timestamp)
        {
            if (width > 0 && height > 0)
            {
                byte[] data = new byte[width * height * 3 / 2];
                for (int i = width * height; i < data.Length; i++)
                    data[i] = 0x80;
                return new VideoFrame()
                {
                    Width = width,
                    Height = height,
                    Data = data,
                    Timestamp = timestamp,
                };
            }
            else
                return null;
        }
    }
}
