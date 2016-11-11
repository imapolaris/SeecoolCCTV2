using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.VideoFlashback;
using VideoNS.VideoInfo;
using VideoStreamClient.Entity;

namespace VideoNS.VideoDistribute
{
    public class VideoData
    {
        private static VideoStreamClient.VideoSourceManager _srcMgr;
        private static VideoStreamClient.VideoSourceManager SrcMgr
        {
            get
            {
                if (_srcMgr == null)
                    _srcMgr = VideoStreamClient.VideoSourceManager.CreateInstance(CCTVInfoManager.Instance.ClientHub);
                return _srcMgr;
            }
        }
        private FlashbackChannel _flashback;
        private VideoStreamClient.VideoSource _vSource;

        internal FlashbackChannel FlashBack { get { return _flashback; } }
        internal VideoStreamClient.VideoSource VideoSource { get { return _vSource; } }

        public VideoData(string videoId, string url)
        {
            _flashback = VideoFlashbackManager.Instance.GetChannel(videoId);
            _vSource = SrcMgr.GetVideoSource(videoId, url);
            _vSource.SetNotifyDataAction(updateFfmpegData); //接收FFMPEG数据通知。
            _vSource.SetNotifyDataAction(updateHikm4Data);  //接收HIKM4数据通知。
            _vSource.SetNotifyDataAction(updateUniviewData);
        }

        private void updateHikm4Data(HikM4Package package)
        {
            _flashback.InputHikM4Package(package);
        }

        private void updateFfmpegData(FfmpegPackage package)
        {
            _flashback.InputFfmpegPackage(package);
        }

        private void updateUniviewData(UniviewPackage package)
        {
            _flashback.InputUniviewPackage(package);
        }
    }
}
