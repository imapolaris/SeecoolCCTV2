using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient;

namespace CenterStorageService
{
    public class VideoStorager: IDisposable
    {
        public string VideoId_StreamId { get; private set; }
        VideoSourceCmd _videoSourceCmd;
        SyncRecorder _syncRec;
        public VideoStorager(string videoId, int streamId)
        {
            VideoId_StreamId = $"{videoId}_{streamId}";
            string path = System.IO.Path.Combine(GlobalData.Path, VideoId_StreamId);
            _syncRec = new SyncRecorder(path);
            VideoSource source = VideoSourcesCmd.Instance.GetVideoSource(videoId, streamId);
            _videoSourceCmd = new  VideoSourceCmd(source);
            _videoSourceCmd.VideoDisplayEvent += _syncRec.Set;
        }

        public void Dispose()
        {
            if (_videoSourceCmd != null)
            {
                _videoSourceCmd.VideoDisplayEvent -= _syncRec.Set;
                _videoSourceCmd.Dispose();
            }
            _videoSourceCmd = null;
            _syncRec.Dispose();
        }
    }
}
