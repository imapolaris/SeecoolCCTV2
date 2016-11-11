using AopUtil.WpfBinding;
using CCTVReplay.Interface;
using CCTVReplay.Proxy;
using CCTVReplay.Util;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace CCTVReplay.Video
{
    public class VideoControlViewModel : ObservableObject
    {
        private List<TimePeriodPacket> _loadedData;
        public IVideoInfo VideoInfo { get; private set; }
        ITimeProcess _playProcess;
        private VideoControlViewModel(DateTime begin, DateTime end, ITimeProcess playProcess)
        {
            this.Begin = begin;
            this.End = end;
            _loadedData = new List<TimePeriodPacket>();
            _playProcess = playProcess;

            StreamChangeCmd = new CommandDelegate(_ => doStreamChangeCmd());
            SnapshotCmd = new CommandDelegate(_ => doSnapshotCmd());
            CloseCmd = new CommandDelegate(_ => doCloseCmd());
        }
        public VideoControlViewModel(DownloadInfoParam downloadInfo, ITimeProcess playProcess) : this(downloadInfo.BeginTime, downloadInfo.EndTime, playProcess)
        {
            VideoInfo = downloadInfo;
            TotalMilliSeconds = (downloadInfo.EndTime - downloadInfo.BeginTime).TotalMilliseconds;
            DisplayModel = new VideoDisplayViewModel(downloadInfo, _playProcess);
        }

        public VideoControlViewModel(LocalDownloadInfoPacket localDownInfo, DateTime begin, DateTime end, ITimeProcess playProcess)
            : this(begin, end, playProcess)
        {
            VideoInfo = localDownInfo.Info;
            DisplayModel = new VideoDisplayViewModel(localDownInfo, begin, end, _playProcess);
        }

        private double TotalMilliSeconds { get; set; }
        private DateTime Begin { get; set; }
        private DateTime End { get; set; }
        [AutoNotify]
        public string VideoName { get; private set; }
        [AutoNotify]
        public VideoDisplayViewModel DisplayModel { get; set; }
        [AutoNotify]
        public double ProgressOffset { get; set; }
        [AutoNotify]
        public string StreamName { get; set; }
        [AutoNotify]
        public bool IsFullScreen { get; set; }
        [AutoNotify]
        public ICommand StreamChangeCmd { get; private set; }
        [AutoNotify]
        public ICommand SnapshotCmd { get; private set; }
        [AutoNotify]
        public ICommand CloseCmd { get; private set; }

        private void doStreamChangeCmd()
        {
        }

        private void doSnapshotCmd()
        {
            var img = DisplayModel.GetSnapshot();
            if (img != null)
            {
                string name = DisplayModel.StreamManager.VideoName;
                if (string.IsNullOrWhiteSpace(name))
                    name = "未知视频";
                PreviewWin.Show(img, name);
            }
            else
                DialogUtil.ShowWarning("未获取到视频数据。");
        }
        
        private void doCloseCmd()
        {
            Close();
        }

        internal void Download(string downPath)
        {
            DisplayModel?.Download(downPath);
        }

        public void Close()
        {
            disposeDisplayModel();
            OnClosed();
        }

        private void disposeDisplayModel()
        {
            if (DisplayModel != null)
            {
                DisplayModel.Close();
            }
            DisplayModel = null;
        }
        #region 【事件】
        public event EventHandler Closed;

        private void OnClosed()
        {
            EventHandler handler = Closed;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件】
    }
}
