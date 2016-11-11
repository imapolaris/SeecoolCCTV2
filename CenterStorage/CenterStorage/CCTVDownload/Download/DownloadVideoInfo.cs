using AopUtil.WpfBinding;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownload.Download
{
    public class DownloadVideoInfo:ObservableObject, IVideoInfo
    {
        public DownloadVideoInfo()
        {
            Checked = true;
        }
        [AutoNotify]
        public bool Checked { get; set; }
        [AutoNotify]
        public string VideoId { get; set; }
        [AutoNotify]
        public int StreamId { get; set; }
        [AutoNotify]
        public string VideoName { get; set; }
    }
}
