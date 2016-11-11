using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVInfoHub.Entity;
using Common.Configuration;
using VideoNS.VideoInfo;

namespace VideoNS.VideoDistribute
{
    public class VideoBufferManager
    {
        #region 【单例】
        static VideoBufferManager()
        {
            Instance = new VideoBufferManager();
        }
        public static VideoBufferManager Instance { get; private set; }

        internal void Init()
        {
        }
        #endregion 【单例】

        #region【缓存服务】
        public class VideoBuffering
        {
            public string Id = string.Empty;
            public int StreamIndex = 0;
        };

        private VideoBufferManager()
        {
            bool videoBuffering = Utils.Config.StringToBool(ConfigHandler.GetValue<VideoInfoPlugin>("VideoBuffering"));

            if (videoBuffering)
            {
                SyncParams<VideoBuffering> param = new SyncParams<VideoBuffering>("VideoBuffering", TimeSpan.FromSeconds(10),onVideoBufferingUpdate);
                CCTVInfoManager.Instance.ClientHub.RegisterSynchronizer<VideoBuffering>(param);
            }
            //_videoBufSync = new StaticInfoSynchronizer<VideoBuffering>(VideoInfoManager.Instance.WebApiBaseUri, "VideoBuffering", TimeSpan.FromSeconds(10), null, onVideoBufferingUpdate);
        }

        ConcurrentDictionary<string, VideoBuffer> _bufferDict = new ConcurrentDictionary<string, VideoBuffer>();
        private void onVideoBufferingUpdate(IEnumerable<string> updatedKeys)
        {
            foreach (string key in updatedKeys)
            {
                VideoBuffering buffering = CCTVInfoManager.Instance.ClientHub.GetRegisteredInfo<VideoBuffering>(key);
                if (buffering != null)
                {
                    _bufferDict.GetOrAdd(key, x => new VideoBuffer(buffering.Id, buffering.StreamIndex));
                }
                else
                {
                    VideoBuffer buffer = null;
                    if (_bufferDict.TryRemove(key, out buffer))
                        buffer.Dispose();
                }
            }
        }

        //旧代码
        //private void onVideoBufferingUpdate(StaticInfoSynchronizer<VideoBuffering> synchronizer, IEnumerable<string> updatedKeys)
        //{
        //    foreach (string key in updatedKeys)
        //    {
        //        VideoBuffering buffering = null;
        //        if (synchronizer.TryGetValue(key, out buffering))
        //        {
        //            _bufferDict.GetOrAdd(key, x => new VideoBuffer(buffering.Id, buffering.StreamIndex));
        //        }
        //        else
        //        {
        //            VideoBuffer buffer = null;
        //            if (_bufferDict.TryRemove(key, out buffer))
        //                buffer.Dispose();
        //        }
        //    }
        //}
        #endregion【缓存服务】
    }
}
