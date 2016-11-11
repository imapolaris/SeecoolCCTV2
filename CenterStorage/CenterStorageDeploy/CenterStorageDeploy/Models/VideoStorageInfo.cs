using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageDeploy.Models
{
    public class VideoStorageInfo
    {
        public string VideoId { get; set; }
        public string VideoName { get; set; }
        public bool StorageOn { get; set; }
        public string Type { get; set; }
        public VideoStorageInfo[] Children { get; set; }
    }
}
