using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageService;
using CenterStorageCmd;
using System.Threading.Tasks;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class StorageDownloaderTest
    {
        [TestMethod]
        public void TestStorageDownloader()
        {
            var baseInfo = StorageDownloader.Instance.GetVideoBaseInfom("videoId", 2, new DateTime(2016,3,22,23,53,30), new DateTime(2016, 3, 22, 23, 55, 30));
            Assert.AreEqual((26*2+23*3), baseInfo.Length);
            var packet = StorageDownloader.Instance.GetVideoPacket("videoId", 2, new DateTime(2016, 3, 22, 23, 53, 30));
            Assert.IsNotNull(packet);
        }

        [TestMethod]
        public void TestStorageDownloader_BaseInfom_100()
        {
            int length = 100;
            DateTime start = DateTime.Now;
            VideoBasePacket[] infos = new VideoBasePacket[length];
            for(int index = 0; index <length; index++)
            //Parallel.For(0, length, index =>
            {
                string videoId = string.Format("VideoId_{0:X}", 0x50BAD15900010301 + index);
                infos[index] = StorageDownloader.Instance.GetVideoBaseInfom(videoId, 2, new DateTime(2012, 12, 31, 1, 53, 30), new DateTime(2013, 1, 2, 23, 55, 30));
            }
            //);
            Console.WriteLine(DateTime.Now - start);
            for (int i = 0; i < infos.Length; i++)
            {
                Assert.AreEqual(2, infos[i].Header.Length);
                Assert.AreEqual((3600 * 10 / 5) * ((20 + 6) + 4 * (20 + 3)), infos[i].Length);
            }
        }
        
        [TestMethod]
        public void TestStorageDownloader_LargeHeader()
        {
            var baseInfo = StorageDownloader.Instance.GetVideoBaseInfom("VideoId_Large_50BAD15900010701", 2, new DateTime(2016, 4, 5, 12, 50, 0), new DateTime(2016, 4, 5, 12, 51, 0));
            Assert.IsNotNull(baseInfo);
            Assert.AreEqual(50, baseInfo.Header.Length);
            Assert.AreEqual(new DateTime(2016, 4, 5, 12, 40, 50, 123), baseInfo.Time);
        }

        [TestMethod]
        public void TestStorageDownloader_LargeFile()
        {
            var packet = StorageDownloader.Instance.GetVideoPacket("VideoId_Large_50BAD15900010701", 2, new DateTime(2016, 4, 5, 12, 50, 0));
            Assert.IsNotNull(packet);
            Assert.AreEqual(5, packet.VideoStreams.Length);
            Assert.AreEqual(DataType.StreamDataKey, packet.VideoStreams[0].Type);
            Assert.AreEqual(1920*1080*3, packet.VideoStreams[0].Buffer.Length);
            Assert.AreEqual(new DateTime(2016, 4, 5, 12, 49, 55, 123), packet.VideoStreams[0].Time);
            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(DataType.StreamData, packet.VideoStreams[i].Type);
                Assert.AreEqual(100000, packet.VideoStreams[i].Buffer.Length);
                Assert.AreEqual(new DateTime(2016, 4, 5, 12, 49, 55, 123).AddSeconds(i), packet.VideoStreams[i].Time);
            }
        }

        [TestMethod]
        public void TestStorageDownloader_Large_ContinueDownload()
        {
            DateTime beginTime = new DateTime(2016, 4, 5, 12, 49, 55, 123);
            DateTime time = beginTime;
            DateTime searchTime = new DateTime(2016, 4, 5, 12, 50, 0);
            while (searchTime < beginTime.AddMinutes(20))
            {
                var packet = StorageDownloader.Instance.GetVideoPacket("VideoId_Large_50BAD15900010701", 2, searchTime);
                assertEquals(time, packet);
                time = packet.TimePeriod.EndTime;
                searchTime = time.AddSeconds(1);
            }
        }

        private static void assertEquals(DateTime time, VideoStreamsPacket packet)
        {
            Assert.IsNotNull(packet);
            Assert.AreEqual(5, packet.VideoStreams.Length);
            Assert.AreEqual(DataType.StreamDataKey, packet.VideoStreams[0].Type);
            Assert.AreEqual(1920 * 1080 * 3, packet.VideoStreams[0].Buffer.Length);
            Assert.AreEqual(time, packet.VideoStreams[0].Time);
            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(DataType.StreamData, packet.VideoStreams[i].Type);
                Assert.AreEqual(100000, packet.VideoStreams[i].Buffer.Length);
                Assert.AreEqual(time.AddSeconds(i), packet.VideoStreams[i].Time);
            }
        }
    }
}
