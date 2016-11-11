using Common.Util;
using PluginBaseLib.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlPlugin
{
    internal class VideoInfoPlugin : ViewModelPlugin
    {
        [Export(InitializationHelper.InitializationFlag)]
        void init()
        {
            CCTVInfoManager.Instance.Init();
            PanTiltControlManager.Instance.Init();
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
