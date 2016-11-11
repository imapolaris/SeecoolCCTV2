using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoBaseFileRecorderTest
    {
        DateTime beginTime;
        VideoBasePacket basePacket;
        VideoTimePeriodsPacket packet;
        [TestInitialize]
        public void Setup()
        {
            beginTime = new DateTime(2016, 5, 6, 13, 58, 0);
            basePacket = new VideoBasePacket(new byte[] { 1, 2, 3, 5, 6, 7, 128, 200, 0, 15 }, beginTime, 1000000);
            var tpps = new TimePeriodPacket[] {
                new TimePeriodPacket(beginTime.AddMinutes(1), beginTime.AddMinutes(10)),
                new TimePeriodPacket(beginTime.AddMinutes(15), beginTime.AddMinutes(30)),
                };
            packet = new VideoTimePeriodsPacket(new VideoInfo("test", 1), tpps);
        }

        [TestMethod]
        public void TestVideoBaseFileRecorder()
        {
            string path = @"D:\读写测试\VideoBaseFileRecorder";
            FolderManager.DeleteDirectoryInfo(path);
            var downloadInfo = new DownloadInfoParam("ip", 10001, beginTime, beginTime.AddMinutes(30), "videoId", 0, path, "test");
            VideoBaseFileRecorder recorder = new VideoBaseFileRecorder(path);
            recorder.UpdateDownloadInfo(downloadInfo);
            Assert.IsNull(recorder.TimePeriods);
            Assert.IsNull(recorder.VideoBase);

            recorder.UpdateTimePeriods(packet);
            Assert.AreEqual(packet, recorder.TimePeriods);
            recorder.UpdateVideoBase(basePacket);
            Assert.AreEqual(basePacket, recorder.VideoBase);
            DownloadInfoParamTest.AssertAreEqual(downloadInfo, recorder.DownloadInfo);

            VideoBaseFileRecorder recorder1 = new VideoBaseFileRecorder(path);
            VideoBasePacketTest.AssertAreEqual(recorder.VideoBase, recorder1.VideoBase);
            VideoTimePeriodsPacketTest.AssertAreEqual(recorder.TimePeriods, recorder1.TimePeriods);

            FolderManager.DeleteDirectoryInfo(path);
        }

        [TestMethod]
        public void TestVideoBaseFileRecorder_Error()
        {
            string path = @"w:\读写测试\VideoBaseFileRecorder#@";
            VideoBaseFileRecorder recorder = new VideoBaseFileRecorder(path);
            Assert.IsNull(recorder.TimePeriods);
            Assert.IsNull(recorder.VideoBase);
            recorder.UpdateVideoBase(basePacket);
            Assert.AreEqual(basePacket, recorder.VideoBase);
            recorder.UpdateTimePeriods(packet);
            Assert.AreEqual(packet, recorder.TimePeriods);
            VideoBaseFileRecorder recorder1 = new VideoBaseFileRecorder(path);
            Assert.IsNull(recorder1.TimePeriods);
            Assert.IsNull(recorder1.VideoBase);
        }
    }
}
