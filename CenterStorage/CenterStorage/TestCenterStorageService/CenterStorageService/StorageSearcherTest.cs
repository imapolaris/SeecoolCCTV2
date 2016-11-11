using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageService;
using CenterStorageCmd;
using System.IO;
using CCTVDownloadService;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class StorageSearcherTest
    {
        DateTime beginTime;
        [TestInitialize]
        public void Setup()
        {
            beginTime = new DateTime(2010, 3, 23, 20, 50, 1, 156);
        }

        [TestMethod]
        public void TestGetVideoStorageInfo()
        {
            var datas = BaseInfo.SecondsPeriodArray;
            TimePeriodPacket[] tis = new TimePeriodPacket[datas.Length];

            VideoInfo videoInfo = new VideoInfo("videoId2", 2);
            DateTime begin = new DateTime(2010, 3, 23, 20, 51, 50, 0);
            DateTime end = beginTime.AddSeconds(57900);
            var results = StorageSearcher.Instance.Search(begin, end, videoInfo);
            Assert.AreEqual(videoInfo.VideoId, results.VideoId);
            Assert.AreEqual(videoInfo.StreamId, results.StreamId);
            var array = results.TimePeriods;
            Assert.AreEqual(datas.Length - 1, array.Length);
            
            Assert.AreEqual(begin, array[0].BeginTime);
            Assert.AreEqual(beginTime.AddSeconds(datas[0].StopSeconds), array[0].EndTime);
            for (int i = 1; i < array.Length - 1; i++)
            {
                int index = i >= 3 ? i + 1 : i;
                Assert.AreEqual(beginTime.AddSeconds(datas[index].StartSeconds), array[i].BeginTime);
                Assert.AreEqual(beginTime.AddSeconds(datas[index].StopSeconds), array[i].EndTime);
            }
            Assert.AreEqual(beginTime.AddSeconds(datas[datas.Length - 1].StartSeconds), array[4].BeginTime);
            Assert.AreEqual(end, array[4].EndTime);
        }

        [TestMethod]
        public void TestStorageSearcher_Search()
        {
            string videoId = "dateTimeNow";
            int streamId = 2;
            string path = System.IO.Path.Combine(GlobalData.Path, $"{videoId}_{streamId}");
            FolderManager.DeleteDirectoryInfo(path);
            GlobalData.FileLengthSup = new TimeSpan(0, 15, 0);
            DateTime beginTime = DateTime.Now.Date;
            testVideoTimePeriodsPacket(videoId, streamId, 0);
            BaseInfo.addData(path, beginTime, 0, 600);
            
            testVideoTimePeriodsPacket(videoId, streamId, 1, 600);

            using (RecorderBase recorder = new DownloadRecorder(Path.Combine(path, $"{DateTime.Now.Year}\\{DateTime.Now.Month.ToString("00")}\\{DateTime.Now.Day.ToString("00")}"), GlobalData.FileLengthSup))
            {
                BaseInfo.recordAddSeconds(recorder, beginTime, 0, DataType.SysHead); //new
                BaseInfo.fillRecordBySeconds(recorder, beginTime, 600, 1200);
            }
            testVideoTimePeriodsPacket(videoId, streamId, 1, 1200);
            FolderManager.DeleteDirectoryInfo(path);
        }

        private void testVideoTimePeriodsPacket(string videoId, int streamId, int length, int lastSeconds = 0)
        {
            //查找当天数据
            IVideoInfo info = new VideoInfo(videoId, streamId);
            DateTime beginTime = DateTime.Now.Date.Subtract(TimeSpan.FromDays(1));
            DateTime endTime = DateTime.Now.Date.AddHours(10);
            VideoTimePeriodsPacket packet = StorageSearcher.Instance.Search(beginTime , endTime, info);
            Assert.IsNotNull(packet);
            Assert.AreEqual(streamId, packet.StreamId);
            Assert.AreEqual(videoId, packet.VideoId);
            Assert.AreEqual(length, packet.TimePeriods.Length);
            if (packet.TimePeriods.Length > 0)
            {
                Assert.AreEqual(DateTime.Now.Date, packet.TimePeriods[0].BeginTime);
                Assert.AreEqual(DateTime.Now.Date.AddSeconds(lastSeconds), packet.TimePeriods[0].EndTime);
            }
        }
        [TestMethod]
        public void TestVideoMultiStorageInfo()
        {
            VideoInfo[] videoInfos = new VideoInfo[] {
                new VideoInfo("videoId2", 2),
                new VideoInfo("videoID_003",2)
            };
            VideoTimePeriodsPacket[] results = StorageSearcher.Instance.Search(beginTime, beginTime.AddDays(1), videoInfos);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(5, results[0].TimePeriods.Length);
            Assert.AreEqual(5, results[1].TimePeriods.Length);
        }
        
        [TestMethod]
        public void TestVideoMultiStorageInfo_null()
        {
            Assert.AreEqual(0, StorageSearcher.Instance.Search(beginTime, beginTime.AddDays(1), new VideoInfo[0]).Length);
        }
    }
}
