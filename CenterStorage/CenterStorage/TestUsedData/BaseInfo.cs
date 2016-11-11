using CenterStorageCmd;
using CenterStorageService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUsedData
{
    class BaseInfo
    {
        public static void AddSeconds()
        {
            var beginTime = new DateTime(2010, 3, 23, 20, 50, 1, 156);
            string videoId = "videoId2";
            int streamId = 2;
            AddSeconds(videoId, streamId, beginTime);
        }

        public static void AddErrorDate()
        {
            DateTime time = new DateTime(2010, 3, 23);
            for (int i = 0; i < 30; i++)
                addErrorFile("videoId2", 2, time.Subtract(TimeSpan.FromDays(i)));
        }

        public static void AddSeconds2()
        {
            AddSeconds("videoID_003", 2, new DateTime(2010, 3, 23, 21, 50, 1, 156));
        }

        public static void AddLargeVideoReal()
        {
            DateTime beginTime = new DateTime(2016, 4, 5, 12, 40, 50, 123);
            Parallel.For(0, 3, index =>
            {
                string videoId = string.Format("CCTV1_{0:X}", 0x50BAD15900010301 + index);
                int streamId = 2;
                string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
                addLargeData(path, beginTime, 0, 3600);
            });
        }

        public static void AddLargeVideos()
        {
            DateTime beginTime = new DateTime(2016, 4, 5, 12, 40, 50, 123);
            Parallel.For(1024, 1027, index =>
            {
                string videoId = string.Format("VideoId_Large_{0:X}", 0x50BAD15900010301 + index);
                int streamId = 2;
                string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
                addLargeData(path, beginTime, 0, 3600);
            });
        }

        public static void AddTextMore()
        {
            parallelMore(new DateTime(2012, 12, 31, 21, 50, 1, 156), 0, 100, 3600 * 10);//10hours
            parallelMore(new DateTime(2013, 12, 31, 21, 50, 1, 156), 0, 10, 3600 * 30);//30hours
            parallelMore(new DateTime(2014, 12, 23, 21, 50, 1, 156), 0, 3, 3600 * 24 * 92);//92day
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

        public static void AddSeconds(string videoId, int streamId, DateTime beginTime)
        {
            string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
            GlobalData.FileLengthSup = new TimeSpan(0, 15, 0);
            using (SyncRecorder recorder = new SyncRecorder(path))
            {
                recordAddSeconds(recorder, beginTime, 0, DataType.SysHead);       //new
                for (int i = 0; i < SecondsPeriodArray.Length; i++)
                    fillRecordBySeconds(recorder, beginTime, SecondsPeriodArray[i].StartSeconds, SecondsPeriodArray[i].StopSeconds);
            }
        }

        private static void addLargeData(string path, DateTime beginTime, int start, int end)
        {
            using (SyncRecorder recorder = new SyncRecorder(path))
            {
                recordAddSecondsLarge(recorder, beginTime, 0, DataType.SysHead); //new
                fillRecordBySecondsLarge(recorder, beginTime, start, end);
            }
        }

        public static void addData(string path, DateTime beginTime, int start, int end)
        {
            using (SyncRecorder recorder = new SyncRecorder(path))
            {
                recordAddSeconds(recorder, beginTime, 0, DataType.SysHead); //new
                fillRecordBySeconds(recorder, beginTime, start, end);
            }
        }

        private static void addErrorFile(string videoId, int streamId, DateTime date)
        {
            string path = Path.Combine(GlobalData.Path, $"{videoId}_{streamId}", GlobalProcess.FolderPath(date));
            Directory.CreateDirectory(path);
            writeError(path, $"{GlobalProcess.FileNameFromDate(date)}235959999{GlobalProcess.IndexesFormat()}");
            writeError(path, $"{GlobalProcess.FileNameFromDate(date)}235959999{GlobalProcess.RecFormat()}");
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
                if (i % 5 == 0)
                    recordAddSeconds(recorder, beginTime, i, DataType.StreamDataKey);
                else
                    recordAddSeconds(recorder, beginTime, i, DataType.StreamData);
            }
            recordAddSeconds(recorder, beginTime, end, DataType.StopSign);
        }

        public static void recordAddSeconds(RecorderBase recorder, DateTime beginTime, int timeoutSeconds, DataType type)
        {
            recordAddTime(recorder, beginTime.AddSeconds(timeoutSeconds), type);
        }

        public static void recordAddSecondsLarge(RecorderBase recorder, DateTime beginTime, int timeoutSeconds, DataType type)
        {
            recordAddTimeLarge(recorder, beginTime.AddSeconds(timeoutSeconds), type);
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

        private static void fillRecordBySecondsLarge(RecorderBase recorder, DateTime beginTime, int start, int end)
        {
            recordAddSecondsLarge(recorder, beginTime, start, DataType.StreamDataKey);
            for (int i = start + 1; i < end; i++)
            {
                if (i % 5 == 0)
                    recordAddSecondsLarge(recorder, beginTime, i, DataType.StreamDataKey);
                else
                    recordAddSecondsLarge(recorder, beginTime, i, DataType.StreamData);
            }
            recordAddSecondsLarge(recorder, beginTime, end, DataType.StopSign);
        }

        private static void recordAddTimeLarge(RecorderBase recorder, DateTime time, DataType type)
        {
            if (type == DataType.StreamData)
                recorder.Set(time, DataType.StreamData, new byte[100000]);
            else if (type == DataType.StreamDataKey)
                recorder.Set(time, DataType.StreamDataKey, new byte[1920 * 1080 * 3]);
            else if (type == DataType.SysHead)
                recorder.Set(time, DataType.SysHead, new byte[50]);
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
