using CenterStorageCmd;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class OnlineDownloadsManager
    {
        public static OnlineDownloadsManager Instance { get; private set; }

        static OnlineDownloadsManager()
        {
            Instance = new OnlineDownloadsManager();
        }

        public Action<byte[]> DownloadAdded;
        public Action<byte[]> DownloadInfoPartChanged;
        List<OnlineDownloadManager> _downloads;
        private OnlineDownloadsManager()
        {
            _downloads = new List<OnlineDownloadManager>();
            loadHistory();
            new Thread(runMonitor) { IsBackground = true }.Start();
            updateDowningAndHistoryControl(true);
            _needDowning = true;
        }
        
        public void AddRange(bool isPrior, bool isLocalDownload, params IDownloadInfo[] infos)
        {
            if(infos != null)
                jointControl(new Action(() => addRange(infos, isPrior, isLocalDownload)));
        }

        public OnlineDownloadManager GetDownloadManagerIgnorePath(IDownloadInfo info)
        {
            lock (_downloads)
                return _downloads.FirstOrDefault(_ => DownloadInfoParam.AreEqualIgnorePath(_.DownloadInfo, info));
        }

        public OnlineDownloadManager GetDownloadManager(IDownloadInfo info)
        {
            lock (_downloads)
                return _downloads.FirstOrDefault(_ => DownloadInfoParam.AreEqual(_.DownloadInfo,info));
        }

        public OnlineDownloadManager GetDownloadManager(Guid guid)
        {
            lock (_downloads)
                return _downloads.FirstOrDefault(_ => _.GuidCode == guid);
        }

        public IDownloadInfoExpand[] GetDownloadPackets()
        {
            return _downloads.ToArray();
        }

        public void DownloadControl(DownloadControlCode controlCode, Guid guid)
        {
            Logger.Default.Trace("DownloadControl: " + controlCode + "  " + guid);
            var down = GetDownloadManager(guid);
            if (down != null)
            {
                if (controlCode == DownloadControlCode.Start)
                    down.Start();
                else if (controlCode == DownloadControlCode.Pause)
                    down.Pause();
                else if (controlCode == DownloadControlCode.Delete)
                    down.Delete();
                else if (controlCode == DownloadControlCode.GoTop)
                {
                    lock(_downloads)
                    {
                        _downloads.Remove(down);
                        _downloads.Insert(0, down);
                    }
                    onPart(down, nameof(DownloadControlCode.GoTop));
                }
            }
        }

        private void addRange(IDownloadInfo[] infos, bool isPrior, bool isLocalDownload)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var down = add(infos[i], DownloadStatus.Waiting, isPrior, isLocalDownload);
                down.Start();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private OnlineDownloadManager add(IDownloadInfo info, DownloadStatus status, bool isPrior, bool isLocalDownload, string errorInfo = null)
        {
            OnlineDownloadManager down = isLocalDownload? GetDownloadManager(info) : GetDownloadManagerIgnorePath(info);
            if (down == null)
            {
                down = new OnlineDownloadManager(info, status, isLocalDownload, errorInfo);
                add(down);
                down.SetPriority(isPrior);
                _needWriting = true;
            }
            else
            {
                if (isPrior)
                    down.SetPriority(isPrior);
                if (!down.IsLocalDownload && isLocalDownload)
                    down.DownloadToLocal(info.DownloadPath);
            }
            Logger.Default.Trace($"添加下载任务：Guid: {down.GuidCode} 名称：{info.VideoName}, VideoId:{info.VideoId}, Stream:{info.StreamId},"+ 
                $" Path:{info.DownloadPath}. Begin:{info.BeginTime}, End:{info.EndTime}, " +
                $"SourceIp:{info.SourceIp}, SourcePort:{info.SourcePort},本地：{isLocalDownload}");
            return down;
        }

        private void onExpandChanged(OnlineDownloadManager down, string obj)
        {
            onDownloadExpand(down, obj);
            onPart(down, obj);
        }

        private void onDownloadExpand(OnlineDownloadManager down, string obj)
        {
            switch (obj)
            {
                case nameof(down.DownloadStatus):
                    updateDownloadStatus(down);
                    break;
                case nameof(down.IsLocalDownload):
                    _needWriting = true;
                    break;
                case nameof(down.IsPrior):
                    _needDowning = true;
                    break;
            }
        }

        private void onPart(OnlineDownloadManager down, string obj)
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, obj);
            if (buffer != null)
                onDownloadInfoPartChanged(buffer);
        }

        private void updateDownloadStatus(OnlineDownloadManager down)
        {
            if(down.DownloadStatus == DownloadStatus.Deleted)
                delete(down);
            _needDowning = _needWriting = true;
        }

        private void jointControl(Action action)
        {
            updateDowningAndHistoryControl(false);
            action();
            updateDowningAndHistoryControl(true);
            _needDowning = true;
        }

        private void delete(OnlineDownloadManager down)
        {
            lock(_downloads)
            {
                down.ExpandChanged -= onExpandChanged;
                _downloads.Remove(down);
            }
        }

        private void add(OnlineDownloadManager down)
        {
            lock(_downloads)
            {
                down.ExpandChanged += onExpandChanged;
                _downloads.Add(down);
                onDownloadChanged(down);
            }
        }

        private void onDownloadChanged(OnlineDownloadManager down)
        {
            var handle = DownloadAdded;
            if (handle != null)
                handle(DownloadInfoExpandPacket.Encode(down));
        }

        private void onDownloadInfoPartChanged(byte[] buffer)
        {
            var handle = DownloadInfoPartChanged;
            if (handle != null)
                handle(buffer);
        }

        #region 更新当前下载项及历史记录

        bool _needDowning;
        bool _isUpdateDowningIfChanged;
        bool _needWriting;
        bool _isUpdateHistoryIfChanged;
        private void runMonitor()
        {
            while (true)
            {
                Thread.Sleep(10);
                updateDownloading();
                writeHistoryMemory();
            }
        }

        void updateDowningAndHistoryControl(bool update)
        {
            _isUpdateDowningIfChanged = _isUpdateHistoryIfChanged = update;
        }

        private void updateDownloading()
        {
            if (!_isUpdateDowningIfChanged || !_needDowning)
                return;
            _needDowning = false;
            while (DownloadingLimitManager.Instance.IsDownloadingLess())
            {
                Console.WriteLine("{0} - {1}", DownloadingLimitManager.Instance.DownloadingNum, DownloadingLimitManager.Instance.DownloadingSup);
                lock(_downloads)
                {
                    var down = _downloads.FirstOrDefault(_ => _.DownloadStatus == DownloadStatus.Waiting);
                    if (down != null)
                        down.Start();
                    else
                        break;
                }
            }
            while (DownloadingLimitManager.Instance.IsDownloadingMore())
            {
                Console.WriteLine("{0} - {1}", DownloadingLimitManager.Instance.DownloadingNum, DownloadingLimitManager.Instance.DownloadingSup);
                lock(_downloads)
                {
                    var down = _downloads.LastOrDefault(_ => DownloadStatusManager.IsDownloading(_.DownloadStatus) && !_.IsPrior);
                    if (down != null)
                        down.Waiting();
                    else
                        break;
                }
            }
        }

        private void loadHistory()
        {
            DownloadMemoryData[] datas = DownloadsConfigFile.Instance.ReadConfig();
            Logger.Default.Trace("共获取到 {0} 条下载纪录！", datas.Length);
            for (int i = 0; i < datas.Length; i++)
                add(datas[i].DownloadInfo, datas[i].Status, false, datas[i].IsLocalDownload, datas[i].Error);
        }
        
        private void writeHistoryMemory()
        {
            if (_isUpdateHistoryIfChanged && _needWriting)
            {
                _needWriting = false;
                lock(_downloads)
                {
                    DownloadMemoryData[] datas = _downloads.Where(_ => _.DownloadStatus != DownloadStatus.Deleted).Select(_ => new DownloadMemoryData(_.DownloadInfo, _.DownloadStatus, _.IsLocalDownload, _.ErrorInfo)).ToArray();
                    Logger.Default.Trace("更新下载列表， 当前下载任务数量：{0}", datas.Length);
                    DownloadsConfigFile.Instance.SetConfig(datas);
                }
            }
        }
        #endregion 更新当前下载项及历史记录
    }
}