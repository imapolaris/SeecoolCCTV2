using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient;

namespace GatewayNet.Tools
{
    public class VideoSourceCreator
    {
        public static VideoSourceCreator Instance { get; private set; }
        static VideoSourceCreator()
        {
            Instance = new VideoSourceCreator();
        }

        private VideoSourceManager _vsMgr;
        private VideoSourceCreator()
        {
            _vsMgr = VideoSourceManager.CreateInstance(InfoService.Instance.ClientHub);
        }

        public VideoSource GetVideoSource(string videoId)
        {
            return _vsMgr.GetVideoSource(videoId);
        }
    }
}
