using CCTVClient;
using Common.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlPlugin
{
    public class PanTiltControlManager
    {
        public readonly static PanTiltControlManager Instance = new PanTiltControlManager();

        public void Init()
        {
            
        }

        CameraControlManager _control { get { return CameraControlManager.Instance; } }
        CCTVInfo _info { get { return CCTVInfoManager.Instance.CCTV1Info; } }

        private PanTiltControlManager()
        {
            RemoteCalls.Global.Register<string, string, double, double, double, double>("CCTV2_VideoPlugin_TrackTarget", TrackTarget);
            RemoteCalls.Global.Register<string>("CCTV2_VideoPlugin_StopTrack", StopTrack);
            RemoteCalls.Global.Register<string, string, int>("CCTV2_VideoPlugin_CameraControl", CameraControl);
            RemoteCalls.Global.Register<string, Action<int, int>>("CCTV2_VideoPlugin_SubscribeSwitchStatus", SubscribeSwitchStatus);
            RemoteCalls.Global.Register<string, Action<int, int>>("CCTV2_VideoPlugin_UnsubscribeSwitchStatus", UnsubscribeSwitchStatus);
        }

        public void TrackTarget(string videoId, string targetUid, double longitude, double latitude, double altitude, double width)
        {
            _control.TrackTarget(videoId, targetUid, longitude, latitude, altitude, width);
        }

        public void StopTrack(string videoId)
        {
            _control.StopTrack(videoId);
        }

        public void CameraControl(string videoId, string action, int actData)
        {
            CameraAction act = CameraAction.StopPT;
            if (Enum.TryParse<CameraAction>(action, out act))
                _control.CameraControl(videoId, act, actData);
        }

        public void CameraControl(string videoId, CameraAction action, int actData)
        {
            _control.CameraControl(videoId, action, actData);
        }

        class Events
        {
            public Events(string videoId)
            {
                VideoId = videoId;
            }

            public string VideoId { get; private set; }

            public event Action<int, int> SwitchStatusEvent;
            public void FireSwitchStatusEvent(int index, int status)
            {
                var callback = SwitchStatusEvent;
                if (callback != null)
                    callback(index, status);
            }

            public bool IsSwitchStatusEventNull()
            {
                return SwitchStatusEvent == null;
            }
        }

        ConcurrentDictionary<string, Events> _eventsDict = new ConcurrentDictionary<string, Events>();

        public void SubscribeSwitchStatus(string videoId, Action<int, int> onSwitchStatus)
        {
            Events events = _eventsDict.GetOrAdd(videoId, x => new Events(x));
            events.SwitchStatusEvent += onSwitchStatus;

            ulong cctv1VideoId = CCTV1CameraControl.ToCCTV1Id(videoId);
            checkVideoInfo();
            _info.StartCtrl(cctv1VideoId, TimeSpan.FromSeconds(30));
        }

        public void UnsubscribeSwitchStatus(string videoId, Action<int, int> onSwitchStatus)
        {
            Events events = null;
            if (_eventsDict.TryGetValue(videoId, out events))
            {
                events.SwitchStatusEvent -= onSwitchStatus;

                if (events.IsSwitchStatusEventNull())
                {
                    ulong cctv1VideoId = CCTV1CameraControl.ToCCTV1Id(videoId);
                    _info.EndCtrl(cctv1VideoId);
                }
            }
        }

        CCTVInfo _lastInfo = null;
        private void checkVideoInfo()
        {
            CCTVInfo info = _info;
            if (info != _lastInfo)
            {
                if (_lastInfo != null)
                    _lastInfo.SwitchStatusEvent -= info_SwitchStatusEvent;
                _lastInfo = info;

                info.SwitchStatusEvent += info_SwitchStatusEvent;
            }
        }

        private void info_SwitchStatusEvent(ulong cctv1VideoId, int index, int status)
        {
            //Console.WriteLine($"switch:{index}, {status}");
            string videoId = CCTV1CameraControl.FromCCTV1Id(cctv1VideoId);
            Events events = null;
            if (_eventsDict.TryGetValue(videoId, out events))
                events.FireSwitchStatusEvent(index, status);
        }
    }
}
