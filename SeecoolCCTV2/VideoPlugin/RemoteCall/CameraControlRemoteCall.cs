using Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVCameraControl.RemoteControl;
using CCTVCameraControl.Base;

namespace VideoNS
{
    public class CameraControlRemoteCall
    {
        public readonly static CameraControlRemoteCall Instance = new CameraControlRemoteCall();

        public void Init()
        {

        }

        private CameraControlRemoteCall()
        {
            RemoteCalls.Global.Register<string, string, double, double, double, double>("CCTV2_VideoPlugin_TrackTarget", TrackTarget);
            RemoteCalls.Global.Register<string>("CCTV2_VideoPlugin_StopTrack", StopTrack);
            RemoteCalls.Global.Register<string, string, int>("CCTV2_VideoPlugin_CameraControl", CameraControl);
            RemoteCalls.Global.Register<string, Action<int, int>>("CCTV2_VideoPlugin_SubscribeSwitchStatus", SubscribeSwitchStatus);
            RemoteCalls.Global.Register<string, Action<int, int>>("CCTV2_VideoPlugin_UnsubscribeSwitchStatus", UnsubscribeSwitchStatus);
        }

        public void TrackTarget(string videoId, string targetUid, double longitude, double latitude, double altitude, double width)
        {
            CCTVInfoManager.Instance.CameraControl.TrackTarget(videoId, targetUid, longitude, latitude, altitude, width);
        }

        public void StopTrack(string videoId)
        {
            CCTVInfoManager.Instance.CameraControl.StopTrack(videoId);
        }

        public void CameraControl(string videoId, string action, int actData)
        {
            CameraAction act = CameraAction.StopPT;
            if (Enum.TryParse<CameraAction>(action, out act))
            {
                Common.Log.Logger.Default.Trace($"VideoPlugin 操作 {videoId}: {act} {actData}");
                CCTVInfoManager.Instance.CameraControl.CameraControl(videoId, act, actData);
            }
        }

        public void CameraControl(string videoId, CameraAction action, int actData)
        {
            CCTVInfoManager.Instance.CameraControl.CameraControl(videoId, action, actData);
        }

        public void SubscribeSwitchStatus(string videoId, Action<int, int> onSwitchStatus)
        {
            CCTVInfoManager.Instance.CameraControl.SubscribeSwitchStatus(videoId, onSwitchStatus);
        }

        public void UnsubscribeSwitchStatus(string videoId, Action<int, int> onSwitchStatus)
        {
            CCTVInfoManager.Instance.CameraControl.UnsubscribeSwitchStatus(videoId, onSwitchStatus);
        }
    }
}
