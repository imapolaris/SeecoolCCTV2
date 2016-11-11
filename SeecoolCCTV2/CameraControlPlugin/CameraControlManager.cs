using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlPlugin
{
    public class CameraControlManager
    {
        Dictionary<string, ICameraControl> _cameraControls;
        public readonly static CameraControlManager Instance = new CameraControlManager();
        private CameraControlManager()
        {
            _cameraControls = new Dictionary<string, ICameraControl>();
        }

        public void CameraControl(string videoId, CameraAction action, int actData)
        {
            getOrAdd(videoId)?.Control(action, actData);
        }

        public void TrackTarget(string videoId, string targetUid, double longitude, double latitude, double altitude, double width)
        {
            getOrAdd(videoId)?.TrackTarget(targetUid, longitude, latitude, altitude, width);
        }

        public void StopTrack(string videoId)
        {
            getOrAdd(videoId)?.StopTrack();
        }

        private ICameraControl getOrAdd(string videoId)
        {
            lock (_cameraControls)
            {
                if (!_cameraControls.ContainsKey(videoId))
                    tryAddKey(videoId);
                if (_cameraControls.ContainsKey(videoId))
                    return _cameraControls[videoId];
                return null;
            }
        }

        private void tryAddKey(string videoId)
        {
            var staticInfo = CCTVInfoManager.Instance.GetStaticInfo(videoId);
            ICameraControl cc = null;
            switch (staticInfo.Platform)
            {
                case  CCTVModels.CCTVPlatformType.CCTV1:
                    cc = new CCTV1CameraControl(videoId);
                    break;
                case CCTVModels.CCTVPlatformType.CCTV2:
                    cc = newCCTV2CameraControl(videoId);
                    break;
            }
            if (cc != null)
                _cameraControls.Add(videoId, cc);
        }

        private ICameraControl newCCTV2CameraControl(string videoId)
        {
            var staticInfo = CCTVInfoManager.Instance.GetStaticInfo(videoId);
            var control = CCTVInfoManager.Instance.GetControlConfig(videoId);
            if (control != null && !string.IsNullOrWhiteSpace(control.Ip))
            {
                return new CCTV2CameraControl("127.0.0.1", 8888, videoId);
            }
            return null;
        }
    }
}
