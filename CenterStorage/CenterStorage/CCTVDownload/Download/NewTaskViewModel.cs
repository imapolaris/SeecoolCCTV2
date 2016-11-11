using AopUtil.WpfBinding;
using CCTVDownload.AutoSave;
using CCTVDownload.Util;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CenterStorageCmd.Url;
using System.IO;

namespace CCTVDownload.Download
{
    public class NewTaskViewModel : ObservableObject
    {
        public IRemoteUrl URLInfo { get; private set; }
        [AutoNotify]
        public bool IsMaximize { get; set; }
        public NewTaskViewModel()
        {
            VideoInfos = new ObservableCollection<DownloadVideoInfo>();
            StartDownloadCmd = new CommandDelegate(_ => doStartDownloadCmd());
            PropertyChanged += this_PropertyChanged;

            DownloadDirectory = CustomSettingAutoSave.Instance.Setting.DownloadPath;
            DownloadUri = genTestUrl();
            tryloadUri();
        }

        private void tryloadUri()
        {
            try
            {
                string text = Clipboard.GetText();
                URLInfo = RemoteUrl.Parse(text) as IRemoteUrl;
                if(URLInfo != null)
                    DownloadUri = text;
            }
            catch
            { }
        }

        private static string genTestUrl()
        {
            DateTime begin = new DateTime(2016, 4, 5, 12, 40, 0);
            DateTime end = new DateTime(2016, 4, 5, 16, 43, 0);
            List<VideoInfo> viList = new List<VideoInfo>();
            for (int i = 1024; i < 1027; i++)
            {
                string videoId = string.Format("VideoId_Large_{0:X}", 0x50BAD15900010301 + i);
                int streamId = 2;
                viList.Add(new VideoInfo(videoId, streamId, "虚拟视频" + (i-1024).ToString()));
            }
            for (int i = 0; i < 3; i++)
            {
                string videoId = string.Format("CCTV1_{0:X}", 0x50BAD15900010301 + i);
                int streamId = 2;
                viList.Add(new VideoInfo(videoId, streamId, "测试视频" + i));
            }
            return new RemoteUrl(ConfigReader.Instance.CenterStorageHost, ConfigReader.Instance.CenterStoragePort, begin, end, viList.ToArray(), null).ToString();
        }

        private void this_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DownloadUri):
                    URLInfo = null;
                    ErrorInfo = null;
                    IsEnabledDirectory = false;
                    IsEnabledDownload = false;
                    VideoInfos = new ObservableCollection<DownloadVideoInfo>();
                    try
                    {
                        URLInfo = RemoteUrl.Parse(DownloadUri) as IRemoteUrl;
                        BeginTime = toShowTime(URLInfo.BeginTime);
                        EndTime = toShowTime(URLInfo.EndTime);
                        DownloadName = GlobalProcess.GetFolderName(URLInfo);

                        foreach (VideoInfo vi in URLInfo.VideoInfos)
                        {
                            VideoInfos.Add(new DownloadVideoInfo()
                            {
                                VideoId = vi.VideoId,
                                VideoName = string.IsNullOrWhiteSpace(vi.VideoName)? "未知名称": vi.VideoName,
                                StreamId = vi.StreamId,
                            });
                        }
                        if (!string.IsNullOrWhiteSpace(URLInfo.LocalPath))
                        {
                            DownloadDirectory = new DirectoryInfo(URLInfo.LocalPath).FullName;
                        }
                        else
                            IsEnabledDirectory = true;
                        IsEnabledDownload = true;
                    }
                    catch (Exception ex)
                    {
                        ErrorInfo = ex.Message;
                        initData();
                    }
                    break;
            }
        }

        private void initData()
        {
            URLInfo = null;
            BeginTime = null;
            EndTime = null;
        }

        [AutoNotify]
        public string ErrorInfo { get; set; }

        [AutoNotify]
        public string DownloadUri { get; set; }
        [AutoNotify]
        public bool IsEnabledDirectory { get; set; }
        [AutoNotify]
        public string DownloadDirectory { get; set; }
        [AutoNotify]
        public bool IsEnabledDownload { get; set; }
        [AutoNotify]
        public string DownloadName { get; set; }
        [AutoNotify]
        public string BeginTime { get; private set; }
        [AutoNotify]
        public string EndTime { get; private set; }
        [AutoNotify]
        public ObservableCollection<DownloadVideoInfo> VideoInfos { get; private set; }
        [AutoNotify]
        public ICommand StartDownloadCmd { get; set; }

        public IRemoteUrl GetParsedUrlInfo()
        {
            if (URLInfo == null)
                return null;
            IVideoInfo[] vis = VideoInfos.Where(dvi => dvi.Checked).ToArray();
            return new RemoteUrl(URLInfo.SourceIp, URLInfo.SourcePort, URLInfo.BeginTime, URLInfo.EndTime, vis, null);
        }

        private void doStartDownloadCmd()
        {
            if (URLInfo == null)
            {
                MessageBox.Show("解析到无效的URL。");
                return;
            }
            DownloadVideoInfo[] dvis = VideoInfos.Where(dvi => dvi.Checked).ToArray();
            if (dvis.Length == 0)
            {
                MessageBox.Show("未选中任何视频。");
                return;
            }
            if (string.IsNullOrWhiteSpace(DownloadName))
            {
                MessageBox.Show("无效的保存名称。");
                return;
            }
            OnStartDownload();
        }

        private string toShowTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region 【事件】
        public event EventHandler StartDownload;
        private void OnStartDownload()
        {
            EventHandler handler = StartDownload;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件】
    }
}
