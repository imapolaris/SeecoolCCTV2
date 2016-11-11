using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CCTVDownloadService
{
    public class OnlineDownloadManager: IDownloadInfoExpand, IDownloadManager, IOnlinePlayback
    {
        Downloader _downloader;
        bool _downloading = false;
        public IDownloadInfo DownloadInfo { get { return _downloader.DownloadInfo; } }
        public Guid GuidCode { get; private set; }
        public string Name { get; private set; }
        public string Quality { get; private set; }
        public long Size { get { return _downloader.Size; } }
        public bool IsLocalDownload { get; private set; }
        public DownloadStatus DownloadStatus { get { return _downloader.DownloadingStatus; } }
        public string ErrorInfo { get { return _downloader.ErrorInfom; } }
        public bool IsPrior { get; private set; }
        int _priorNum = 0;
        public TimePeriodPacket[] TimePeriodsAll { get { return _downloader?.TimePeriodsAll?.TimePeriods; } }
        public TimePeriodPacket[] TimePeriodsCompleted { get { return _downloader.TimePeriodsCompleted; } }
        public long Speed { get { return _downloader.Speed; } }
        public DateTime UpdatedLastestTime { get { return _downloader.UpdateLastestTime; } }
        public Action<OnlineDownloadManager, string> ExpandChanged;
        Thread _thread;

        public OnlineDownloadManager(IDownloadInfo info, DownloadStatus status, bool isLocalDownload, string errorInfo)
        {
            GuidCode = Guid.NewGuid();
            IsLocalDownload = isLocalDownload;
            initDownloader(info, status,errorInfo);
            updateThread();
            Quality = "标清";
        }

        public void SetPriority(bool isPrior)
        {
            if (isPrior)
                _priorNum++;
            else
                _priorNum = Math.Max(_priorNum - 1, 0);
            IsPrior = _priorNum > 0;
            if (isNeedStartDownload())
                Start();
            else if (isInvalidDownload())
                Delete();
            onChanged(nameof(IsPrior));
        }

        public void Start()
        {
            if (DownloadStatus == DownloadStatus.Completed)
                return;
            if (DownloadStatusManager.IsDownloading(DownloadStatus))
                return;
            if (IsPrior || DownloadingLimitManager.Instance.IsDownloadingLess())
            {
                removeInvalidDownloadingNum();
                _downloading = true;
                DownloadingLimitManager.Instance.DownloadingNum++;
                updateThread();
                _downloader.Start();
            }
            else
                Waiting();
        }

        public void Waiting()
        {
            if (DownloadStatus != DownloadStatus.Completed && DownloadStatus != DownloadStatus.Waiting)
            {
                removeInvalidDownloadingNum();
                _downloader.Waiting();
            }
        }

        public void Pause()
        {
            removeInvalidDownloadingNum();
            if (!DownloadStatusManager.IsPauseOrEnd(DownloadStatus))
                _downloader.Stop();
        }

        public void Delete()
        {
            if (DownloadStatus != DownloadStatus.Deleted)
            {
                if (!IsPrior)
                {
                    removeInvalidDownloadingNum();
                    _downloader.Delete();
                }
                else
                    updateLocalDownloadStatus(false);
            }
        }

        public void SetProbeTime(DateTime time)
        {
            _downloader.SetProbeTime(time);
        }

        public string DownloadToLocal(string downloadPath)
        {
            string error = null;
            bool isLocalDownload = initIsLocalDownload(ref downloadPath);
            updateLocalDownloadStatus(isLocalDownload);
            string oldName = Name;
            updateName(DownloadInfo);

            if (DownloadInfo.DownloadPath != downloadPath || oldName != Name)
            {
                var info = DownloadInfo;
                var status = DownloadStatus;
                if (_downloader != null)
                    _downloader.Dispose();
                updateMoveDirect(info, oldName, downloadPath, ref error);
                initDownloader(info, status, null);
                onChanged(nameof(DownloadInfo));
            }
            Start();
            return error;
        }

        public VideoTimePeriodsPacket GetCompletedTimePeriods()
        {
            if (TimePeriodsCompleted != null)
                return new VideoTimePeriodsPacket(DownloadInfo, TimePeriodsCompleted);
            else
                return null;
        }

        public bool IsEndOfDownload()
        {
            return DownloadStatusManager.IsEndOfDownload(DownloadStatus);
        }

        void updateMoveDirect(IDownloadInfo info, string oldName, string path, ref string error)
        {
            try
            {
                moveTo(info.DownloadPath, path, oldName);
                info.UpdatePath(path);
            }
            catch (Exception ex)
            {
                error = string.Format("下载到指定路径\"{0}\"错误！\n当前下载路径：{1}\n{2}", path, info.DownloadPath, ex.Message);
            }
        }

        private void moveTo(string oldPath, string newPath, string oldFileName)
        {
            string oldName = Path.Combine(oldPath, oldFileName);
            string newName = Path.Combine(newPath, Name);
            if(new DirectoryInfo(newName).Exists)
                throw new IOException(string.Format("指定路径已存在下载任务！{0}", newName));
            new DirectoryInfo(newPath).Create();
            DirectoryInfo oldDir = new DirectoryInfo(oldName);
            if(oldDir.Root == new DirectoryInfo(newPath).Root)
            {
                oldDir.MoveTo(newName);//相同根路径移动
            }
            else
            {
                new DirectoryInfo(newName).Create();
                foreach (var file in oldDir.EnumerateFiles())
                {
                    string newFile = Path.Combine(newName, file.Name);
                    file.CopyTo(newFile);
                }
                FolderManager.DeleteDirectoryInfo(oldDir.FullName);
            }
        }

        private void updateLocalDownloadStatus(bool isLocalDownload)
        {
            IsLocalDownload = isLocalDownload;
            onChanged(nameof(IsLocalDownload));
        }

        private void updateName(IDownloadInfo info)
        {
            if (IsLocalDownload)
                Name = string.Format($"{info.VideoId}_{info.StreamId}");
            else
                Name = GuidCode.ToString();
        }

        private bool initIsLocalDownload(ref string downloadPath)
        {
            bool isLocalDownload = (!string.IsNullOrEmpty(downloadPath) && !downloadPath.Equals(ConstSettings.DefaultTempPath));
            if (!isLocalDownload)
                downloadPath = ConstSettings.DefaultTempPath;
            return isLocalDownload;
        }

        public VideoStreamsPacket GetVideoStreamsPacket(DateTime time)
        {
            return _downloader?.GetVideoStreamsPacket(time);
        }

        bool isNeedStartDownload()
        {
            return IsPrior && (DownloadStatusManager.IsWaitingOrPause(DownloadStatus));
        }

        bool isInvalidDownload()
        {
            return !IsPrior && !IsLocalDownload;
        }

        private void removeInvalidDownloadingNum()
        {
            if (_downloading)
            {
                _downloading = false;
                DownloadingLimitManager.Instance.DownloadingNum--;
            }
        }

        private void initDownloader(IDownloadInfo info, DownloadStatus status, string errorInfo)
        {
            updateName(info);
            _downloader = new Downloader(info, status, errorInfo, Name);
            onSize();
            _downloader.SizeEvent += onSize;
            onTimePeriodsCompleted();
            _downloader.TimePeriodsAllEvent += onTimePeriodsAll;
            _downloader.TimePeriodsCompletedEvent += onTimePeriodsCompleted;
            onStatus();
            _downloader.DownloadStatusEvent += onStatus;
        }

        public VideoTimePeriodsPacket GetVideoTimePeriods()
        {
            return _downloader?.TimePeriodsAll;
        }

        public VideoBasePacket GetVideoBasePacket()
        {
            return _downloader?.GetVideoBaseInfom();
        }

        private void updateThread()
        {
            if (_thread == null || _thread.ThreadState != ThreadState.Running)
            {
                _thread = new Thread(run) { IsBackground = true };
                _thread.Start();
            }
        }

        private void run()
        {
            while (true)
            {
                updateDynamicInfo();
                if (!DownloadStatusManager.IsDownloadingOrWaiting(DownloadStatus))
                    break;
                Thread.Sleep(1000);
            }
        }

        private void onSize()
        {
            onChanged(nameof(Size));
        }

        private void onTimePeriodsAll()
        {
            onChanged(nameof(TimePeriodsAll));
        }

        private void onTimePeriodsCompleted()
        {
            onChanged(nameof(TimePeriodsCompleted));
        }

        private void onStatus()
        {
            DownloadStatus status = _downloader.DownloadingStatus;
            if (!DownloadStatusManager.IsDownloadingOrWaiting(status))
                Pause();
            if (status == DownloadStatus.Completed)
                onChanged(nameof(UpdatedLastestTime));
            onChanged(nameof(ErrorInfo));
            onChanged(nameof(DownloadStatus));
            updateDynamicInfo();
        }

        private void updateDynamicInfo()
        {
            onChanged(nameof(Speed));
        }

        void onChanged(string settings)
        {
            var handle = ExpandChanged;
            if (handle != null)
                handle(this, settings);
        }
   }
}