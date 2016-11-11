using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public class SyncRecorder: RecorderBase
    {
        public SyncRecorder(string filePath):base(filePath)
        {
        }

        protected override void updateFolderPath()
        {
            _curFolder = Path.Combine(_filePath, GlobalProcess.FolderPath(_fileStartTime));
        }

        protected override bool isCanCloseFilesFromTime(DateTime curTime)
        {
            return (curTime >= _fileStartTime.Add(GlobalData.FileLengthSup)
                    || curTime.Year != _fileStartTime.Year
                    || curTime.DayOfYear != _fileStartTime.DayOfYear);
        }

        protected override void updateShortIndexes(TimePeriodPacket newTi)
        {
            List<TimePeriodPacket> shortIndexes = new List<TimePeriodPacket>();
            shortIndexes.Add(newTi);
            string simpleIndexesName = $"{GlobalProcess.FileNameFromDate(_fileStartTime)}{GlobalProcess.SimpleIndexesFormat()}";
            string fileName = Path.Combine(_curFolder, simpleIndexesName);
            var indexesDatas = FileManager.GetTimePeriods(fileName);
            if (indexesDatas != null)
                shortIndexes.AddRange(indexesDatas);
            var newArray = TimePeriodManager.Combine(shortIndexes.ToArray());
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                for (int i = 0; i < newArray.Length; i++)
                    writeBuffer(fs, TimePeriodPacket.Encode(newArray[i]));
            }
        }
    }
}