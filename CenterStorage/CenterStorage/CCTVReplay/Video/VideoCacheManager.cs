using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketHelper.Events;
using System.IO;
using System.Threading;
using SocketHelper;
using AopUtil.WpfBinding;
using CCTVReplay.Util;
using CCTVReplay.Video;
using System.Windows.Input;

namespace CCTVReplay
{
    public class VideoCacheManager : ObservableObject, IDisposable
    {
        const int MaxCache = 10;
        IVideoInfo _vInfo;
        VideoStreamProxy _streamProxy;
        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        Queue<VideoStreamsPacket> _vsQueue = new Queue<VideoStreamsPacket>();
        [AutoNotify]
        public string VideoName { get; private set; }
        [AutoNotify]
        public VideoDataInfoViewModel DataInfoModel { get; set; }
        [AutoNotify]
        public TimePeriodPacket[] TimePeriods { get; set; }
        [AutoNotify]
        public TimePeriodPacket[] DownloadProgress { get; private set; }
        [AutoNotify]
        public int DownloadPercent { get; set; }
        [AutoNotify]
        public VideoBasePacket BaseInfo { get; private set; }
        [AutoNotify]
        public DateTime LastestTime { get; private set; } = DateTime.MinValue;
        public DateTime NextTime { get; private set; } = DateTime.MinValue;

        [AutoNotify]
        public bool Downloading { get; private set; } = false;
        [AutoNotify]
        public ICommand DownloadCmd { get; private set; }
        [AutoNotify]
        public ICommand OpenPathCmd { get; private set; }
        private string _downPath;

        public bool IsConnected { get { return _streamProxy != null && _streamProxy.IsConnected; } }

        private VideoCacheManager(DateTime begin, DateTime end)
        {
            _disposeEvent.Reset();
            DataInfoModel = new VideoDataInfoViewModel();
            DataInfoModel.UpdateTimePacket(begin, end);

            DownloadCmd = new CommandDelegate(_ => doDownloadCmd());
            OpenPathCmd = new CommandDelegate(_ => doOpenPathCmd());

            loadStreamProxy();
        }

        private void loadStreamProxy()
        {
            _streamProxy = new VideoStreamProxy();
            _streamProxy.TimePeriodsAllReceived += onTimePeriodsAll;
            _streamProxy.TimePeriodsDownloadedReceived += onTimePeriodsDownloaded;
            _streamProxy.VideoBaseReceived += onVideoBase;
            _streamProxy.VideoStreamsReceived += onVideoStream;
            _streamProxy.MessageReceived += onMessage;
            _streamProxy.DownloadPathReceived += onDownloadPathReceived;
        }

        public VideoCacheManager(DownloadInfoParam param) : this(param.BeginTime, param.EndTime)
        {
            _vInfo = param;
            VideoName = param.VideoName;
            LastestTime = param.BeginTime;

            _streamProxy.Load(param);
            Start(param.BeginTime);
        }

        public VideoCacheManager(LocalDownloadInfoPacket param, DateTime begin, DateTime end) : this(begin, end)
        {
            _vInfo = param.Info;
            VideoName = param.Info.VideoName;
            LastestTime = begin;

            _streamProxy.LoadLocal(param);
            Start(begin);
        }

        private void doDownloadCmd()
        {
            PathSelectWin win = new PathSelectWin();
            if ((bool)win.ShowDialog())
            {
                Download(win.DownPath, true);
            }
        }

        private void doOpenPathCmd()
        {
            if (!string.IsNullOrWhiteSpace(_downPath))
            {
                string path = Path.Combine(_downPath, $"{_vInfo.VideoId}_{_vInfo.StreamId}");
                if (new DirectoryInfo(path).Exists)
                    System.Diagnostics.Process.Start("Explorer", "/select," + path);
                else
                    System.Diagnostics.Process.Start("Explorer", _downPath);
            }
        }

        public void Download(string path, bool showTip)
        {
            if (!string.IsNullOrWhiteSpace(path) && !Downloading)
            {
                string subPath = $"Time_{DataInfoModel.Begin:yyyyMMddHHmm}_{DataInfoModel.End:yyyyMMddHHmm}";
                string downPath = Path.Combine(path, subPath);
                DownloadToPath(downPath);
            }
            else
                DialogUtil.ShowError("无效的下载路径。");
        }

        public void Start(DateTime time)
        {
            LastestTime = time;
            if (changedNextTime())
            {
                clearQueue();
                startDownload();
            }
        }
        /// <summary>移除并返回位于开始处的视频流包。</summary>
        public VideoStreamsPacket DequeuePacket()
        {
            lock (_vsQueue)
            {
                if (_vsQueue.Count > 0)
                    return _vsQueue.Dequeue();
                return null;
            }
        }

        public int Count()
        {
            lock (_vsQueue)
                return _vsQueue.Count;
        }

        public void DownloadToPath(string downloadPath)
        {
            _streamProxy?.DownloadToPath(downloadPath);
            Downloading = true;
        }

