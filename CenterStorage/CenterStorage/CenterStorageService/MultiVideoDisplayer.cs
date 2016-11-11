using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;

namespace CenterStorageService
{
    public class MultiVideoDisplayer: IDisposable
    {
        List<VideoStorager> _videoDisps = new List<VideoStorager>();
        object _lockObj = new object();
        public int Count { get { lock(_lockObj) { return _videoDisps.Count; } } }
        public void Add(string videoId, int streamId)
        {
            lock(_lockObj)
            {
                if (!isExist(videoId, streamId))
                {
                    try
                    {
                        _videoDisps.Add(new VideoStorager(videoId, streamId));
                    }
                    catch(NullReferenceException ex)
                    {
                        Logger.Default.Trace("获取{0}_{1}视频流失败！{2}", videoId, streamId, ex);
                    }
                }
            }
        }

        public void Remove(string videoId, int streamId)
        {
            lock(_lockObj)
            {
                var disp = get(videoId, streamId);
                if (disp != null)
                {
                    _videoDisps.Remove(disp);
                    disp.Dispose();
                }
            }
        }

        public bool Exist(string videoId, int streamId)
        {
            lock(_lockObj)
            {
                return isExist(videoId, streamId);
            }
        }

        bool isExist(string videoId, int streamId)
        {
            return get(videoId, streamId) != null;
        }

        private VideoStorager get(string videoId, int streamId)
        {
            string videoId_streamId = $"{videoId}_{streamId}";
            return _videoDisps.FirstOrDefault(_ => _.VideoId_StreamId == videoId_streamId);
        }

        public void Dispose()
        {
            lock(_lockObj)
            {
                foreach (var disp in _videoDisps)
                    disp.Dispose();
                _videoDisps.Clear();
            }
        }
    }
}
