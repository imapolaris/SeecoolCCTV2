using Common.Command;
using Common.Message;
using Common.Object;
using PluginBaseLib.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Telerik.Windows.Controls.Docking;
using UIManipulator;
using WindowDispatcher.DispatcherLib;
using VideoNS.SplitScreen;
using VideoNS.Model;
using VideoNS.Json;
using VideoNS.Helper;
using System.IO;
using VideoNS.AutoSave;

namespace VideoNS
{
    public class VideoPlugin : ViewModelPlugin
    {
        public override void Init()
        {
            base.Init();

            VideoControlManager.Instance.Init();
            CCTVInfoManager.Instance.Init();
            CameraControlRemoteCall.Instance.Init();

            RemoteEvents.Global.Subscribe("CCTV2_LayoutPlugin_RegisteredUIInited", initUI);

            UIService.SubscribeExitPoller(onUIExit);
        }

        public override void Exit()
        {
            base.Exit();
        }

        private bool onUIExit()
        {
            return false;
        }

        private void initUI()
        {
            addSplitScreenPanel();
        }

        private void addSplitScreenPanel()
        {
            SplitScreenControl panel = new SplitScreenControl();
            RemoteCalls.Global.Call("CCTV2_LayoutPlugin_SetPane", panel);
        }
    }
}
