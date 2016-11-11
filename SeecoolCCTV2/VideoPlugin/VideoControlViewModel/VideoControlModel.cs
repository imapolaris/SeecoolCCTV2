using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using VideoNS.VideoInfo;
using VideoNS.VideoDisp;
using VideoNS.VideoControlViewModel;
using VideoNS.SubWindow;
using System.Threading;
using Common.Util;
using CCTVModels;

namespace VideoNS
{
    public class VideoControlModel : ObservableObject, IDisposable
    {
        public VideoControlModel() : this(false)
        {
        }

        public VideoControlModel(bool onEditting)
        {
            initPlayType();
            DisplayModel = new VideoDisplayViewModel();
            RealTimeControl = new RealTimeControlModel() { DisplayModel = this.DisplayModel };
            ReplayControl = new ReplayControlModel() { DisplayModel = this.DisplayModel };
            SnapshotCommand = new DelegateCommand(_ => snapshot());
            SaveVideoCommand = new DelegateCommand(_ => doSaveVideo());
            StopCommand = new DelegateCommand(_ => stop());
            PropertyChanged += onPropertyChanged;
            IsOnEditting = onEditting;
            CloseBtnVisibility = Visibility.Visible;
            FullScreenBtnVisibility = Visibility.Visible;
            DisplayModel.PropertyChanged += displayModel_PropertyChanged;
            ResetStatus();
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VideoId):
                    RealTimeControl.VideoId = VideoId;
                    if (string.IsNullOrWhiteSpace(VideoId))
                    {
                        ResetStatus();
                        stopMonitor();
                    }
                    else
                        startMonitor();
                    DisplayModel.VideoTransform.Reset();
                    break;
                case nameof(IsControlPanelVisible):
                    if (!IsControlPanelVisible)
                    {
                        IsShowPlayType = false;
                        RealTimeControl.IsShowStreamType = false;
                        RealTimeControl.PTZControl.IsVisible = false;
                        RealTimeControl.PresetModel.IsVisible = false;
                        RealTimeControl.SwitchModel.IsVisible = false;
                        ReplayControl.IsShowReplayControl = false;
                    }
                    break;
                case nameof(SelectedDisplayType):
                    IsShowPlayType = false;
                    updateVisibility(SelectedDisplayType.Equals("回看"));
                    break;
                case nameof(IsOnEditting):
                    if (!IsOnEditting)
                    {
                        IsControlPanelVisible = false;
                        IsFullScreen = false;
                        DisplayModel.VideoTransform.Reset();
                    }
                    break;
                case nameof(DisplayModel):
                    ReplayControl.DisplayModel = this.DisplayModel;
                    break;
            }
        }

        private void ResetStatus()
        {
            IsControlPanelVisible = false;
            SelectedDisplayType = "实时";
        }

        #region 【监控视频状态】

        private void displayModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DisplayModel.LastImageTime):
                    this.TooltipVisible = false;
                    break;
            }
        }

        private Thread _videoMonitor;
        private bool _safeStop = false;
        private void startMonitor()
        {
            _safeStop = false;
            if (_videoMonitor != null && _videoMonitor.IsAlive)
                return;
            _videoMonitor = new Thread(monitor);
            _videoMonitor.IsBackground = true;
            _videoMonitor.Start();
        }

        private void monitor()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    if (_safeStop)
                        break;
                    WindowUtil.Invoke(new Action(monitorProcess));
                }
            }
            catch (ThreadAbortException)
            {
                //不处理异常。
            }
            catch (NullReferenceException)
            {
                //在应用程序关闭时的异常。
            }
        }

        private void monitorProcess()
        {
            TimeSpan ts = DateTime.Now - DisplayModel.LastImageTime;
            bool showTooltip = false;
            string tooltip = null;
            if (!string.IsNullOrEmpty(VideoId) && ts.TotalMilliseconds > 30000)
            {
                CCTVOnlineStatus ols = CCTVInfoManager.Instance.GetOnlineStatus(VideoId);
                if (ols != null && !ols.Online)
                    tooltip = "视频断线……";
                else
                    tooltip = "图像传输超时……";
                showTooltip = true;
            }
            //只有在实时监控时显示提示。
            if (showTooltip && RealTimeControl.IsVisible)
            {
                this.ToolTip = tooltip;
                this.TooltipVisible = true;
            }
            else
            {
                this.TooltipVisible = false;
                this.ToolTip = null;
            }
        }

        private void stopMonitor()
        {
            if (_videoMonitor != null)
            {
                _safeStop = true;
                TooltipVisible = false;
                ToolTip = null;
            }
        }
        #endregion 【监控视频状态】

        [AutoNotify]
        public bool TooltipVisible { get; private set; }

        [AutoNotify]
        public string ToolTip { get; private set; }

        [AutoNotify]
        public bool IsVisible { get; set; }
        [AutoNotify]
        public double Opacity { get; set; } = 1;

        [AutoNotify]
        public Visibility CloseBtnVisibility { get; set; }

        [AutoNotify]
        public Visibility FullScreenBtnVisibility { get; set; }

        [AutoNotify]
        public bool IsControlPanelVisible { get; set; }

        [AutoNotify]
        public bool IsOnEditting { get; set; }

        [AutoNotify]
        public string VideoId { get; set; }

        #region 实时回放模式
        [AutoNotify]
        public bool IsShowPlayType { get; set; }

        private void initPlayType()
        {
            DisplayTypeSource = new CollectionViewSource();
            DisplayTypeSource.Source = new List<string>() { "实时", "回看" };
        }

        [AutoNotify]
        public CollectionViewSource DisplayTypeSource { get; set; }

        [AutoNotify]
        public string SelectedDisplayType { get; set; }


        private void updateVisibility(bool isReplayType)
        {
            RealTimeControl.IsVisible = !isReplayType;
            RealTimeControl.VideoInfoMessage.IsViewMessage = false;
            ReplayControl.IsVisible = isReplayType;
        }
        #endregion 实时回放模式

        public ICommand SnapshotCommand { get; set; }

        public Action SnapshotEvent { get; set; }

        private void snapshot()
        {
            //ImageSaver.SavedByHandle(DisplayModel.GetSnapshot());
            var img = DisplayModel.GetSnapshot();
            if (img != null)
            {
                string name = DisplayModel.VideoName;
                if (string.IsNullOrWhiteSpace(name))
                    name = "未知视频";
                PreviewWin.Show(img, name);
                if (SnapshotEvent != null)
                    SnapshotEvent();
            }
            else
                DialogWin.Show("未获取到视频数据。", DialogWinImage.Information);
        }

        public ICommand SaveVideoCommand { get; set; }
        public Action SaveVideoAction;

        private void doSaveVideo()
        {
            //TODO:保存视频录像。
            DialogWin.Show("保存视频录像部分暂未开发！", DialogWinImage.Warning);
            if (SaveVideoAction != null)
                SaveVideoAction();
        }

        [AutoNotify]
        public bool IsFullScreen { get; set; }

        [AutoNotify]
        public ReplayControlModel ReplayControl { get; set; }

        [AutoNotify]
        public RealTimeControlModel RealTimeControl { get; set; }

        public VideoDisplayViewModel DisplayModel { get; set; }

        public ICommand StopCommand { get; set; }

        private void stop()
        {
            VideoId = null;
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool flag)
        {
            if (!_isDisposed)
            {
                //Console.WriteLine("VideoControlModel Dispose:ID+++++++++++:" + VideoId);
                if (DisplayModel != null)
                    DisplayModel.Dispose();
                _safeStop = true;
                _isDisposed = true;
            }
        }

        ~VideoControlModel()
        {
            Dispose(false);
        }
    }
}