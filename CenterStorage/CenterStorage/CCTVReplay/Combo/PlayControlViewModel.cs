using AopUtil.WpfBinding;
using CCTVReplay.Util;
using CCTVReplay.Video;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using CCTVReplay.StaticInfo;
using CCTVReplay.Source;
using Common.Util;

namespace CCTVReplay.Combo
{
    public class PlayControlViewModel : ObservableObject
    {
        private Dictionary<string, VideoControlViewModel> _dictVideoVMS;
        private object _videoLockObj = new object();
        ReplayProcessRate _replayProcess;
        public PlaySliderViewModel PlaySlider { get; private set; }
        public PlayControlViewModel()
        {
            _replayProcess = new ReplayProcessRate(DateTime.MinValue);
            _replayProcess.FastTimes = 4;
            _dictVideoVMS = new Dictionary<string, VideoControlViewModel>();
            StopCmd = new CommandDelegate(_ => Stop());
            DownloadCmd = new CommandDelegate(_ => doDownloadCmd());
            SnapshotCmd = new CommandDelegate(_ => doSnapshotCmd());
            PropertyChanged += onPropertyChanged;

            SpeedType = new CollectionViewSource();
            SpeedType.Source = VideoPlaySpeedManager.SpeedSources();
            SelectedSpeedType = "正常";

            VideoInfoManager.Instance.DataSourceChanged += onDataSourceChanged;
            VideoInfoManager.Instance.LocalSourceInfoReceived += onLocalSourceInfoReceived;
            PlaySlider = new PlaySliderViewModel(_replayProcess);
            PlaySlider.ProgressOffsetEvent += onProgressOffset;
            PlaySlider.JumpEvent += jump;
        }

        private void onProgressOffset()
        {
            lock (_videoLockObj)
            {
                //更新单视频进度时间。
                if (_dictVideoVMS.Values.Count > 0)
                {
                    foreach (VideoControlViewModel vm in _dictVideoVMS.Values)
                        vm.ProgressOffset = PlaySlider.Slider / (double)PlaySlider.SliderMaximum;
                }
                else
                    PlaySlider.Slider = 0;
            }
        }

        private void onLocalSourceInfoReceived(LocalVideosInfoPacket obj)
        {
            WindowUtil.BeginInvoke(() =>
            {
                UpdateTimePeriod(obj.TimePeriod.BeginTime, obj.TimePeriod.EndTime);
            });
        }

