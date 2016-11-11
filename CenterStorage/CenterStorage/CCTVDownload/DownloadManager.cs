using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVDownload.Util;

namespace CCTVDownload
{
    public class DownloadManager
    {
        public static DownloadManager Instance { get; private set; }
        static DownloadManager()
        {
            Instance = new DownloadManager();
        }

        public DownloadsViewModel DownloadsViewModel { get; private set; }
        DownloadCmd _downloader;
        private DownloadManager()
        {
            DownloadsViewModel = new DownloadsViewModel();
            DownloadsViewModel.DownloadControlEvent += onDownloadControl;
            SourceInfo source = new SourceInfo(ConfigReader.Instance.DownloadHost, ConfigReader.Instance.DownloadPort);
            _downloader = new DownloadCmd(source);
            _downloader.ErrorEvent += onError;
            _downloader.ErrorReceived += onErrorReceived;
            _downloader.DownloadInfoExpandAllReceived += onDownloadInfoExpandAll;
            _downloader.DownloadInfoExpandAnyReceived += onDownloadInfoExpandAny;
            _downloader.DownloadExpandPartReceived += onDownloadInfoExpandPart;
            _downloader.GetAllDownloadInfos();
        }

        private void onDownloadControl(DownloadControlCode code, byte[] buffer)
        {
            _downloader?.DownloadControl(code, buffer);
        }

        private void onDownloadInfoExpandAll(DownloadInfoExpandPacket[] packets)
        {
            try
            {
                Common.Util.WindowUtil.BeginInvoke(new Action(() => {
                    DownloadsViewModel.Load(packets);
                }));
            }
            catch { }
        }

        private void onDownloadInfoExpandAny(DownloadInfoExpandPacket packet)
        {
            try
            {
                Common.Util.WindowUtil.BeginInvoke(new Action(() => {
                    DownloadsViewModel.Add(packet);
                }));
            }
            catch { }
        }

        private void onDownloadInfoExpandPart(DownloadExpandPart part)
        {
            try
            {
                Common.Util.WindowUtil.BeginInvoke(new Action(() => {
                    DownloadsViewModel.Update(part);
                }));
            }
            catch { }
        }

        private void onErrorReceived(Exception obj)
        {
            System.Windows.MessageBox.Show(obj.ToString());
        }

        private void onError(Exception obj)
        {
            System.Windows.MessageBox.Show(obj.ToString());
        }
    }
}
