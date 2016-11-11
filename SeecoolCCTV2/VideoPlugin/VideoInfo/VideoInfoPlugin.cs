using Common.Util;
using PluginBaseLib.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.Thumbnail;
using VideoNS.VideoDistribute;

namespace VideoNS.VideoInfo
{
    public class VideoInfoPlugin : ViewModelPlugin
    {
        [Export(InitializationHelper.InitializationFlag)]
        void init()
        {
            ThumbnailsPack.Instance.Init();
            //ThumbnailManager.Default.Init();
            //VideoSourceManager.Instance.Init();
            VideoDataManager.Instance.Init();
            VideoBufferManager.Instance.Init();
        }

        public override void Init()
        {
            base.Init();

            init();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