        private void onDataSourceChanged(object sender, EventArgs e)
        {
            VideoDataSource src = VideoInfoManager.Instance.GetStorageSource();

            if (src == null)
                throw new ErrorMessageException("未配置有效的数据源！");
            if (src.SrcType == SourceType.Local && (string.IsNullOrEmpty(src.LocalSourcePath) || !new System.IO.DirectoryInfo(src.LocalSourcePath).Exists))
                throw new ErrorMessageException("未设置有效的本地视频路径！");
            if (src.SrcType == SourceType.Remote && src.Storage == null)
                throw new ErrorMessageException("未配置集中存储服务!");

            //设置集中存储数据源
            UpdateSource(src);
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsPlaying):
                    //Console.WriteLine("IsPlaying: " + IsPlaying);
                    //if (IsPlaying && _dictVideoVMS.Count == 0)
                    //    IsPlaying = false;
                    //else
                    _replayProcess.Playing = IsPlaying;
                    break;
                case nameof(SelectedSpeedType):
                        updateSpeed();
                    break;
                case nameof(IsFullScreen):
                    doFullscreenCmd();
                    break;
            }
        }

        public IEnumerable<IVideoInfo> GetPlayingVideos()
        {
            return _dictVideoVMS.Values.Select(x => x.VideoInfo);
        }

        public VideoDataSource Source { get; set; }
        [AutoNotify]
        public bool IsFullScreen { get; set; }
        [AutoNotify]
        public bool ShowCtrlBar { get; set; }
        [AutoNotify]
        public bool IsPlaying { get; set; }
        [AutoNotify]
        public ICommand StopCmd { get; private set; }
        [AutoNotify]
        public ICommand DownloadCmd { get; private set; }
        [AutoNotify]
        public ICommand SnapshotCmd { get; private set; }

        #region 播放速度控制
        [AutoNotify]
        public CollectionViewSource SpeedType { get; set; }
        [AutoNotify]
        public bool IsSelectedSpeedControl { get; set; }
        [AutoNotify]
        public string SelectedSpeedType { get; set; } = "正常";

        private void updateSpeed()
        {
            _replayProcess.PlayRate = VideoPlaySpeedManager.GetSpeed(SelectedSpeedType);
            _replayProcess.FastTimes = (int)Math.Round(Math.Log(_replayProcess.PlayRate, 2));
            IsSelectedSpeedControl = false;
        }
        #endregion 播放速度控制

        private void doDownloadCmd()
        {
            PathSelectWin win = new PathSelectWin();
            if ((bool)win.ShowDialog())
            {
                lock (_videoLockObj)
                {
                    string path = win.DownPath;
                    if (_dictVideoVMS.Values.Count > 1)
                    {
                        path = System.IO.Path.Combine(path, $"Time_{PlaySlider.BeginTime:yyyyMMddHHmm}_{PlaySlider.EndTime:yyyyMMddHHmm}");
                    }
                    foreach (VideoControlViewModel vm in _dictVideoVMS.Values)
                        vm.Download(path);
                }
            }
        }

        private void doSnapshotCmd()
        {
        }

        private void doFullscreenCmd()
        {
            if (IsFullScreen)
                ShowCtrlBar = false;
            onFullScreenChanged();
        }

        private string buildKey(IVideoInfo vi)
        {
            return $"{vi.VideoId}|{vi.StreamId}";
        }
        private bool isContained(IVideoInfo vi)
        {
            return _dictVideoVMS.ContainsKey(buildKey(vi));
        }

        public void UpdateTimePeriod(DateTime begin, DateTime end)
        {
            List<IVideoInfo> vis = new List<IVideoInfo>();
            foreach (VideoControlViewModel video in _dictVideoVMS.Values)
            {
                vis.Add(video.VideoInfo);
            }

            Clear();
            PlaySlider.UpdateTimes(begin, end);
            
            onTimePeriodChanged();
            _replayProcess.JumpTo(PlaySlider.BeginTime);

            foreach (var video in vis)
                AddToPlay(video.VideoId, video.StreamId, video.VideoName);
        }

        public void UpdateSource(VideoDataSource src)
        {
            Clear();
            this.Source = src;
        }

        public bool IsReadyToPlay()
        {
            return Source != null;
        }

        public void AddToPlay(string videoId, int streamId, string videoName)
        {
            if (PlaySlider.BeginTime >= PlaySlider.EndTime)
            {
                DialogUtil.ShowWarning("无效查询时间段。");
                return;
            }
            if (Source == null)
            {
                DialogUtil.ShowWarning("无效的数据源。");
                return;
            }

            VideoControlViewModel vm = null;
            string dictKey = null;
            if (Source.SrcType == SourceType.Local)
            {
                LocalDownloadInfoPacket param = new LocalDownloadInfoPacket(new VideoInfo(videoId, streamId, videoName), Source.LocalSourcePath);
                if (!isContained(param.Info))
                {
                    vm = new VideoControlViewModel(param, PlaySlider.BeginTime, PlaySlider.EndTime, _replayProcess);
                    dictKey = buildKey(param.Info);
                }
            }
            else
            {
                DownloadInfoParam downloadInfo = new DownloadInfoParam(Source.Storage.Ip, Source.Storage.Port, PlaySlider.BeginTime, PlaySlider.EndTime, videoId, streamId, ConstSettings.CachePath, videoName);
                if (!isContained(downloadInfo))
                {
                    vm = new VideoControlViewModel(downloadInfo, _replayProcess);
                    dictKey = buildKey(downloadInfo);
                }
            }
            if (vm != null && dictKey != null)
            {
                lock (_videoLockObj)
                {
                    _dictVideoVMS[dictKey] = vm;
                }
                addOrRemoveVideoEvent(vm, true);
                onVideoAdded(vm);
                vm.ProgressOffset = PlaySlider.Slider / (double)PlaySlider.SliderMaximum;
            }
        }

        public void Remove(IVideoInfo vInfo)
        {
            if (isContained(vInfo))
            {
                string key = buildKey(vInfo);
                VideoControlViewModel vm = _dictVideoVMS[key];
                addOrRemoveVideoEvent(vm, false);
                lock (_videoLockObj)
                {
                    vm.Close();
                    _dictVideoVMS.Remove(key);
                }
                if (_dictVideoVMS.Count == 0)
                    Stop();

                onVideoRemoved(vm);
            }
        }

        public void Clear()
        {
            Stop();
            lock (_videoLockObj)
            {
                foreach (VideoControlViewModel vm in _dictVideoVMS.Values)
                {
                    addOrRemoveVideoEvent(vm, false);
                    vm.Close();
                }
                _dictVideoVMS.Clear();
            }
            onCleared();
        }

        private void addOrRemoveVideoEvent(VideoControlViewModel vm, bool add)
        {
            if (add)
                vm.Closed += Video_Closed;
            else
                vm.Closed -= Video_Closed;
        }

        private void Video_Closed(object sender, EventArgs e)
        {
            VideoControlViewModel vm = sender as VideoControlViewModel;
            Remove(vm.VideoInfo);
        }

        private void jump()
        {
            TimeSpan ts = TimeSpan.FromSeconds(PlaySlider.Slider);
            DateTime time = (DateTime)PlaySlider.BeginTime + ts;
            _replayProcess.JumpTo(time);
        }

        public void Stop()
        {
            IsPlaying = false;
            PlaySlider.Slider = 0;
            jump();
        }

        #region 【事件】
        public event Action<VideoControlViewModel> VideoAdded;
        public event Action<VideoControlViewModel> VideoRemoved;
        public event EventHandler Cleared;
        public event EventHandler TimePeriodChanged;
        public event EventHandler FullScreenChanged;

        private void onVideoAdded(VideoControlViewModel vm)
        {
            if (VideoAdded != null)
                VideoAdded(vm);
        }

        private void onVideoRemoved(VideoControlViewModel vm)
        {
            if (VideoRemoved != null)
                VideoRemoved(vm);
        }

        private void onCleared()
        {
            if (Cleared != null)
                Cleared(this, new EventArgs());
        }

        private void onTimePeriodChanged()
        {
            if (TimePeriodChanged != null)
                TimePeriodChanged(this, new EventArgs());
        }

        private void onFullScreenChanged()
        {
            EventHandler handler = FullScreenChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion 【事件】
    }
}