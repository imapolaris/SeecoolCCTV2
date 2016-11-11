using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using System.Threading;

namespace CenterStorageService
{
    public class StorageFlagsManager
    {
        public static StorageFlagsManager Instance { get; private set; }
        static StorageFlagsManager()
        {
            Instance = new StorageFlagsManager();
        }

        List<VideoInfo> _flags = new List<VideoInfo>();
        MultiVideoDisplayer _diplayer;
        public IVideoInfo[] StorageFlags { get { return _flags.ToArray(); } }
        private StorageFlagsManager()
        {
            _diplayer = new MultiVideoDisplayer();
            new Thread(hardDiskFreeSpaceCheck).Start();
            loadFlags();
        }

        public void Add(StorageFlagParam param)
        {
            Logger.Default.Trace("视频 {0}({3})  {1}  通道{2}", param.VideoName, param.StorageOn ? "开启" : "关闭", param.StreamId, param.VideoId);
            updateFlags(param);
            if (param.StorageOn)
                add(param);
            else
                remove(param);
        }

        void add(IVideoInfo vi)
        {
            _diplayer.Add(vi.VideoId, vi.StreamId);
        }

        void remove(IVideoInfo vi)
        {
            _diplayer.Remove(vi.VideoId, vi.StreamId);
        }

        private void updateFlags(StorageFlagParam param)
        {
            bool _flagChanged = _flags.RemoveAll(_ => _.VideoId == param.VideoId && _.StreamId == param.StreamId) > 0;
            if (param.StorageOn)
            {
                _flags.Add(param);
                _flagChanged = true;
            }
            if (_flagChanged)
                saveFlags();
        }

        string _fileName;
        private void loadFlags()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Seecool\\CCTVDownload");
            _fileName = System.IO.Path.Combine(path, "data.storage");
            if (new FileInfo(_fileName).Exists)
            {
                VideoInfo[] infos = ConfigFile<CenterStorageItem[]>.FromFile(_fileName)?.Select(_ => new VideoInfo(_.VideoId, _.StreamId, _.VideoName))?.ToArray();
                if (infos != null)
                {
                    _flags.AddRange(infos);
                    _flags.ForEach(add);
                }
            }
            Logger.Default.Trace("导入本地存储视频：" + path + "， 视频路数：" + _flags.Count);
        }

        private void saveFlags()
        {
            new FileInfo(_fileName).Directory.Create();
            CenterStorageItem[] items = _flags.Select(_ => new CenterStorageItem(_.VideoId, _.StreamId, _.VideoName)).ToArray();
            ConfigFile<CenterStorageItem[]>.SaveToFile(_fileName, items);
        }

        #region 磁盘剩余空间检测及相关操作

        private void hardDiskFreeSpaceCheck()
        {
            string diskName = new DirectoryInfo(GlobalData.Path).Root.FullName;
            long lengthDisk = HardDiskSpaceManager.GetHardDiskSpace(diskName);
            Logger.Default.Trace($"{diskName} 盘总空间: {lengthDisk}");
            while (true)
            {
                long lengthFree = HardDiskSpaceManager.GetHardDiskFreeSpace(diskName);
                if (lengthFree < GlobalData.HardDiskFreeSpaceInf)
                {
                    Logger.Default.Trace($"{diskName} 盘剩余空间: {lengthFree}, 剩余空间不足  {GlobalData.HardDiskFreeSpaceInf}");
                    VideoStoragerManager.DeleteEarliestVideo();      //删除最早的视频
                }
                else
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        #endregion 磁盘剩余空间检测及相关操作

        public class CenterStorageItem
        {
            public string VideoId;
            public int StreamId;
            public string VideoName;
            public CenterStorageItem(string videoId, int streamId, string name)
            {
                VideoId = videoId;
                StreamId = streamId;
                VideoName = name;
            }

            public CenterStorageItem()
            { }
        }
    }
}
