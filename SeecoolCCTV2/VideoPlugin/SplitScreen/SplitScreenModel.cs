using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AopUtil.WpfBinding;
using Common.Command;
using VideoNS.AutoSave;
using VideoNS.Helper;
using VideoNS.Json;
using VideoNS.Model;
using VideoNS.SubWindow;
using VideoNS.TimeSwitch;

namespace VideoNS.SplitScreen
{
    public class SplitScreenModel : ObservableObject, IDisposable
    {
        public SplitScreenModel() : this(true)
        {

        }

        public SplitScreenModel(bool onEditing)
        {
            IsOnEditting = onEditing;
            this.PropertyChanged += SplitScreenModel_PropertyChanged;
        }

        private void SplitScreenModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsOnEditting):
                    {
                        if (IsOnEditting)
                            StopSwitchThread();
                        break;
                    }
            }
        }

        [AutoNotify]
        public bool CanItemClose { get; set; } = true;
        [AutoNotify]
        public bool OnSwitching { get; private set; }
        [AutoNotify]
        public bool SwitchPaused { get; set; }
        [AutoNotify]
        public double SwitchProgress { get; private set; }
        [AutoNotify]
        public int SwitchInterval { get; private set; }

        [AutoNotify]
        public bool IsOnEditting
        {
            get;
            set;
        }

        private SplitScreenInfo _ssInfo;
        public SplitScreenInfo SplitScreenData
        {
            get { return _ssInfo; }
            set
            {
                UninstallSebEvent(_ssInfo);
                updateProperty(ref _ssInfo, value);
                InstallSubEvent(_ssInfo);
                OnDataChanged(new EventArgs());
            }
        }

        private void InstallSubEvent(SplitScreenInfo info)
        {
            if (info != null)
                info.DataChanged += Info_DataChanged;
        }

        private void UninstallSebEvent(SplitScreenInfo info)
        {
            if (info != null)
                info.DataChanged -= Info_DataChanged;
        }

        private void Info_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged(e);
        }

        #region 【事件】
        public event EventHandler DataChanged;
        public event EventHandler<PrepareEventArgs> DataPrepare;

        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }

        protected virtual void OnDataPrepare(PrepareEventArgs e)
        {
            if (DataPrepare != null)
                DataPrepare(this, e);
        }
        #endregion 【事件】

        #region 【定时切换执行】
        private SplitScreenInfo _oldInfo;
        private Thread _switchThread;
        public void StartTimingSwitch()
        {
            TimeSwitchInfo[] scheme = TimeSwitchScheme.Instance.Scheme;
            if (scheme != null && scheme.Length > 0)
            {
                StopSwitchThread();
                _oldInfo = SplitScreenData;
                OnSwitching = true;
                _switchThread = new Thread(StartSwitch);
                _switchThread.IsBackground = true;
                _switchThread.Start(scheme);
            }
        }

        public void StopTimingSwitch()
        {
            StopSwitchThread();
            if (OnSwitching)
            {
                OnSwitching = false;
                SwitchPaused = false;
                SplitScreenData = _oldInfo;
            }
        }

        private void StopSwitchThread()
        {
            if (_switchThread != null && _switchThread.IsAlive)
                _switchThread.Abort();
            _switchThread = null;
        }

        private void StartSwitch(object obj)
        {
            try
            {
                TimeSwitchInfo[] scheme = (TimeSwitchInfo[])obj;
                int index = 0;
                do
                {
                    if (Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(new Action<SplitScreenInfo>(InvokeSwitch), scheme[index].Plan);
                    int millis = (int)(scheme[index].StayTime * 1000);
                    if (millis < 1000)
                        millis = 1000;
                    if (Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(new Action<int>(InvokeInterval), millis);
                    int prog = 0;
                    bool prepared = false;
                    while (true)
                    {
                        Thread.Sleep(50);

                        if (!SwitchPaused)
                        {
                            prog += 50;
                            if (Application.Current != null)
                                Application.Current.Dispatcher.BeginInvoke(new Action<double>(InvokeProgress), prog / (double)millis);
                        }
                        if (prog >= millis)
                            break;
                        else if (!prepared && millis - prog <= 5000)
                        {
                            //准备数据，预加载
                            prepared = true;
                            int preIndex = index + 1;
                            if (preIndex >= scheme.Length)
                                preIndex = 0;
                            if (Application.Current != null)
                                Application.Current.Dispatcher.BeginInvoke(new Action<SplitScreenInfo>(InvokePrepare), scheme[preIndex].Plan);
                        }
                    }

                    index++;
                    if (index >= scheme.Length)
                        index = 0;
                }
                while (true);
            }
            catch (ThreadAbortException)
            {

            }
        }

        private void InvokePrepare(SplitScreenInfo info)
        {
            if (SplitScreenData != info)
            {
                OnDataPrepare(new PrepareEventArgs(info));
            }
        }

        private void InvokeSwitch(SplitScreenInfo info)
        {
            if (SplitScreenData != info)
                SplitScreenData = info;
        }

        private void InvokeProgress(double prog)
        {
            SwitchProgress = prog;
        }

        private void InvokeInterval(int interval)
        {
            SwitchInterval = interval;
        }
        #endregion 【定时切换执行】

        #region 【实现IDisposable接口】
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            StopTimingSwitch();
        }

        ~SplitScreenModel()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】

        public class PrepareEventArgs : EventArgs
        {
            public SplitScreenInfo PrepareData { get; private set; }
            public PrepareEventArgs(SplitScreenInfo info)
            {
                PrepareData = info;
            }
        }
    }
}
