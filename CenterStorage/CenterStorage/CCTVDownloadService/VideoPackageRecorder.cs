using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class VideoPackageRecorder: IDisposable
    {
        DownloadProgressManager _downloadProgress;
        DownloadRecorder _recorder;
        DateTime _endTime = DateTime.MaxValue;
        object _lockObj = new object();
        public bool IsInitedVideoBase { get; private set; } = false;
        public VideoPackageRecorder(string path, TimeSpan fileTimeSpanMax, TimePeriodPacket[] tisAll, TimePeriodPacket[] tisCompleted)
        {
            _recorder = new DownloadRecorder(path, fileTimeSpanMax);
            _downloadProgress = new DownloadProgressManager(tisAll, tisCompleted);
        }

        public void SetVideoBaseInfo(VideoBasePacket packet)
        {
            lock(_lockObj)
            {
                if (_recorder != null)
                {
                    _recorder.Set(new StreamPacket(packet.Time, DataType.SysHead, packet.Header));
                    IsInitedVideoBase = true;
                }
            }
        }

        public bool Set(VideoStreamsPacket info)
        {
            lock(_lockObj)
            {
                if (_recorder != null && info != null)
                {
                    if (_downloadProgress.IsValidTime(info.TimePeriod))
                    {
                        update(info);
                        if (IsDownloaded)
                            stopSign();
                        return true;
                    }
                }
            }
            return false;
        }

        private void update(VideoStreamsPacket info)
        {
            if (_endTime != info.TimePeriod.BeginTime)
                stopSign();
            for (int i = 0; i < info.VideoStreams.Length; i++)
                _recorder.Set(info.VideoStreams[i]);
            _recorder.FinishPackage(info.TimePeriod.EndTime);
            _endTime = info.TimePeriod.EndTime;
            _downloadProgress.Download(info.TimePeriod);
        }

        public void Dispose()
        {
            Stop();
        }

        private void stopSign()
        {
            if (_recorder != null)
            {
                _recorder.Set(new StreamPacket(_endTime, DataType.StopSign, new byte[0]));
            }
        }

        public DateTime ProbeTime
        {
            get { return _downloadProgress.ProbeTime; }
            set
            {
                _downloadProgress.UpdateProbeTime(value);
            }
        }

        public bool IsDownloaded { get { return _downloadProgress.IsDownloaded; } }

        public double Percent { get { return _downloadProgress.Percent; } }

        public TimePeriodPacket[] GetDownloadedTimePeriods()
        {
            return _downloadProgress.DownloadedTimePeriods;
        }

        public void Stop()
        {
            lock (_lockObj)
            {
                stopSign();
            }
        }
    }
}
