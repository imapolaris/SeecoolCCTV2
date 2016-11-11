using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoNS.VideoInfo;
using VideoStreamClient;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;
using CCTVModels;

namespace VideoNS.VideoDistribute
{
    internal class VideoBuffer : IDisposable
    {
        bool _disposed = false;
        VideoData _videoData;
        object _videoSourceLockObj = new object();

        public VideoBuffer(string id, int streamIndex = -1)
        {
            playAndRetry(id, streamIndex);
        }

        Timer _timer;

        void playAndRetry(string videoId, int streamIndex = -1)
        {
            if (!play(videoId, streamIndex))
            {
                if (!_disposed)
                {
                    TimerCallback callback = x =>
                    {
                        WindowUtil.BeginInvoke(() => playAndRetry(videoId, streamIndex));
                    };
                    _timer = new Timer(callback, null, 500, Timeout.Infinite);
                }
            }
        }

        bool play(string videoId, int streamIndex = -1)
        {
            if (!_disposed)
            {
                CCTVStaticInfo videoInfo = CCTVInfoManager.Instance.GetStaticInfo(videoId);
                if (videoInfo != null && videoInfo.Streams != null && videoInfo.Streams.Length > 0)
                {
                    if (streamIndex < 0)
                        streamIndex = videoInfo.Streams[0].Index;
                    StreamInfo streamInfo = videoInfo.Streams.First(x => x.Index == streamIndex);
                    if (streamInfo != null)
                    {
                        lock (_videoSourceLockObj)
                        {
                            _videoData = VideoDataManager.Instance.GetVideoData(videoId, streamInfo.Url);
                            _videoData.VideoSource.Hikm4StreamReceived += VideoSource_Hikm4StreamReceived;
                            _videoData.VideoSource.FfmpegStreamReceived += VideoSource_FfmpegStreamReceived;
                            _videoData.VideoSource.UniviewStreamReceived += VideoSource_UniviewStreamReceived;
                            _videoData.VideoSource.VideoFrameReceived += VideoSource_VideoFrameReceived;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void VideoSource_UniviewStreamReceived(object sender, UniviewStreamEventArgs e)
        {
        }

        private void VideoSource_VideoFrameReceived(object sender, VideoFrameEventArgs e)
        {
        }

        private void VideoSource_FfmpegStreamReceived(object sender, FfmpegStreamEventArgs e)
        {
        }

        private void VideoSource_Hikm4StreamReceived(object sender, HikM4StreamEventArgs e)
        {
        }

        public void Dispose()
        {
            _disposed = true;

            lock (_videoSourceLockObj)
            {
                if (_videoData != null)
                {
                    _videoData.VideoSource.FfmpegStreamReceived -= VideoSource_FfmpegStreamReceived;
                    _videoData.VideoSource.Hikm4StreamReceived -= VideoSource_Hikm4StreamReceived;
                    _videoData.VideoSource.UniviewStreamReceived -= VideoSource_UniviewStreamReceived;
                    _videoData.VideoSource.VideoFrameReceived -= VideoSource_VideoFrameReceived;
                }
                _videoData = null;
            }
        }
    }
}
