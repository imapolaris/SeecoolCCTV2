using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using CenterStorageService;
using System.IO;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class VideoStoragerManagerTest
    {
        [TestMethod]
        public void TestVideoStoragerManager_GetVideoPacket()
        {
            var beginTime = new DateTime(2016, 3, 22, 23, 50, 1, 156);
            Assert.IsNull(VideoStoragerManager.GetVideoPacket("videoId_21 111", 2, new DateTime()));
            Assert.IsNull(VideoStoragerManager.GetVideoPacket("videoId", 2, beginTime));

            VideoStreamsPacket vp = VideoStoragerManager.GetVideoPacket("videoId", 2, beginTime.AddMinutes(1));
            assertVideoStreams(1, beginTime.AddMinutes(1), vp);

            VideoStreamsPacket vp2 = VideoStoragerManager.GetVideoPacket("videoId", 2, beginTime.AddMinutes(3));
            assertVideoStreams(2, beginTime.AddMinutes(2), vp2);

            VideoStreamsPacket vp3 = VideoStoragerManager.GetVideoPacket("videoId", 2, beginTime.AddMinutes(5));
            assertVideoStreams(3, beginTime.AddMinutes(4), vp3);
        }

        [TestMethod]
        public void TestVideoStoragerManager_GetVideoHeader()
        {
            var beginTime = new DateTime(2016, 3, 22, 23, 50, 1, 156);
            Assert.IsNull(VideoStoragerManager.GetVideoHeader("videoId_21 111", 2, new DateTime()));
            Assert.IsNull(VideoStoragerManager.GetVideoHeader("videoId", 2, beginTime));
            StreamPacket sp = VideoStoragerManager.GetVideoHeader("videoId", 2, beginTime.AddMinutes(1));
            Assert.IsNotNull(sp);
            Assert.AreEqual(DataType.SysHead, sp.Type);
            Assert.AreEqual(2, sp.Buffer.Length);
        }

        [TestMethod]
        public void TestVideoStoragerManager_GetFirstVideoHeader()
        {
            Assert.IsNull(VideoStoragerManager.GetVideoBaseInfom("w",10, DateTime.MinValue, DateTime.MaxValue));
            Assert.IsNull(VideoStoragerManager.GetVideoBaseInfom("videoId2", 2, new DateTime(2010, 3, 13), new DateTime(2010, 3, 23)));
            Assert.IsNull(VideoStoragerManager.GetVideoBaseInfom("videoId2", 2, new DateTime(2010, 3, 23), new DateTime(2010, 3, 23,20,50,0)));
            var info = VideoStoragerManager.GetVideoBaseInfom("videoId2", 2, new DateTime(2010, 3, 23), new DateTime(2010, 3, 23,20,55,0));
            Assert.IsNotNull(info);
            Assert.AreEqual(2, info.Header.Length);
            Assert.AreEqual(7080, info.Length);
        }

        [TestMethod]
        public void TestVideoStoragerManager_GetFoldersPathArray()
        {
            BaseInfo.Add1();
            string path = GlobalData.Path;
            string videoId = "videoId";
            int streamId = 2;
            string fullPath = Path.Combine(path, $"{videoId}_{streamId}");
            DateTime beginTime = new DateTime(2015, 3, 20);
            DateTime endTime = new DateTime(2017, 3, 20);
            DateTime[] folder = VideoStoragerManager.GetFolderPaths(videoId, streamId, beginTime, endTime);
            Assert.AreEqual(2, folder.Length);
            Assert.AreEqual(new DateTime(2016, 03, 22), folder[0]);
            Assert.AreEqual(new DateTime(2016, 03, 23), folder[1]);
        }

        [TestMethod]
        public void TestVideoStoragerManager_SearchEarliestSubfolder_InvalidPath()
        {
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolder(null));
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolder("ddddd"));
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolder(GlobalData.Path));
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolder(@"D:\视频录像\videoID_003_2\2001\03\23"));
            string path = Path.Combine(GlobalData.Path, @"Empty Video");
            Directory.CreateDirectory(path);
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolder(path));
        }

        [TestMethod]
        public void TestVideoStoragerManager_SearchEarliestSubfolder_One()
        {
            BaseInfo.AddOldVideo();
            string videoId = "videoID_003";
            int streamId = 2;
            string path = GlobalData.VideoPath(videoId, streamId);
            DateTime beginTime = new DateTime(2001, 3, 23, 01, 50, 1, 156);
            Directory.CreateDirectory(Path.Combine(path, @"10000\01\01"));
            Directory.CreateDirectory(Path.Combine(path, @"2001\1\01"));
            string earliestPath = Path.Combine(path, GlobalProcess.FolderPath(beginTime));
            var info = VideoStoragerManager.SearchEarliestSubfolder(path);
            Assert.IsNotNull(info);
            Assert.AreEqual(earliestPath, info.Path);
            Assert.AreEqual(beginTime.Date, info.Time);
        }

        [TestMethod]
        public void TestVideoStoragerManager_SearchEarliestSubfolders_NULL()
        {
            GlobalData.Path = null;
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolders());
            GlobalData.Path = @"D:\视频录像\videoID_003_2\123";
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolders());
            GlobalData.Path = @"D:\视频录像\videoID_003_2?\2001\03\23";
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolders());
            GlobalData.Path = @"i:\";
            Assert.IsNull(VideoStoragerManager.SearchEarliestSubfolders());
            GlobalData.Path = @"D:\视频录像\";
        }

        [TestMethod]
        public void TestVideoStoragerManager_SearchEarliestSubfolders_More()
        {
            BaseInfo.AddMoreOldVideos();
            DateTime start = DateTime.Now;
            var pi = VideoStoragerManager.SearchEarliestSubfolders();
            Console.WriteLine(DateTime.Now - start);
            Assert.IsTrue(TimeSpan.FromMilliseconds(20) > DateTime.Now - start);
            Assert.IsNotNull(pi);
            Assert.AreEqual(new DateTime(2000, 3, 23), pi.Time);
            Assert.AreEqual(100, pi.Paths.Length);
            start = DateTime.Now;
            VideoStoragerManager.DeleteEarliestVideo();
            Console.WriteLine(DateTime.Now - start);
            Assert.IsTrue(DateTime.Now - start < TimeSpan.FromMilliseconds(100));

            start = DateTime.Now;
            var pi1 = VideoStoragerManager.SearchEarliestSubfolders();
            Console.WriteLine(DateTime.Now - start);
            Assert.IsTrue(DateTime.Now - start < TimeSpan.FromMilliseconds(20));
            Assert.AreNotEqual(new DateTime(2000, 3, 23), pi1.Time);
        }

        [TestMethod]
        public void TestVideoStreamSizeSearcher＿GetLength()
        {
            DateTime beginTime = new DateTime(2016, 04, 05, 12, 40, 50, 123);
            DateTime endTime = new DateTime(2016, 04, 05, 12, 55, 50, 123);
            long length = VideoStoragerManager.GetLength("VideoId_Large_50BAD15900010701", 2, beginTime, endTime, 70);
            Assert.AreEqual(1191762070 - 70, length);
        }

        [TestMethod]
        public void TestVideoStreamSizeSearcher_GetLength＿LongTime()
        {
            DateTime beginTime = new DateTime(2014, 4, 23, 1, 40, 50, 123);
            DateTime endTime = new DateTime(2018, 04, 05, 12, 55, 50, 123);
            DateTime start = DateTime.Now;
            long length = VideoStoragerManager.GetLength("VideoId_50BAD15900010301", 2, beginTime, endTime, 22);
            Console.WriteLine(DateTime.Now - start);
            long size = (3600 * 24 * 92 / 5) * ((20 + 6) + 4 * (20 + 3));
            Assert.AreEqual(size, length);
        }

        private static void assertVideoStreams(int expLength, DateTime beginTime, VideoStreamsPacket vp)
        {
            Assert.IsNotNull(vp);
            Assert.AreEqual(beginTime, vp.TimePeriod.BeginTime);
            Assert.AreEqual(beginTime.AddMinutes(expLength), vp.TimePeriod.EndTime);
            Assert.AreEqual(expLength, vp.VideoStreams.Length);
            Assert.AreEqual(beginTime, vp.VideoStreams[0].Time);
            Assert.AreEqual(DataType.StreamDataKey, vp.VideoStreams[0].Type);
            Assert.AreEqual(6, vp.VideoStreams[0].Buffer.Length);
            for (int i = 1; i < expLength; i++)
            {
                Assert.AreEqual(beginTime.AddMinutes(i), vp.VideoStreams[i].Time);
                Assert.AreEqual(DataType.StreamData, vp.VideoStreams[i].Type);
                Assert.AreEqual(3, vp.VideoStreams[i].Buffer.Length);
            }
        }
    }
}
