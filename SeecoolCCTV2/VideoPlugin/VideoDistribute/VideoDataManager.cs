using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoNS.VideoDistribute
{
    public class VideoDataManager
    {
        #region 【单例】
        static VideoDataManager()
        {
            Instance = new VideoDataManager();
        }
        public static VideoDataManager Instance { get; private set; }

        internal void Init()
        {
        }
        #endregion 【单例】

        #region【码流】
        ConcurrentDictionary<string, VideoData> _videoSourceDict = new ConcurrentDictionary<string, VideoData>();
        public VideoData GetVideoData(string videoId, string url)
        {
            return _videoSourceDict.GetOrAdd(url, x => new VideoData(videoId, url));
        }
        #endregion【码流】
    }
}
