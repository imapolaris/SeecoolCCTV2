using CCTVReplay.Combo;
using CCTVReplay.StaticInfo;
using CCTVReplay.Url;
using CCTVReplay.Util;
using CenterStorageCmd;
using CenterStorageCmd.Url;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CCTVReplay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpCommunication.Listener _listener;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            playCtrl.ViewModel.TimePeriodChanged += PlayCtrl_TimePeriodChanged;
            playCtrl.ViewModel.FullScreenChanged += PlayCtrl_FullScreenChanged;
            playCtrl.ViewModel.VideoAdded += PlayCtrl_VideoAdded;
            playCtrl.ViewModel.VideoRemoved += PlayCtrl_VideoRemoved;
            videoSelPanel.searcher.ViewModel.PlayVideoEvent += Searcher_PlayVideoEvent;
            loadTimePeriod();
            loadUdpCom();
            var updateCustom = AutoSave.CustomSettingAutoSave.Instance;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConstSettings.Url != null)
                playFromUrl(ConstSettings.Url);
            else if (ConstSettings.Remote != null)
                handleRemote(ConstSettings.Remote);
        }

        private void loadTimePeriod()
        {
            DateTime time = DateTime.Now.Subtract(TimeSpan.FromHours(1));
            DateTime begin = time.Date.AddHours(time.Hour);
            DateTime end = begin.AddHours(1);
            playCtrl.ViewModel.UpdateTimePeriod(begin, end);
        }

        private void playFromUrl(IUrl ui)
        {
            if (ui is ITimePeriod)
            {
                ITimePeriod tp = ui as ITimePeriod;
                playCtrl.ViewModel.UpdateTimePeriod(tp.BeginTime, tp.EndTime);
            }
            //更新数据源。
            VideoInfoManager.Instance.UpdateSource(ui);
            int sourceIndex = VideoInfoManager.Instance.SourceIndex;
            UrlAndIndex uai = new UrlAndIndex(ui, sourceIndex);
            new Thread(initPlay)
            {
                IsBackground = true,
                Name = "InitPlayVideos"
            }.Start(uai);
        }

        private void initPlay(object uaiObj)
        {
            UrlAndIndex uai = (UrlAndIndex)uaiObj;
            IUrl ui = uai.Url;
            int index = uai.Index;
            if (ui.VideoInfos == null || ui.VideoInfos.Length == 0)
                return;
            while (index == VideoInfoManager.Instance.SourceIndex)
            {
                PlayControlViewModel vm = getPlayCtrlModel();
                if (vm.IsReadyToPlay())
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        playVideoInfos(vm, ui);
                    }), null);
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private void playVideoInfos(PlayControlViewModel vm, IUrl ui)
        {
            if (checkEqual(ui, vm))
            {
                foreach (VideoInfo vi in ui.VideoInfos)
                {
                    if (!string.IsNullOrWhiteSpace(ui.LocalPath))
                    {
                        string path = Path.Combine(ui.LocalPath, $"{vi.VideoId}_{vi.StreamId}");
                        if (!new DirectoryInfo(path).Exists)
                        {
                            Util.DialogUtil.ShowMessage(string.Format("未找到 \"{0}\" 视频！", vi.VideoName));
                            continue;
                        }
                    }
                    vm.AddToPlay(vi.VideoId, vi.StreamId, vi.VideoName);
                }
                vm.IsPlaying = true;
            }
        }

        private PlayControlViewModel getPlayCtrlModel()
        {
            return this.Dispatcher.Invoke(() => { return playCtrl.ViewModel; });
        }

        private bool checkEqual(IUrl ui, PlayControlViewModel vm)
        {
            if (ui is ILocalUrl)
                return ui.LocalPath == vm.Source.LocalSourcePath;
            else if (ui is IRemoteUrl)
            {
                IRemoteUrl ru = ui as IRemoteUrl;
                return ru.SourceIp == vm.Source.Storage.Ip
                    && ru.SourcePort == vm.Source.Storage.Port
                    && ru.BeginTime == vm.PlaySlider.BeginTime
                    && ru.EndTime == vm.PlaySlider.EndTime;
            }
            return false;
        }

        private WindowState _initState = WindowState.Normal;
        private void PlayCtrl_FullScreenChanged(object sender, EventArgs e)
        {
            if (playCtrl.ViewModel.IsFullScreen)
            {
                _initState = this.WindowState;
                this.WindowState = WindowState.Maximized;
                ctrlBox.Visibility = Visibility.Collapsed;
                if (nodeTreeSpan.Width.Value != 0)
                {
                    _rightWidth = nodeTreeSpan.Width;
                    nodeTreeSpan.Width = new GridLength(0);
                }
            }
            else
            {
                this.WindowState = _initState;
                ctrlBox.Visibility = Visibility.Visible;
                if (nodeTreeSpan.Width.Value == 0)
                {
                    nodeTreeSpan.Width = _rightWidth;
                }
            }
        }

        private void PlayCtrl_VideoRemoved(Video.VideoControlViewModel obj)
        {
            updateDisplayVideos();
        }

        private void PlayCtrl_VideoAdded(Video.VideoControlViewModel obj)
        {
            updateDisplayVideos();
        }

        //更新树节点中，正在播放的视频节点状态。
        private void updateDisplayVideos()
        {
            IEnumerable<IVideoInfo> videos = playCtrl.ViewModel.GetPlayingVideos();
            videoSelPanel.searcher.ViewModel.UpdateDisplayVideos(videos.Select(v => v.VideoId).ToList());
        }

        private void PlayCtrl_TimePeriodChanged(object sender, EventArgs e)
        {
            videoSelPanel.UpdateTimePeriod(playCtrl.ViewModel.PlaySlider.BeginTime, playCtrl.ViewModel.PlaySlider.EndTime);
        }

        private void Searcher_PlayVideoEvent(string videoId, int streamId, string name)
        {
            playCtrl.ViewModel.AddToPlay(videoId, streamId, name);
        }

        #region 外部控制接口

        private void loadUdpCom()
        {
            Dictionary<string, Action<string>> dict = new Dictionary<string, Action<string>>();
            //dict["Init"] = handleInit;
            dict["Remote"] = handleRemote;
            dict["TimePeriod"] = handleTimePeriod;
            dict["VideoIds"] = handleVideoIds;
            dict["Seek"] = handleSeek;
            dict["Play"] = handlePlay;
            dict["Stop"] = handleStop;
            dict["Speed"] = handleSpeed;
            dict["Close"] = handleClose;
            _listener = new UdpCommunication.Listener();
            _listener.Register("Replay", dict);
        }

        private void handleRemote(string obj)
        {
            try
            {
                Source.DataSource source = new Source.DataSource();
                string[] strs = obj.Split(',');
                source.SourceType = Source.SourceType.Remote;
                source.RemoteSourceIp = strs[0];
                if (strs.Length > 1)
                    source.Username = strs[1];
                if (strs.Length > 2)
                    source.Password = strs[2];
                this.Dispatcher.Invoke(() =>
                {
                    VideoInfoManager.Instance.UpdateSource(source);
                });
            }
            catch (ErrorMessageException ex)
            {
                Common.Log.Logger.Default.Error("Remote: " + obj, ex.Message);
                Util.DialogUtil.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("Remote: " + obj, ex);
            }
        }

        void handleSeek(string param)
        {
            try
            {
                DateTime time = DateTime.Parse(param);
                this.Dispatcher.Invoke(() =>
                {
                    playCtrl.ViewModel.PlaySlider.JumpTo(time);
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("Seek: " + param, ex);
            }
        }

        private void handleTimePeriod(string param)
        {
            try
            {
                string[] strTimes = param.Split(',');
                this.Dispatcher.Invoke(() => {
                    playCtrl.ViewModel.UpdateTimePeriod(DateTime.Parse(strTimes[0]), DateTime.Parse(strTimes[1]));
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("TimePeriod: " + param, ex);
            }
        }

        private void handleVideoIds(string param)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    playCtrl.ViewModel.Clear();
                    string[] videoIds = param.Split(',');
                    foreach (var videoId in videoIds)
                        playCtrl.ViewModel.AddToPlay(videoId, ConstSettings.StreamId, null);
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("VideoIds: " + param, ex);
            }
        }

        private void handlePlay(string param)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    playCtrl.ViewModel.IsPlaying = bool.Parse(param);
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("Play: " + param, ex);
            }
        }

        private void handleStop(string param)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    playCtrl.ViewModel.Stop();
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("Stop: " + param, ex);
            }
        }

        private void handleSpeed(string param)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    var sources = VideoPlaySpeedManager.SpeedSources().ToList();
                    int index = Math.Min(sources.Count - 1, Math.Max(0, sources.Count / 2 - int.Parse(param)));
                    playCtrl.ViewModel.SelectedSpeedType = sources[index];
                });
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error("Play: " + param, ex);
            }
        }

        private void handleClose(string param)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        #endregion 外部控制接口

        #region 右侧视频列表显隐控制

        private GridLength _rightWidth;
        bool _isExpand = false;
        public bool IsExpand
        {
            get { return _isExpand;}
            set
            {
                _isExpand = value;
                if (IsExpand)
                {
                    _rightWidth = nodeTreeSpan.Width;
                    nodeTreeSpan.Width = new GridLength(0);
                    nodeSplitter.Width = new GridLength(0);
                }
                else
                {
                    nodeTreeSpan.Width = _rightWidth;
                    nodeSplitter.Width = new GridLength(5);
                }
                Console.WriteLine(nodeTreeSpan.Width + "  " + _rightWidth);
            }
        }

        #endregion 右侧视频列表显隐控制

        #region 【窗体控制事件】
        private void CloseCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void CanCloseExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MaximizeCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void CanMaximizeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState != WindowState.Maximized;
        }

        private void MinimizeCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CanMinimizeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RestoreCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void CanRestoreExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState != WindowState.Normal;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.Dispose();
            playCtrl.ViewModel.Clear();
            playCtrl.ViewModel.PlaySlider.Dispose();
            videoSelPanel.searcher.ViewModel.ClearSearcher();
        }
        #endregion 【窗体控制事件】

        #region 【URL导入导出事件】
        private void headerMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void headerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void ImportUrlCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ImportUrlWin win = new ImportUrlWin();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if ((bool)win.ShowDialog())
            {
                IRemoteUrl ui = win.ImportUrl;
                playFromUrl(ui);
            }
        }

        private void CanImportUrlExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExportUrlCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ExportUrlChecker checker = new ExportUrlChecker(playCtrl.ViewModel);
            if (checker.CanExport)
            {
                ExportUrlWin win = new ExportUrlWin();
                win.Owner = this;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                win.SetExportUrl(checker.GetExportUrl());
                win.ShowDialog();
            }
            else
            {
                DialogUtil.ShowWarning("无法从当前状态生成URL:\n" + checker.ErrorMessage);
            }
        }

        private void CanExportUrlExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion 【URL导入导出事件】

        class UrlAndIndex
        {
            public IUrl Url { get; private set; }
            public int Index { get; private set; }
            public UrlAndIndex(IUrl url, int index)
            {
                Url = url;
                Index = index;
            }
        }
    }
}
