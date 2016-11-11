using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadingLimitManager
    {
        public static DownloadingLimitManager Instance { get; private set; }
        static DownloadingLimitManager()
        {
            Instance = new DownloadingLimitManager();
        }

        public int DownloadingSup { get; set; } = 5;
        public int DownloadingNum { get; set; }

        public bool IsDownloadingLess()
        {
            return DownloadingNum < DownloadingSup;
        }

        public bool IsDownloadingMore()
        {
            return DownloadingNum > DownloadingSup;
        }
    }
}