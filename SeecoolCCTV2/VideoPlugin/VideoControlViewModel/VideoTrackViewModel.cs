using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VideoNS.VideoTrack;
using VideoTrackingCmd;

namespace VideoNS.VideoControlViewModel
{
    public class VideoTrackViewModel : ObservableObject, IDisposable
    {
        [AutoNotify]
        public bool CanBeTracked { get; set; }
        [AutoNotify]
        public bool IsVisible { get; set; }

        [AutoNotify]
        public double Zoom { get; set; }

        [AutoNotify]
        public bool IsZoomMax { get; set; }

        [AutoNotify]
        public bool IsAutoZoom { get; set; }

        [AutoNotify]
        public bool IsSetPointStatus { get; set; }

        [AutoNotify]
        public bool IsTracking { get; set; }

        [AutoNotify]
        public string Status { get; set; }
        [AutoNotify]
        public string StatusColor { get; set; } = "White";

        [AutoNotify]
        public bool IsCanReset { get; set; }
        public Action<TrackRect> TrackRectEvent;
        bool _stopTrackIfStopControl;
        DateTime _startTrack = DateTime.MinValue;

        TrackingDataSwapClient _swapClient { get; set; }
        string _videoId;

        public VideoTrackViewModel(string videoId)
        {
            StopTracking = new DelegateCommand(_ => stopTrack());
            ResetTracking = new DelegateCommand(_ => resetTrack());
            if (!string.IsNullOrWhiteSpace(videoId))
            {
                PropertyChanged += onPropertyChanged;
            }
            LoadTrackSwap(videoId);
        }

