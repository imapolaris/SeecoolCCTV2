using CCTVDownloadService;
using CenterStorageCmd;
using CenterStorageService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCenterStorageService
{
    class BaseInfo
    {
        public static void Add1()
        {
            var beginTime = new DateTime(2016, 3, 22, 23, 50, 1, 156);
            string videoId = "videoId";
            int streamId = 2;
            string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
            GlobalData.FileLengthSup = new TimeSpan(0, 5, 0);
            using (SyncRecorder recorder = new SyncRecorder(path))
            {
                recordAddMinutes(recorder, beginTime, 0, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 1, DataType.SysHead);       //new
                recordAddMinutes(recorder, beginTime, 1, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 2, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 3, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 4, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 5, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 6, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 7, DataType.StreamDataKey);  //new
                recordAddMinutes(recorder, beginTime, 8, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 9, DataType.SysHead);        //new
                recordAddMinutes(recorder, beginTime, 9, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 10, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 11, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 12, DataType.StreamDataKey);  //new
                recordAddMinutes(recorder, beginTime, 13, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 14, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 15, DataType.StreamDataKey);
                recordAddMinutes(recorder, beginTime, 16, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 17, DataType.StreamData);
                recordAddMinutes(recorder, beginTime, 18, DataType.StopSign);
                writeError(Path.Combine(path, GlobalProcess.FolderPath(beginTime)), $"20100322235959999{GlobalProcess.IndexesFormat()}");
            }
        }

        public static void AddData_videoId_003_2_20160330()
        {
            var beginTime = new DateTime(2016, 3, 30, 01, 10, 20, 30);
            string videoId = "videoID_003";
            int streamId = 2;
            string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
            addData(path, beginTime, 0, 60);
            parallelMore(new DateTime(2010, 3, 23, 01, 10, 20, 30), 0, 100, 60);
        }
        
        public static void AddOldVideo()
        {
            var beginTime = new DateTime(2001, 3, 23, 01, 50, 1, 156);
            string videoId = "videoID_003";
            int streamId = 2;
            string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
            addData(path, beginTime, 0, 60);
        }

        public static void AddMoreOldVideos()
        {
            DateTime beginTime = new DateTime(2000, 3, 23, 1, 50, 1, 156);
            parallelMore(beginTime, 0, 100, 60);//60s
        }
        
        static void parallelMore(DateTime beginTime, int from, int to, int secondsLength)
        {
            Parallel.For(from, to, index =>
            {
                string videoId = string.Format("VideoId_{0:X}", 0x50BAD15900010301 + index);
                int streamId = 2;
                string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
                addData(path, beginTime, 0, secondsLength);
            });
        }

        public static SecondsPeriod[] SecondsPeriodArray = new SecondsPeriod[]
        {
            new SecondsPeriod(0, 600),
            new SecondsPeriod(700, 1600),
            new SecondsPeriod(1800, 2000),
            new SecondsPeriod(3600, 3600),
            new SecondsPeriod(3700, 20000),
            new SecondsPeriod(57200, 58000)
        };

        public static void addData(string path, DateTime beginTime, int start, int end)
        {
            using (SyncRecorder recorder = new SyncRecorder(path))
            {
                recordAddSeconds(recorder, beginTime, 0, DataType.SysHead); //new
                fillRecordBySeconds(recorder, beginTime, start, end);
            }
        }

        private static void writeError(string path, string fileName)
        {
            if (Directory.Exists(path))
            {
                FileStream indexFile = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                byte[] buffer = Encoding.UTF8.GetBytes("test error data!!!");
                indexFile.Write(buffer, 0, buffer.Length);
                indexFile.Close();
            }
        }

        public static void fillRecordBySeconds(RecorderBase recorder, DateTime beginTime, int start, int end)
        {
            recordAddSeconds(recorder, beginTime, start, DataType.StreamDataKey);
            for (int i = start + 1; i < end; i++)
            {
                if(i % 5 == 0)
                    recordAddSeconds(recorder, beginTime, i, DataType.StreamDataKey);
                else
                    recordAddSeconds(recorder, beginTime, i, DataType.StreamData);
            }
            recordAddSeconds(recorder, beginTime, end, DataType.StopSign);
        }

        private static void recordAddMinutes(RecorderBase recorder, DateTime beginTime, int timeoutMinutes, DataType type)
        {
            recordAddTime(recorder, beginTime.AddMinutes(timeoutMinutes), type);
        }

        public static void recordAddSeconds(RecorderBase recorder, DateTime beginTime, int timeoutSeconds, DataType type)
        {
            recordAddTime(recorder, beginTime.AddSeconds(timeoutSeconds), type);
        }

        private static void recordAddTime(RecorderBase recorder, DateTime time, DataType type)
        {
            if (type == DataType.StreamData)
                recorder.Set(time, DataType.StreamData, new byte[3]);
            else if (type == DataType.StreamDataKey)
                recorder.Set(time, DataType.StreamDataKey, new byte[6]);
            else if (type == DataType.SysHead)
                recorder.Set(time, DataType.SysHead, new byte[2]);
            else
                recorder.Set(time, type, new byte[0]);
        }
    }

    public struct SecondsPeriod
    {
        public int StartSeconds;
        public int StopSeconds;
        public SecondsPeriod(int start, int end)
        {
            StartSeconds = start;
            StopSeconds = end;
        }
    }
}
