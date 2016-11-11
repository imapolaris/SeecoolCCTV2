using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient;

namespace CenterStorageService
{
    public class VideoSourcesCmd
    {
        public static VideoSourcesCmd Instance { get; private set; }
        static VideoSourcesCmd()
        {
            Instance = new VideoSourcesCmd();
        }

        VideoStreamClient.VideoSourceManager _manager;
        private VideoSourcesCmd()
        {
            _manager = new VideoSourceManager(GlobalData.StaticInfoAddress);
            System.Threading.Thread.Sleep(1000);
        }


        public VideoSource GetVideoSource(string videoId, int streamId)
        {
            return _manager.GetVideoSource(videoId, streamId);
        }
    }
}
