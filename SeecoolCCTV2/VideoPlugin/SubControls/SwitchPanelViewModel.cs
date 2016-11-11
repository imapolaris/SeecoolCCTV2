using AopUtil.WpfBinding;
using CCTVClient;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CCTVModels;

namespace VideoNS.SubControls
{
    public class SwitchPanelViewModel : ObservableObject, IDisposable
    {
        private bool _hasSubscribed = false;
        public SwitchPanelViewModel()
        {
            AllSwitchStatus = new ObservableCollection<SwitchStatus>();
            SwitchClickCmd = new DelegateCommand(doSwitchClickCmd);
            PropertyChanged += thisPropertyChanged;
        }

        private void thisPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsVisible):
                    if (IsVisible)
                        subscribe(VideoId);
                    else
                        unsubscribe(VideoId);
                    break;
            }
        }

        private string _videoId;
        public string VideoId
        {
            get { return _videoId; }
            set
            {
                unsubscribe(_videoId);
                updateProperty(ref _videoId, value);
            }
        }
        [AutoNotify]
        public bool IsVisible { get; set; }
        [AutoNotify]
        public ObservableCollection<SwitchStatus> AllSwitchStatus { get; private set; }
        [AutoNotify]
        public ICommand SwitchClickCmd { get; private set; }

        private void doSwitchClickCmd(object obj)
        {
            SwitchStatus ss = obj as SwitchStatus;
            if (ss != null && ss.Index >= 0)
            {
                var onOff = ss.IsSwitchOn ? "AuxOff" : "AuxOn";
                CameraControlRemoteCall.Instance.CameraControl(VideoId, onOff, ss.Index);
            }
        }

        private void switchStatusChanged(int index, int ooff)
        {
            SwitchStatus ss = AllSwitchStatus.FirstOrDefault(s => s.Index == index);
            if (ss != null)
            {
                ss.IsSwitchOn = ooff == 0 ? false : true;
            }
        }

        private void subscribe(string vId)
        {
            if (vId != null && !_hasSubscribed)
            {
                CameraControlRemoteCall.Instance.SubscribeSwitchStatus(vId, new Action<int, int>(switchStatusChanged));
                _hasSubscribed = true;
            }
        }
        private void unsubscribe(string vId)
        {
            if (vId != null && _hasSubscribed)
            {
                CameraControlRemoteCall.Instance.UnsubscribeSwitchStatus(vId, new Action<int, int>(switchStatusChanged));
                _hasSubscribed = false;
            }
        }

        public void UpdateSwitchInfo()
        {
            if (!string.IsNullOrWhiteSpace(VideoId))
            {
                var control = CCTVInfoManager.Instance.GetControlConfig(VideoId);
                if (control != null && control.AuxSwitch != null)
                {
                    SwitchInfo[] infos = control.AuxSwitch;
                    AllSwitchStatus.Clear();
                    for (int i = 0; i < infos.Length; i++)
                    {
                        AllSwitchStatus.Add(new SwitchStatus()
                        {
                            Name = infos[i].Name,
                            Index = infos[i].Index,
                            IsEnable = true,
                            IsSwitchOn = false
                        });
                    }
                    OnSwitchesUpdated();
                }
            }
        }

        #region 【事件】
        public event EventHandler SwitchesUpdated;

        private void OnSwitchesUpdated()
        {
            EventHandler handler = SwitchesUpdated;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件】

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            unsubscribe(VideoId);
        }

        ~SwitchPanelViewModel()
        {
            Dispose(false);
        }

        public class SwitchStatus : ObservableObject
        {
            [AutoNotify]
            public string Name { get; set; }
            [AutoNotify]
            public int Index { get; set; }
            [AutoNotify]
            public bool IsEnable { get; set; }
            [AutoNotify]
            public bool IsSwitchOn { get; set; }
        }
    }
}
