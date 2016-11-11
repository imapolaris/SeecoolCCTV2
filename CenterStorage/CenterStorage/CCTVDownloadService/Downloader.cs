using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class Downloader: IDisposable
    {
        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        VideoDownloadCmd _downCmd;
        VideoPackageRecorder _recorder;
        DownloadSpeedMonitor _speeder = new DownloadSpeedMonitor();
        VideoBaseFileRecorder _baseRec;
        public IDownloadInfo DownloadInfo { get; private set; }
        public DownloadStatus DownloadingStatus { get; private set; } = DownloadStatus.Waiting;
        public string ErrorInfom { get; private set; }
        public string Path { get; private set; }
        public long Size { get { return _baseRec?.VideoBase == null ? 0 : _baseRec.VideoBase.Length; } }
        public double Percent { get { return _recorder == null ? 0 : _recorder.Percent; } }
        public long Speed { get { return _speeder.Speed(DateTime.Now); } }
        public DateTime UpdateLastestTime { get; private set; } = DateTime.MaxValue;
        public Action DownloadStatusEvent;
        public Action TimePeriodsCompletedEvent;
        public Action TimePeriodsAllEvent;
        public Action SizeEvent;

        public Downloader(IDownloadInfo info, DownloadStatus status, string errorInfo, string name)
        {
            DownloadInfo = info;
            ErrorInfom = errorInfo;
            Path = System.IO.Path.Combine(DownloadInfo.DownloadPath, name);
            load();
            if (DownloadingStatus == DownloadStatus.Waiting)
            {
                if(DownloadStatusManager.IsDownloadingOrWaiting(status))
                    status = DownloadStatus.Waiting;
                onStatus(status);
                updateFromDownloadedStatus();
            }
        }

        object _objProbeTime = new object();
        DateTime? _probeTime = null;
        DateTime _lastProbeInputTime = DateTime.MinValue;
        private void updateProbeTime()
        {
            while (!_disposeEvent.WaitOne(100))
            {
                lock(_objProbeTime)
                {
                    if (_probeTime != null)
                    {
                        if (_lastProbeInputTime.AddSeconds(0.5) < DateTime.Now)
                        {
                            setProbeTime(_probeTime.Value);
                            Console.WriteLine(_probeTime.Value);
                            _probeTime = null;
                        }
                    }
                }
            }
        }

        void load()
        {
            if (isValidSavePath())
            {
                _speeder = new DownloadSpeedMonitor();
                updateLastWriteTime();
                _baseRec = new VideoBaseFileRecorder(Path);
                _baseRec.UpdateDownloadInfo(DownloadInfo);
                loadRecorder();
                loadVideoBase();
            }
        }

        private void loadRecorder()
        {
            if (_baseRec.TimePeriods != null)
            {
                try
                {
                    IndexesPacket[] indexesPackets = FolderManager.GetIndexesPackets(Path);
                    var downloadedTPPs = TimePeriodManager.Combine(indexesPackets);
                    initRecorder(_baseRec.TimePeriods.TimePeriods, downloadedTPPs);
                }
                catch(Exception ex) { Console.WriteLine(ex); }
            }
        }

        public void SetProbeTime(DateTime time)
        {
            lock(_objProbeTime)
            {
                _probeTime = time;
                _lastProbeInputTime = DateTime.Now;
                Console.WriteLine("---------------------\t" + DateTime.Now.TimeOfDay + "  \t" + time.TimeOfDay);

            }
        }

        private void setProbeTime(DateTime time)
        {
            if (_recorder != null)
                _recorder.ProbeTime = time;
        }

        public VideoStreamsPacket GetVideoStreamsPacket(DateTime time)
        {
            return FolderManager.GetVideoStreamsPacket(Path, time);
        }

        private void initRecorder(TimePeriodPacket[] tisAll, TimePeriodPacket[] tisCompleted)
        {
            disposeRecorder();
            _recorder = new VideoPackageRecorder(Path, TimeSpan.FromHours(1), tisAll, tisCompleted);
            onUpdateTimePeriodAll();
            onTimePeriodPacket();
        }

        public void Start()
        {
            Console.WriteLine("Start From Downloader: " + DownloadInfo.VideoId);
            if (isValidSavePath())
            {
                ErrorInfom = null;
                startDownload();
            }
        }

        public void Stop()
        {
            if (DownloadingStatus != DownloadStatus.Completed)
            {
                stopDownload();
                if (DownloadingStatus != DownloadStatus.Error)
                {
                    ErrorInfom = null;
                    onStatus(DownloadStatus.Paused);
                }
            }
        }

        public void Waiting()
        {
            stopDownload();
            onStatus(DownloadStatus.Waiting);
        }

        public void Delete()
        {
            onStatus(DownloadStatus.Deleted);
            stopDownload();
            FolderManager.ClearDirectoryInfoAll(Path);
        }

        public TimePeriodPacket[] TimePeriodsCompleted
        {
            get
            {
                TimePeriodPacket[] packets = new TimePeriodPacket[0];
                if (_recorder != null && TimePeriodsAll?.TimePeriods != null)
                    packets = TimePeriodManager.GetIntersections(_recorder.GetDownloadedTimePeriods(), TimePeriodsAll.TimePeriods);
                return packets;
            }
        }

        public VideoTimePeriodsPacket TimePeriodsAll
        {
            get
            {
                return _baseRec?.TimePeriods;
            }
        }

        public VideoBasePacket GetVideoBaseInfom()
        {
            return _baseRec?.VideoBase;
        }

        private void initTimePeriods()
        {
            _downCmd.GetTimePeriods();
        }

        private void initCmd()
        {
            _downCmd = new VideoDownloadCmd(DownloadInfo);
            _downCmd.BytesLengthEvent += onBytes;
            _downCmd.ErrorEvent += onErrorStatus;
            _downCmd.VideoTimePeriodsEvent += onVideoTimePeriodsReceived;
            _downCmd.VideoBaseEvent += onVideoBasePacketReceived;
            _downCmd.VideoStreamEvent += onVideoStream;
        }

        private void disposeCmd()
        {
            if (_downCmd != null)
            {
                _downCmd.BytesLengthEvent -= onBytes;
                _downCmd.ErrorEvent -= onErrorStatus;
                _downCmd.VideoTimePeriodsEvent -= onVideoTimePeriodsReceived;
                _downCmd.VideoBaseEvent -= onVideoBasePacketReceived;
                _downCmd.VideoStreamEvent -= onVideoStream;
                _downCmd.Dispose();
            }
            _downCmd = null;
        }

        private void onBytes(int bytesLength)
        {
            _speeder.Add(DateTime.Now, bytesLength);
        }

        Thread _thread = null;
        private void startDownload()
        {
            Console.WriteLine($"startDownload: {DownloadInfo.VideoId} DownloadStatus: {DownloadingStatus}");
            stopDownload();
            onStatus(DownloadStatus.Ready);
            _disposeEvent.Reset();
            _thread = new Thread(run) { IsBackground = true };
            _thread.Start();
        }

        private void stopDownload()
        {
            _disposeEvent.Set();
            _thread = null;
            disposeCmd();
            disposeRecorder();
        }

        private void run()
        {
            try
            {
                initCmd();
                getDatas();
            }
            catch (Exception ex)
            {
                onErrorStatus(ex.Message);
            }
        }

        private void getDatas()
        {
            do
            {//获取时间分布
                initTimePeriods();
            } while (!_disposeEvent.WaitOne(1000) && _recorder == null && isStatus(DownloadStatus.Ready));
            do
            {//获取视频包头
                _downCmd.GetVideoBaseInfo();
            }
            while (!_disposeEvent.WaitOne(1000) && !_recorder.IsInitedVideoBase && isStatus(DownloadStatus.Ready));
            if (!_disposeEvent.WaitOne(1) && isStatus(DownloadStatus.Ready))
            {//视频流
                onStatus(DownloadStatus.Downloading);
                getVideoPacket();
            }


            updateProbeTime();
        }

        private void onVideoTimePeriodsReceived(VideoDownloadCmd vd,VideoTimePeriodsPacket packet)
        {
            _baseRec.UpdateTimePeriods(packet);
            loadRecorder();
        }

        private void updateLastWriteTime()
        {
            UpdateLastestTime = FolderManager.GetLastestTime(Path);
        }

        private bool isStatus(DownloadStatus status)
        {
            return DownloadingStatus == status;
        }
        
        private void onVideoBasePacketReceived(VideoBasePacket packet)
        {
            if (packet != null)
            {
                _baseRec.UpdateVideoBase(packet);
                loadVideoBase();
            }
        }

        private void loadVideoBase()
        {
            if (_baseRec.VideoBase != null)
            {
                _recorder.SetVideoBaseInfo(_baseRec.VideoBase);
                onSize();
            }
        }
        
        private void onVideoStream(VideoStreamsPacket packet)
        {
            if (_recorder.Set(packet))
            {
                updateFromDownloadedStatus();
                onTimePeriodPacket();
                if (isStatus(DownloadStatus.Downloading))
                    getVideoPacket();
            }
        }

        void updateFromDownloadedStatus()
        {
            if (_recorder != null && _recorder.IsDownloaded)
            {
                updateLastWriteTime();
                onStatus(DownloadStatus.Completed);
                stopDownload();
            }
        }

        private void getVideoPacket() //获取数据。
        {
            _downCmd.GetVideoStreamsPacket(_recorder.ProbeTime);
        }

        private void onErrorStatus(string promptInfom)
        {
            ErrorInfom = promptInfom;
            onStatus(DownloadStatus.Error);
            stopDownload();
        }

        #region event
        private void onSize()
        {
            var hander = SizeEvent;
            if (hander != null)
                hander();
        }

        private void onTimePeriodPacket()
        {
            var hander = TimePeriodsCompletedEvent;
            if (hander != null)
                hander();
        }

        private void onStatus(DownloadStatus status)
        {
            Console.WriteLine($"{DownloadInfo.VideoId}\t {DownloadingStatus} => {status}");
            DownloadingStatus = status;

            var hander = DownloadStatusEvent;
            if (hander != null)
                hander();
        }

        bool isValidSavePath()
        {
            if (!new System.IO.DirectoryInfo(Path).Root.Exists)
            {
                onErrorStatus("无效的存储路径");
                return false;
            }
            return true;
        }

        private void onUpdateTimePeriodAll()
        {
            var handle = TimePeriodsAllEvent;
            if (handle != null)
                handle();
        }

        private void disposeRecorder()
        {
            if (_recorder != null)
                _recorder.Stop();
        }

        public void Dispose()
        {
            Stop();
            if (_recorder != null)
                _recorder.Dispose();
            _recorder = null;
        }
        #endregion event
    }
}