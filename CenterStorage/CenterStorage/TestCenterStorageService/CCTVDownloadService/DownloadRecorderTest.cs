using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Linq;
using System.IO;
using CCTVDownloadService;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class DownloadRecorderTest
    {
        [TestMethod]
        public void TestDownloadRecorder()
        {
            string path = "D:\\读写测试\\TestDownloadRecorder";
            StreamPacket[] sPackets = Recorder(path);

            VideoStreamsPacket packet = FolderManager.GetVideoStreamsPacket(path, sPackets.First(_ => _.Type == DataType.StreamDataKey).Time.AddSeconds(1));
            Assert.AreEqual(sPackets.First(_=>_.Type == DataType.StreamDataKey).Time, packet.TimePeriod.BeginTime);
            Assert.AreEqual(sPackets.Last().Time.AddSeconds(1), packet.TimePeriod.EndTime);
            Assert.AreEqual(5, packet.VideoStreams.Length);
            for (int i = 0; i < packet.VideoStreams.Length; i++)
                assertAreEqual(sPackets[i + 2], packet.VideoStreams[i]);
            FolderManager.DeleteDirectoryInfo(path);
        }

        static void assertAreEqual(StreamPacket exp, StreamPacket act)
        {
            Assert.AreEqual(exp.Time, act.Time);
            Assert.AreEqual(exp.Type, act.Type);
            Assert.AreEqual(exp.Buffer.Length, act.Buffer.Length);
        }

        public static StreamPacket[] Recorder(string path)
        {
            FolderManager.DeleteDirectoryInfo(path);
            StreamPacket[] sPackets = GetStreamPackets();
            DownloadRecorder recorder = new DownloadRecorder(path, TimeSpan.FromMinutes(15));

            for (int i = 0; i < sPackets.Length; i++)
                recorder.Set(sPackets[i]);
            recorder.FinishPackage(sPackets.Last().Time.AddSeconds(1));
            return sPackets;
        }

        public static StreamPacket[] GetStreamPackets()
        {
            DateTime beginTime = new DateTime(2016, 5, 6);
            return new StreamPacket[] {
                new StreamPacket(beginTime, DataType.SysHead, new byte[50]),
                new StreamPacket(beginTime.AddSeconds(0), DataType.StreamData, new byte[20]),
                new StreamPacket(beginTime.AddSeconds(1), DataType.StreamDataKey, new byte[200]),
                new StreamPacket(beginTime.AddSeconds(2), DataType.StreamData, new byte[20]),
                new StreamPacket(beginTime.AddSeconds(3), DataType.StreamData, new byte[20]),
                new StreamPacket(beginTime.AddSeconds(4), DataType.StreamData, new byte[20]),
                new StreamPacket(beginTime.AddSeconds(5), DataType.StreamData, new byte[20]),
            };
        }
    }
}