        public void LoadTrackSwap(string videoId)
        {
            _videoId = videoId;
            if (!string.IsNullOrWhiteSpace(videoId))
            {
                var staticInfo = CCTVInfoManager.Instance.GetStaticInfo(videoId);
                var vTrack = CCTVInfoManager.Instance.GetVideoTrack(videoId);
                CanBeTracked = (staticInfo != null && vTrack != null);
            }
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsVisible):
                    updateSwap();
                    break;
                case nameof(IsAutoZoom):
                    setTrackModel(IsAutoZoom);
                    break;
            }
        }

        private void updateSwap()
        {
            if (IsVisible)
                new System.Threading.Thread(loadTrackSwap).Start();
            else
                disposeTrackSwap();
            Console.WriteLine("UpdatedSwap!");
        }

        private void loadTrackSwap()
        {
            StatusColor = "Yellow";
            Status = "正在连接视频跟踪服务……";
            disposeTrackSwap();
            try
            {
                var onlineStatus = CCTVInfoManager.Instance.GetOnlineStatus(_videoId);
                var vTrack = CCTVInfoManager.Instance.GetVideoTrack(_videoId);
                if (onlineStatus != null && onlineStatus.Online)
                {
                    var staticInfo = CCTVInfoManager.Instance.GetStaticInfo(_videoId);
                    if (staticInfo != null && vTrack != null && vTrack.Ip != null)
                    {
                        _swapClient = new TrackingDataSwapClient(vTrack.Ip, "8061", "8068");
                        _swapClient.TrackingStatusEvent += onStatus;
                        IsAutoZoom = _swapClient.IsAutoZoom;
                        Status = "";
                    }
                }
                else
                    IsVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                StatusColor = "Red";
                Status = "视频跟踪服务连接失败！";
                //Status = "";
                //IsVisible = false;
            }
        }

        void disposeTrackSwap()
        {
            if (_swapClient != null)
            {
                updateTrackStatusWhenDispose();
                _swapClient.TrackingStatusEvent -= onStatus;
                _swapClient.Dispose();
            }
            _swapClient = null;
        }

        private void updateTrackStatusWhenDispose()
        {
            if (_stopTrackIfStopControl)
            {
                resetTrack();
                updateTrackRect(new TrackRect());
            }
        }
        #region 状态检测
        private void onStatus(TrackingStatusEnum statusEnum, string statusData)
        {
            switch (statusEnum)
            {
                case TrackingStatusEnum.Error:
                    updateWorkStatus(statusData);
                    break;
                case TrackingStatusEnum.Warn:
                    updateWorkStatus(statusData);
                    break;
                case TrackingStatusEnum.Rect:
                    updateTrackRect(statusData);
                    break;
                case TrackingStatusEnum.AutoZoom:
                    IsAutoZoom = bool.Parse(statusData);
                    break;
                case TrackingStatusEnum.ZoomAndStatus:
                    updateZoomAndStatus(statusData);
                    break;
                case TrackingStatusEnum.Info:
                    Console.WriteLine("{0}: Info {1}", DateTime.Now.TimeOfDay, statusData);
                    updateWorkStatus(statusData);
                    break;
                case TrackingStatusEnum.Status:
                    updateTrackingStatus(statusData);
                    break;
                case TrackingStatusEnum.Debug:
                    break;
                case TrackingStatusEnum.None:
                    updateWorkStatus(statusData);
                    break;
                case TrackingStatusEnum.Other:
                    Console.WriteLine(DateTime.Now + ": " + statusData);
                    break;
            }
        }

        private void updateZoomAndStatus(string statusData)
        {
            try
            {
                char[] separator = new char[] { ',' };
                string[] strList = statusData.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (strList != null && strList.Length >= 3)
                {
                    IsZoomMax = bool.Parse(strList[0]);
                    Zoom = double.Parse(strList[1]);
                    IsTracking = bool.Parse(strList[2]);
                    updateTrackStatus();
                    updateResetStatus();
                }
            }
            catch { }
        }

        void updateTrackStatus()
        {
            if (!IsTracking)
            {
                if (_stopTrackIfStopControl && DateTime.Now - _startTrack > TimeSpan.FromMilliseconds(2000))
                    _stopTrackIfStopControl = false;
            }
        }

        void updateResetStatus()
        {
            if (IsTracking || Zoom != 1)
                IsCanReset = true;
            else
                IsCanReset = false;
        }

        public ICommand StopTracking { get; set; }

        private void stopTrack()
        {
            if (_swapClient != null)
                _swapClient.StopTrack();
            _stopTrackIfStopControl = false;
        }

        public ICommand ResetTracking { get; set; }

        private void resetTrack()
        {
            if (_swapClient != null)
                _swapClient.ResetPtz();
            _stopTrackIfStopControl = false;
        }

        private void updateTrackRect(string data)
        {
            updateTrackRect(TrackRect.GetTrackRect(data));
        }
        private void updateTrackRect(TrackRect rect)
        {
            if (TrackRectEvent != null)
                TrackRectEvent(rect);
        }

        private void updateWorkStatus(string status)
        {
        }

        private void updateTrackingStatus(string info)
        {
        }
        #endregion 状态检测

        public void Dispose()
        {
            IsVisible = false;
            CanBeTracked = false;
            disposeTrackSwap();
        }

        public void SetZoomScale(double scale)
        {
            if (_swapClient != null)
            {
                _swapClient.StopDrag();
                _swapClient.SetZoomScale(scale);
            }
        }

        public void StartDrag(double x, double y)
        {
            _swapClient?.StartDrag(x, y);
        }

        public void Draging(double x, double y)
        {
            _swapClient?.Draging(x, y);
        }

        public void StopDrag()
        {
            _swapClient?.StopDrag();
        }

        public void StartTrackFromPoint(double x, double y)
        {
            try
            {
                if (_swapClient != null)
                {
                    if (!_swapClient.IsTracking && _swapClient.Zoom <= 1.2)
                        IsAutoZoom = true;
                    _swapClient.StartTrackFromPoint(new System.Drawing.PointF((float)x, (float)y));
                    _stopTrackIfStopControl = true;
                    _startTrack = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception: " + ex.ToString());
            }
        }

        private void setTrackModel(bool isAutoZoom)
        {
            if (_swapClient != null)
                _swapClient.IsAutoZoom = isAutoZoom;
        }
    }
}