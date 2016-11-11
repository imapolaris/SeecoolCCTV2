using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCTVDownloadService;
using CenterStorageCmd;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class VideoPackageRecorderTest
    {
        string path = @"D:\读写测试\TestVideoPackageRecorder";
        DateTime begin = new DateTime(2016, 5, 6, 15, 30, 0);
        DateTime end = new DateTime(2016, 5, 6, 15, 32, 0);
        [TestMethod]
        public void TestVideoPackageRecorder()
        {
            FolderManager.DeleteDirectoryInfo(path);
            TimePeriodPacket[] tisAll = new TimePeriodPacket[] { new TimePeriodPacket(begin, end) };
            TimePeriodPacket[] tisCompleted = null;

            using (VideoPackageRecorder recorder = new VideoPackageRecorder(path, TimeSpan.FromMinutes(1), tisAll, tisCompleted))
            {
                recorder.ProbeTime = end;
                check(recorder, false, false, 0, 0);
                recorder.SetVideoBaseInfo(new VideoBasePacket(new byte[5], begin, 10000));
                check(recorder, true, false, 0, 0);
                recorder.Set(null);
                for (int i = 0; i < 23; i++)
                {
                    recorder.Set(getVideoStreamsPacket(begin.AddSeconds(5 * i)));
                    check(recorder, true, false, 5 * (i + 1), 1);
                }
                recorder.Set(getVideoStreamsPacket(begin.AddSeconds(5 * 23)));
                check(recorder, true, true, 120, 1);
                

                recorder.Set(getVideoStreamsPacket(begin.AddSeconds(120)));
                check(recorder, true, true, 120, 1);

                recorder.Stop();
                recorder.Set(getVideoStreamsPacket(begin.AddSeconds(120)));
                check(recorder, true, true, 120, 1);
            }
            FolderManager.DeleteDirectoryInfo(path);
        }

        private void check(VideoPackageRecorder recorder, bool isInitedVideoBase, bool isDownloaded, int seconds,int length)
        {
            Assert.AreEqual(isInitedVideoBase, recorder.IsInitedVideoBase);
            Assert.AreEqual(isDownloaded, recorder.IsDownloaded);
            Assert.AreEqual(seconds * 100.0 / 120, recorder.Percent);
            TimePeriodPacket[] tpps = recorder.GetDownloadedTimePeriods();
            Assert.AreEqual(length, tpps.Length);
            if (length > 0)
            {
                Assert.AreEqual(begin, tpps[0].BeginTime);
                Assert.AreEqual(begin.AddSeconds(seconds), tpps[0].EndTime);
            }
            if(seconds > 0)
                Assert.IsNotNull(FolderManager.GetVideoStreamsPacket(path, begin.AddSeconds(seconds - 1)));
            if(seconds < 120)
                Assert.AreEqual(begin.AddSeconds(seconds), recorder.ProbeTime);
        }

        private static VideoStreamsPacket getVideoStreamsPacket(DateTime time)
        {
            StreamPacket[] sp = new StreamPacket[] 
            {
                new StreamPacket(time, DataType.StreamDataKey, new byte[50]),
                new StreamPacket(time.AddSeconds(1), DataType.StreamData, new byte[20]),
                new StreamPacket(time.AddSeconds(2), DataType.StreamData, new byte[20]),
                new StreamPacket(time.AddSeconds(3), DataType.StreamData, new byte[20]),
                new StreamPacket(time.AddSeconds(4), DataType.StreamData, new byte[20])
            };
            return new VideoStreamsPacket(new TimePeriodPacket(time, time.AddSeconds(5)), sp);
        }
    }
}
