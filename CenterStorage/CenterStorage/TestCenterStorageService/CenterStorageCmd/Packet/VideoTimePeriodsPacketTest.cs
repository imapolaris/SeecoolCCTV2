using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoTimePeriodsPacketTest
    {
        [TestMethod]
        public void TestVideoTimePeriodsPacket()
        {
            VideoTimePeriodsPacket packet = new VideoTimePeriodsPacket(new VideoInfo("id", 2), new TimePeriodPacket[0]);
            var buffer = VideoTimePeriodsPacket.Encode(packet);
            VideoTimePeriodsPacket packet2 = VideoTimePeriodsPacket.Decode(buffer);
            AssertAreEqual(packet, packet2);
        }

        public static void AssertAreEqual(VideoTimePeriodsPacket exp, VideoTimePeriodsPacket act)
        {
            Assert.AreNotSame(exp, act);
            Assert.AreEqual(exp.VideoId, act.VideoId);
            Assert.AreEqual(exp.StreamId, act.StreamId);
            TimePeriodPacketTest.AssertAreEqualArray(exp.TimePeriods, act.TimePeriods);
        }
    }
}