        private double _allTime = 0;
        private void calcAllTime(TimePeriodPacket[] tpps)
        {
            _allTime = 0;
            foreach (TimePeriodPacket tpp in tpps)
            {
                TimeSpan ts = tpp.EndTime - tpp.BeginTime;
                _allTime += ts.TotalMilliseconds;
            }
        }

        public void Reload()
        {
            Console.WriteLine("TODO: Reload Cache!");
        }

        private int calcProgress(TimePeriodPacket[] pairs)
        {
            if (_allTime == 0)
                return 0;
            double total = 0;

            foreach (TimePeriodPacket tpp in pairs)
            {
                TimeSpan ts = tpp.EndTime - tpp.BeginTime;
                total += ts.TotalMilliseconds;
            }
            return Convert.ToInt32(total / _allTime * 100);
        }

        private void startDownload()
        {
            if (_streamProxy != null)
            {
                _streamProxy.ProbeTime(NextTime);
                sendToGetVideoData();
            }
        }
        private void onDownloadPathReceived(string obj)
        {
            _downPath = obj;
            if (string.IsNullOrWhiteSpace(_downPath))
                Downloading = false;
            else
                Downloading = true;
        }
        
        private void onTimePeriodsAll(TimePeriodPacket[] packets)
        {
            TimePeriods = packets;
            calcAllTime(packets);
            DataInfoModel.UpdateValidRange(TimePeriods);
            changedNextTime();
        }

        private void onTimePeriodsDownloaded(TimePeriodPacket[] packets)
        {
            DownloadProgress = packets;
            DownloadPercent = calcProgress(packets);
            DataInfoModel.UpdateLoadedRange(DownloadProgress);
            changedNextTime();
        }

        private void onVideoBase(VideoBasePacket obj)
        {
            BaseInfo = obj;
            startDownload();
        }

        private void onVideoStream(VideoStreamsPacket vsp)
        {
            if ((vsp.TimePeriod.BeginTime == DateTime.MinValue || vsp.TimePeriod.EndTime == DateTime.MinValue) && !_disposeEvent.WaitOne(1))
            {
                sendToGetVideoData();
                Thread.Sleep(10);
                return;
            }
            if (isValidPacket(vsp))
            {
                enqueue(vsp);
                //等待下次缓存或结束
                while (Count() >= MaxCache && !_disposeEvent.WaitOne(10))
                { }
                //判断是否需要继续获取缓存数据。
                if (!_disposeEvent.WaitOne(1))
                {
                    changedNextTime();
                    //Console.WriteLine("接收：" + vsp.TimePeriod.BeginTime.TimeOfDay + " - " + vsp.TimePeriod.EndTime.TimeOfDay);
                    //Console.WriteLine("Packet Count: " + Count() + "\t Next Time: " + NextTime);
                    if (NextTime >= DataInfoModel.End)//下载完成,停止缓冲
                    {
                        //Console.WriteLine("下载缓冲完毕！");
                        return;
                    }
                    //有下一数据包时间。
                    sendToGetVideoData();
                }
            }
            else
            {
                Console.WriteLine("无效的数据包!{0} - {1}", vsp.TimePeriod.BeginTime.TimeOfDay, vsp.TimePeriod.EndTime.TimeOfDay);
            }
        }

        private void onMessage(MessagePacket obj)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => 
            {
                string message = obj.Message;
                if (obj.Operate != null)
                    message += "\n" + obj.Operate;
                if (obj.Type == MessageType.Infom)
                    Console.WriteLine(message);
                else if (obj.Type == MessageType.Warn)
                    Util.DialogUtil.ShowWarning(message);
                else if (obj.Type == MessageType.Error)
                    Util.DialogUtil.ShowError(message);
            }));
        }

        private void sendToGetVideoData()
        {
            if (NextTime <= DataInfoModel.End)
                _streamProxy?.GetVideoStream(new VideoDataParam(_vInfo, NextTime));
        }

        private void enqueue(VideoStreamsPacket vsp)
        {
            lock (_vsQueue)
                _vsQueue.Enqueue(vsp);
            LastestTime = vsp.TimePeriod.EndTime;
        }

        private void clearQueue()
        {
            lock (_vsQueue)
                _vsQueue.Clear();
        }

        private bool changedNextTime()
        {
            DateTime nextTime = TimeProbeManager.GetProbeTime(TimePeriods?.ToArray(), LastestTime);
            if (nextTime != NextTime)
            {
                NextTime = nextTime;
                return true;
            }
            return false;
        }

        bool isValidPacket(VideoStreamsPacket vsp)
        {
            if (vsp?.TimePeriod != null)
            {
                if (vsp.TimePeriod.IsInRange(NextTime)) //防止数据重复下载，保证数据有效性
                    return true;
            }
            return false;
        }

        public void Dispose()
        {
            _disposeEvent.Set();
            if (_streamProxy != null)
                _streamProxy.Dispose();
            _streamProxy = null;
        }
    }
}
