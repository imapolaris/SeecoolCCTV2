using AopUtil.WpfBinding;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Common.Util;
using CCTVClient;

namespace CCTVReplay.Video
{
    public class VideoDisplayViewModel : ObservableObject
    {
        public VideoCacheManager StreamManager { get; private set; }
        VideoDisplayManager _displayer;

        public VideoDisplayViewModel(DownloadInfoParam param, ITimeProcess playProcess)
        {
            StreamManager = new VideoCacheManager(param);
            initDisplayer(StreamManager, playProcess);
        }

        public VideoDisplayViewModel(LocalDownloadInfoPacket param, DateTime begin, DateTime end, ITimeProcess palyProcess)
        {
            StreamManager = new VideoCacheManager(param, begin, end);
            initDisplayer(StreamManager, palyProcess);
        }

        private void initDisplayer(VideoCacheManager cache, ITimeProcess playProcess)
        {
            _displayer = new VideoDisplayManager(cache, playProcess);
            _displayer.OnCaching += onCaching;
            _displayer.OnHaveData += onHaveData;
            _displayer.VideoFrameImageEvent += onVideoFrameImage;
        }

        private void onHaveData(bool haveData)
        {
            NoVideoData = !haveData;
        }

        private void onCaching(bool cache)
        {
            OnBuffering = cache;
        }

        private void onVideoFrameImage(ImageSource obj)
        {
            WindowUtil.BeginInvoke(() =>
            {
                this.ImageSrc = obj;
                Console.WriteLine(StreamManager?.VideoName + ": Image Source Updated!");
            });
        }
        [AutoNotify]
        public ImageSource ImageSrc { get; set; }
        [AutoNotify]
        public bool NoVideoData { get; private set; }
        [AutoNotify]
        public bool OnBuffering { get; private set; }

        public void Download(string downloadPath)
        {
            StreamManager?.DownloadToPath(downloadPath);
        }

        internal Image GetSnapshot()
        {
            var frame = _displayer.LastFrame;
            if (frame != null)
                return ImageGrabber.GetImageFromYv12Data(frame.Width, frame.Height, frame.Data);
            else
                return null;
        }

        public void Close()
        {
            if (_displayer != null)
                _displayer.Dispose();
            _displayer = null;
            if (StreamManager != null)
                StreamManager.Dispose();
            StreamManager = null;
        }
    }
}