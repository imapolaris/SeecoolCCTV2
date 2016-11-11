using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Util
{
    public static class ConstSettings
    {
        public static int StreamId = 1;
        public static string CachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
        public static readonly string DownloadPath = Path.Combine(getStorePath(), "Download");
        public static readonly string ThumbnailPath = Path.Combine(getStorePath(), "Thumbnail");
        public static readonly string ConfigPath = Path.Combine(getStorePath(), "Config");
        public static CenterStorageCmd.Url.IUrl Url = null;
        public static string Remote = null;

        private static string getStorePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Seecool\\CCTVDownload");
        }
    }
}
