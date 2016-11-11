using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Collections.Generic;
using CenterStorageService;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class LargeDataSearcherTest
    {
        [TestMethod]
        public void TestLargeDataSearch_100Ids8Hours()
        {
            DateTime fromTime = new DateTime(2012, 12, 31, 21, 50, 1, 156);
            DateTime toTime = fromTime.AddHours(8);
            List<VideoInfo> vis = getVideoInfoList(0, 100);
            DateTime start = DateTime.Now;
            var results = StorageSearcher.Instance.Search(fromTime, toTime, vis.ToArray());
            DateTime end = DateTime.Now;
            Console.WriteLine($"Used Time: {DateTime.Now - start}");
            assertMore(fromTime, toTime, vis.ToArray(), results);
            Assert.IsTrue(end - start < TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestLargeDataSearch_10Ids1Day()
        {
            DateTime fromTime = new DateTime(2013, 12, 31, 21, 50, 1, 156);
            DateTime toTime = fromTime.AddDays(1);
            List<VideoInfo> vis = getVideoInfoList(0, 10);
            DateTime start = DateTime.Now;
            var results = StorageSearcher.Instance.Search(fromTime, toTime, vis.ToArray());
            DateTime end = DateTime.Now;
            Console.WriteLine($"Used Time: {DateTime.Now - start}");
            assertMore(fromTime, toTime, vis.ToArray(), results);
            Assert.IsTrue(end - start < TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestLargeDataSearch_3Ids90Day()
        {
            DateTime fromTime = new DateTime(2014, 12, 23, 21, 50, 1, 156);
            DateTime toTime = fromTime.AddDays(90);
            List<VideoInfo> vis = getVideoInfoList(0, 3);
            DateTime start = DateTime.Now;
            var results = StorageSearcher.Instance.Search(fromTime, toTime, vis.ToArray());
            DateTime end = DateTime.Now;
            Console.WriteLine($"Used Time: {DateTime.Now - start}");
            assertMore(fromTime, toTime, vis.ToArray(), results);
            Assert.IsTrue(end - start < TimeSpan.FromMilliseconds(50));
        }

        private void assertMore(DateTime start, DateTime end, VideoInfo[] vis, VideoTimePeriodsPacket[] results)
        {
            Assert.AreEqual(vis.Length, results.Length);
            for (int i = 0; i < vis.Length; i++)
            {
                Assert.AreEqual(vis[i].VideoId, results[i].VideoId);
                Assert.AreEqual(vis[i].StreamId, results[i].StreamId);
                Assert.AreEqual(1, results[i].TimePeriods.Length);
                Assert.AreEqual(start, results[i].TimePeriods[0].BeginTime);
                Assert.AreEqual(end, results[i].TimePeriods[0].EndTime);
            }
        }

        private static List<VideoInfo> getVideoInfoList(int startindex,int count)
        {
            List<VideoInfo> vis = new List<VideoInfo>();
            for (int i = 0; i < count; i++)
                vis.Add(new VideoInfo(string.Format("VideoId_{0:X}", 0x50BAD15900010301 + i + startindex), 2));
            return vis;
        }
    }
}
