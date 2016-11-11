using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class HistoryFolderInfo
    {
        public DateTime Time { get; private set; }
        public string Path { get; private set; }
        public HistoryFolderInfo(string path, DateTime time)
        {
            Path = path;
            Time = time;
        }
    }

    public class HistoryFolderArrayInfo
    {
        public DateTime Time { get; private set; }
        public string[] Paths { get; private set; }
        public HistoryFolderArrayInfo(string[] pathArray, DateTime time)
        {
            Paths = pathArray;
            Time = time;
        }
    }
}
