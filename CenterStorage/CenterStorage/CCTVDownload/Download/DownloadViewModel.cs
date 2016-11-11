using AopUtil.WpfBinding;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using CCTVDownload.Util;
using System.Windows;
using CenterStorageCmd.Url;
using System.Diagnostics;
using System.IO;

namespace CCTVDownload
{
    public class DownloadViewModel : ObservableObject
    {
        public Action<DownloadControlCode, Guid> DownloadControlEvent;
        public Action<DownloadViewModel> DeleteEvent;
        public Action RefreshEvent;
        public Guid GuidCode { get; private set; }
        [AutoNotify]
        public string Name { get; private set; }
        [AutoNotify]
        public string Quality { get; private set; } = "未知";
        [AutoNotify]
        public string Size { get; private set; } = "未知";
        [AutoNotify]
        public string IPAddress { get; private set; }
        [AutoNotify]
        public string BeginTime { get; private set; }
        [AutoNotify]
        public string EndTime { get; private set; }
        [AutoNotify]
        public string DownloadTimeSpan { get; private set; }
        [AutoNotify]
        public DownloadStatus DownloadStatus { get; private set; } = DownloadStatus.Waiting;
        [AutoNotify]
        public bool IsLocalDownload { get; set; }
        [AutoNotify]
        public bool IsDownloading { get; set; }
        [AutoNotify]
        public bool IsPrior { get; set; }
        [AutoNotify]
        public int Slider { get; private set; } = 0;
        [AutoNotify]
        public string Speed { get; private set; } = "0B/s";
        [AutoNotify]
        public string ErrorInfo { get; private set; }
        [AutoNotify]
        public ICommand PlayingCommand { get; set; }
        [AutoNotify]
        public ICommand DeleteCommand { get; set; }
        [AutoNotify]
        public ICommand GoTopCommand { get; set; }
        [AutoNotify]
        public ICommand OpenCommand { get; set; }
        [AutoNotify]
        public string UpdatedLastestTime { get; set; }
        [AutoNotify]
        public bool Selected { get; set; }
        [AutoNotify]
        public bool SingleSelected { get; set; }
        [AutoNotify]
        public bool DisplayExpand { get; internal set; }
        TimePeriodPacket[] _all;
        TimePeriodPacket[] _downloaded;
        IDownloadInfo _downloadInfo;
        public DownloadViewModel(IDownloadInfoExpand packet)
        {
            _downloadInfo = packet.DownloadInfo;
            _all = packet.TimePeriodsAll;
            _downloaded = packet.TimePeriodsCompleted;
            GuidCode = packet.GuidCode;
            Quality = packet.Quality;
            Size = getByteLengthString(packet.Size);
            IPAddress = packet.DownloadInfo.SourceIp;
            BeginTime = GlobalProcess.TimeFormatOfCn(packet.DownloadInfo.BeginTime);
            EndTime = GlobalProcess.TimeFormatOfCn(packet.DownloadInfo.EndTime);
            DownloadTimeSpan = (packet.DownloadInfo.EndTime - packet.DownloadInfo.BeginTime).ToString();
            updateStatus(packet.DownloadStatus);
            IsLocalDownload = packet.IsLocalDownload;
            updateSlider();
            ErrorInfo = packet.ErrorInfo;
            updatedLastestTime(packet.UpdatedLastestTime);
            updateSpeed(packet.Speed);
            PlayingCommand = new CommandDelegate(_ => Play());
            DeleteCommand = new CommandDelegate(_ => Delete());
            GoTopCommand = new CommandDelegate(_ => GoTop());
            OpenCommand = new CommandDelegate(_ => Open());
            PropertyChanged += onPropertyChanged;
            if (packet.DownloadInfo.VideoName != null)
                Name = string.Format("{0} （{1}）", packet.DownloadInfo.VideoName, packet.Name);
            else
                Name = packet.Name;
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsDownloading):
                    if (!_changedDownloadingFromService)
                        updateStartOrPause();
                    break;
            }
        }

        private void updateStartOrPause()
        {
            if (IsDownloading)
                Start();
            else
                Pause();
        }

        public void Start()
        {
            onDownloadControl(GuidCode, DownloadControlCode.Start);
        }

        public void Pause()
        {
            onDownloadControl(GuidCode, DownloadControlCode.Pause);
        }

        public void Delete()
        {
            onDownloadControl(GuidCode, DownloadControlCode.Delete);
        }

        private void GoTop()
        {
            onDownloadControl(GuidCode, DownloadControlCode.GoTop);
        }

        private void Open()
        {
            string foldPath = _downloadInfo.DownloadPath;
            string path = System.IO.Path.Combine(foldPath, Name);
            if (new System.IO.DirectoryInfo(path).Exists)
                System.Diagnostics.Process.Start("Explorer", "/select," + path);
            else
                System.Diagnostics.Process.Start("Explorer", foldPath);
        }

        private void Play()
        {
            try
            {
                IDownloadInfo dInfo = _downloadInfo;
                IUrl ui = null;
                VideoInfo[] vInfos = new VideoInfo[] { new VideoInfo(dInfo.VideoId, dInfo.StreamId, dInfo.VideoName) };

                if (!new DirectoryInfo(Path.Combine(dInfo.DownloadPath, $"{dInfo.VideoId}_{dInfo.StreamId}")).Exists)
                    //throw new FileNotFoundException("未找到该视频！");
                    vInfos = null;

                ui = new RemoteUrl(dInfo.SourceIp, dInfo.SourcePort, dInfo.BeginTime, dInfo.EndTime, vInfos, dInfo.DownloadPath);

                string fileName = @"D:\Workspace\CCTV\CCTVReplay\CCTVReplay\bin\Debug\CCTVReplay.exe";
                if (!new System.IO.FileInfo(fileName).Exists)
                    fileName = @"F:\CCTV\CCTVReplay\CCTVReplay\bin\Debug\CCTVReplay.exe";
                if (!new System.IO.FileInfo(fileName).Exists)
                    fileName = @"CCTVReplay.exe";
                if (!new FileInfo(fileName).Exists)
                    throw new FileNotFoundException("未找到播放软件！");
                Process.Start(fileName, ui.ToString());
                Console.WriteLine("\n\n" + ui.ToString() + "\n\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Update(DownloadCode code, object value)
        {
            switch (code)
            {
                case DownloadCode.DownloadInfo:
                    var param = (DownloadInfoParam)value;
                    _downloadInfo = param;
                    IPAddress = param.SourceIp;
                    break;
                case DownloadCode.Name:
                    Name = (string)value;
                    break;
                case DownloadCode.Quality:
                    Quality = (string)value;
                    break;
                case DownloadCode.TimePeriodsAll:
                    _all = (TimePeriodPacket[])value;
                    updateSlider();
                    break;
                case DownloadCode.TimePeriodsCompleted:
                    _downloaded = (TimePeriodPacket[])value;
                    updateSlider();
                    break;
                case DownloadCode.Size:
                    Size = getByteLengthString((long)value);
                    break;
                case DownloadCode.IsLocalDownload:
                    IsLocalDownload = (bool)value;
                    onRefresh();
                    break;
                case DownloadCode.Status:
                    updateStatus((DownloadStatus)(int)value);
                    break;
                case DownloadCode.Speed:
                    updateSpeed((long)value);
                    break;
                case DownloadCode.ErrorInfo:
                    ErrorInfo = (string)value;
                    break;
                case DownloadCode.UpdatedLastestTime:
                    updatedLastestTime((DateTime)value);
                    break;
            }
        }

        void updateStatus(DownloadStatus status)
        {
            DownloadStatus = status;
            updateIsDownloading();
            updateStatus();
        }

        bool _changedDownloadingFromService = false;
        void updateIsDownloading()
        {
            _changedDownloadingFromService = true;
            IsDownloading = DownloadStatusManager.IsDownloadingOrWaiting(DownloadStatus);
            _changedDownloadingFromService = false;
        }

        private void updateSpeed(long speed)
        {
            if (DownloadStatus == DownloadStatus.Downloading)
                Speed = getByteLengthString(speed) + "/s";
            else
                Speed = DownloadStatusManager.ToHanZi(DownloadStatus);
        }

        private void updateStatus()
        {
            updateSpeed(0);
            if (DownloadStatus == DownloadStatus.Deleted)
                onDelete();
            else if (DownloadStatus == DownloadStatus.Completed)
                onRefresh();
        }

        void updatedLastestTime(DateTime time)
        {
            UpdatedLastestTime = time.ToString();
        }

        private void updateSlider()
        {
            if (_all != null && _downloaded != null)
            {
                var sum = _all.Sum(_ => (_.EndTime - _.BeginTime).Ticks);
                if (sum > 0)
                {
                    var valid = _downloaded.Sum(_ => (_.EndTime - _.BeginTime).Ticks);
                    Slider = (int)(valid * 100 / sum);
                    return;
                }
            }
            Slider = 0;
        }

        private void onDownloadControl(Guid guid, DownloadControlCode control)
        {
            var handle = DownloadControlEvent;
            if (handle != null)
                handle(control, guid);
        }

        void onDelete()
        {
            var handle = DeleteEvent;
            if (handle != null)
                handle(this);
        }

        void onRefresh()
        {
            var handle = RefreshEvent;
            if (handle != null)
                handle();
        }

        private static string getByteLengthString(long size)
        {
            ByteLengthUnit unit = ByteLengthUnit.B;
            double data = size;
            while (data > 1024 && unit < ByteLengthUnit.TB)
            {
                data /= 1024;
                unit++;
            }
            return data.ToString("0.0") + unit.ToString();
        }

        enum ByteLengthUnit
        {
            B,
            KB,
            MB,
            GB,
            TB
        }
    }
}
