using CCTVClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlPlugin
{
    public class CCTV1CameraControl : ICameraControl
    {
        const string _cctv1Prefix = "CCTV1_";
        ulong _videoId;
        CCTVInfo _info { get { return CCTVInfoManager.Instance.CCTV1Info; } }

        public CCTV1CameraControl(ulong videoId)
        {
            _videoId = videoId;
        }
        public CCTV1CameraControl(string videoId)
            :this(ToCCTV1Id(videoId))
        {
        }

        public void Control(CameraAction action, int actData)
        {
            CCTVInfo.CameraAction act = CCTVInfo.CameraAction.Stop;
            int speed = 0;
            int aux = 0;
            switch (action)
            {
                case CameraAction.StopPT:
                case CameraAction.StopZoom:
                case CameraAction.StopFocus:
                case CameraAction.StopIris:
                    act = CCTVInfo.CameraAction.Stop;
                    break;
                case CameraAction.Up:
                case CameraAction.Down:
                case CameraAction.Left:
                case CameraAction.Right:
                case CameraAction.LeftUp:
                case CameraAction.LeftDown:
                case CameraAction.RightUp:
                case CameraAction.RightDown:
                case CameraAction.AutoScan:
                    act = (CCTVInfo.CameraAction)(int)action;
                    speed = actData;
                    break;
                case CameraAction.AuxOn:
                    act = CCTVInfo.CameraAction.AuxOn;
                    aux = actData;
                    break;
                case CameraAction.AuxOff:
                    act = CCTVInfo.CameraAction.AuxOff;
                    aux = actData;
                    break;
                case CameraAction.ZoomIn:
                    act = CCTVInfo.CameraAction.ZoomWide;
                    break;
                case CameraAction.ZoomOut:
                    act = CCTVInfo.CameraAction.ZoomTele;
                    break;
                case CameraAction.FocusNear:
                    act = CCTVInfo.CameraAction.FocusNear;
                    break;
                case CameraAction.FocusFar:
                    act = CCTVInfo.CameraAction.FocusFar;
                    break;
                case CameraAction.IrisOpen:
                    act = CCTVInfo.CameraAction.IrisOpen;
                    break;
                case CameraAction.IrisClose:
                    act = CCTVInfo.CameraAction.IrisClose;
                    break;
                case CameraAction.GoPreset:
                    act = CCTVInfo.CameraAction.GoPreset;
                    aux = actData;
                    break;
                case CameraAction.SetPreset:
                    act = CCTVInfo.CameraAction.SetPreset;
                    aux = actData;
                    break;
                default:
                    return;
            }
            _info.CameraControl(_videoId, act, speed, aux);
        }

        string _lastTargetUid = null;
        bool _zoomedId = false;
        public void TrackTarget(string targetUid, double longitude, double latitude, double altitude, double width)
        {
            bool zoomControl = updateZoomCantrol(targetUid, width);
            int targetWidth = (int)Math.Round(width);
            int firstWidth = zoomControl ? targetWidth : 0;
            _info.TrackTarget(_videoId, "", targetUid, longitude, latitude, altitude, firstWidth, targetWidth);
        }

        private bool updateZoomCantrol(string targetUid, double width)
        {
            bool zoom = false;
            if (_lastTargetUid != targetUid)
            {
                _zoomedId = false;
                _lastTargetUid = targetUid;
            }
            if (!_zoomedId && width > 0)
            {
                _zoomedId = true;
                zoom = true;
            }
            return zoom;
        }

        public void StopTrack()
        {
            _info.StopTrack(_videoId);
        }

        public static ulong ToCCTV1Id(string id)
        {
            ulong cctv1Id = 0;
            if (!string.IsNullOrEmpty(id) && id.Length > _cctv1Prefix.Length && id.Substring(0, _cctv1Prefix.Length) == _cctv1Prefix)
            {
                string idString = id.Substring(_cctv1Prefix.Length);
                ulong.TryParse(idString, NumberStyles.HexNumber, null, out cctv1Id);
            }
            return cctv1Id;
        }
        
        public static string FromCCTV1Id(ulong cctv1Id)
        {
            return _cctv1Prefix + cctv1Id.ToString("X");
        }
    }
}
