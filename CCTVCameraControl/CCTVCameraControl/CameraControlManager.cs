using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVCameraControl.Base;
using CCTVClient;
using CCTVInfoHub;

namespace CCTVCameraControl
{
    internal class CameraControlManager
    {
        Dictionary<string, ICameraControl> _cameraControls;
        CCTVDefaultInfoSync _infoSync;
        public CCTVInfo CCTV1Info { get; set; }
        public CameraControlManager(CCTVDefaultInfoSync infoSync, CCTVInfo cctv1Info)
        {
            _infoSync = infoSync;
            CCTV1Info = cctv1Info;
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
            var staticInfo = _infoSync.GetStaticInfo(videoId);
            ICameraControl cc = null;
            switch (staticInfo.Platform)
            {
                case CCTVModels.CCTVPlatformType.CCTV1:
                    cc = new CCTV1CameraControl(videoId, CCTV1Info);
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
            var staticInfo = _infoSync.GetStaticInfo(videoId);
            var control = _infoSync.GetControlConfig(videoId);
            if (control != null && !string.IsNullOrWhiteSpace(control.Ip))
            {
                return new CCTV2CameraControl("127.0.0.1", 8888, videoId);
            }
            return null;
        }
    }
}
