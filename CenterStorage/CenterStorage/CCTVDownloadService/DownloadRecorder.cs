using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadRecorder : RecorderBase
    {
        TimeSpan _fileTimeSpanMax;
        public DownloadRecorder(string path, TimeSpan fileTimeSpanMax) : base(path)
        {
            _fileTimeSpanMax = fileTimeSpanMax;
        }

        protected override bool isCanCloseFilesFromTime(DateTime curTime)
        {
            return (curTime >= _fileStartTime.Add(_fileTimeSpanMax));
        }

        protected override void updateFolderPath()
        {
            _curFolder = _filePath;
        }

        protected override void updateShortIndexes(TimePeriodPacket newTi)
        { }
    }
}
