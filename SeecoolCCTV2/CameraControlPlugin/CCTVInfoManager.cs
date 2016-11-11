using CCTVClient;
using Common.Configuration;
using Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;
using CCTVInfoHub;
using CCTVInfoHub.Entity;

namespace CameraControlPlugin
{
    public class CCTVInfoManager
    {
        public readonly static CCTVInfoManager Instance = new CCTVInfoManager();

        public void Init()
        {
        }

        private CCTVInfoManager()
        {
            init();
        }

        public CCTVGlobalInfo GetGlobalInfo()
        {
            return ClientHub.GetGlobalInfo();
        }

        private int _lastSITick;
        public CCTVStaticInfo GetStaticInfo(string videoId)
        {
            if (Environment.TickCount - _lastSITick > 60000)
            {
                _lastSITick = Environment.TickCount;
                ClientHub.UpdateDefault(CCTVInfoType.StaticInfo);
            }
            return ClientHub.GetStaticInfo(videoId);
        }

        private int _lastCCTick;
        public CCTVControlConfig GetControlConfig(string videoId)
        {
            if (Environment.TickCount - _lastCCTick > 60000)
            {
                _lastCCTick = Environment.TickCount;
                ClientHub.UpdateDefault(CCTVInfoType.ControlConfig);
            }
            return ClientHub.GetControlConfig(videoId);
        }

        CCTVGlobalInfo _globalInfo = null;
        public CCTVInfo CCTV1Info { get; set; }
        CCTVDefaultInfoSync ClientHub { get; set; }

        private void init()
        {
            string webApiBaseUri = ConfigHandler.GetValue<VideoInfoPlugin>("WebApiBaseUri");
            ClientHub = new CCTVDefaultInfoSync(webApiBaseUri);
            ClientHub.RegisterDefault(CCTVInfoType.GlobalInfo, TimeSpan.Zero, onGlobalInfoUpdate);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.StaticInfo);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.ControlConfig);
            _lastSITick = _lastCCTick = Environment.TickCount;
            ClientHub.UpdateAllDefault();
        }

        private void onGlobalInfoUpdate(IEnumerable<string> keysUpdated)
        {
            _globalInfo = GetGlobalInfo();
            if (_globalInfo != null)
            {
                if (CCTV1Info == null || CCTV1Info.ServerHost != _globalInfo.CCTV1Host)
                {
                    if (CCTV1Info != null)
                        CCTV1Info.Stop();
                    CCTV1Info = new CCTVInfo(_globalInfo.CCTV1Host);
                    CCTV1Info.Start();
                }
            }
        }
    }
}
