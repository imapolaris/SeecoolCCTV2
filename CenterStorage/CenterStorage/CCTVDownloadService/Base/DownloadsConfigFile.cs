using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadsConfigFile
    {
        public static DownloadsConfigFile Instance { get; private set; }
        static DownloadsConfigFile()
        {
            Instance = new DownloadsConfigFile();
        }
        
        string _fileName;
        object _obj = new object();

        DownloadsConfigFile()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Seecool\\CCTVDownload");
            _fileName = System.IO.Path.Combine(path, "data.memory");
            Console.WriteLine(_fileName);
            ReadConfig();
        }

        public DownloadMemoryData[] ReadConfig()
        {
            lock (_obj)
            {
                if (new FileInfo(_fileName).Exists)
                {
                    MemoryConfig[] infos = ConfigFile<MemoryConfig[]>.FromFile(_fileName);
                    if(infos != null)
                        return infos.Select(_ => _.ToDownloadMemoryData()).ToArray();
                }
                return new DownloadMemoryData[0];
            }
        }

        public void SetConfig(DownloadMemoryData[] memorys)
        {
            lock (_obj)
            {
                new FileInfo(_fileName).Directory.Create();
                MemoryConfig[] infos = memorys.Select(_ => _.ToMemoryInfo()).ToArray();
                ConfigFile<MemoryConfig[]>.SaveToFile(_fileName, infos);
            }
        }
    }

    public class MemoryConfig
    {
        public string IPAddress;
        public int Port;
        public string VideoId;
        public int StreamId;
        public string VideoName;
        public DateTime BeginTime;
        public DateTime EndTime;
        public string Path;
        public DownloadStatus Status;
        public bool IsLocalDownload;
        public string Error;

        public MemoryConfig()
        {
            IsLocalDownload = true;
        }

        public DownloadMemoryData ToDownloadMemoryData()
        {
            return new DownloadMemoryData(new DownloadInfoParam(IPAddress, Port, BeginTime, EndTime, VideoId, StreamId, Path, VideoName), Status, IsLocalDownload, Error);
        }
    }

    public class DownloadMemoryData
    {
        public IDownloadInfo DownloadInfo { get; private set; }
        public DownloadStatus Status { get; private set; }
        public bool IsLocalDownload { get; private set; }
        public string Error { get; private set; }

        public DownloadMemoryData(IDownloadInfo downloadInfo, DownloadStatus status, bool isLocalDownload, string error)
        {
            DownloadInfo = downloadInfo;
            IsLocalDownload = isLocalDownload;
            Status = status;
            Error = error;
        }

        public MemoryConfig ToMemoryInfo()
        {
            return new MemoryConfig()
            {
                IPAddress = DownloadInfo.SourceIp,
                Port = DownloadInfo.SourcePort,
                VideoId = DownloadInfo.VideoId,
                StreamId = DownloadInfo.StreamId,
                VideoName = DownloadInfo.VideoName,
                BeginTime = DownloadInfo.BeginTime,
                EndTime = DownloadInfo.EndTime,
                Path = DownloadInfo.DownloadPath,
                Status = this.Status,
                IsLocalDownload = this.IsLocalDownload,
                Error = this.Error,
            };
        }
    }
}
