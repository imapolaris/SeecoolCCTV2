using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoBasePacketTest
    {
        [TestMethod]
        public void TestVideoBasePacket()
        {
            VideoBasePacket packet = new VideoBasePacket(new byte[30], new DateTime(2016, 1, 1), 10000);
            byte[] buffer = VideoBasePacket.Encode(packet);
            var packet2 = VideoBasePacket.Decode(buffer);
            AssertAreEqual(packet, packet2);
        }

        [TestMethod]
        public void TestVideoBasePacketLong()
        {
            VideoBasePacket packet = new VideoBasePacket(new byte[30], new DateTime(2016, 1, 1), 1000000000000);
            byte[] buffer = VideoBasePacket.Encode(packet);
            var packet2 = VideoBasePacket.Decode(buffer);
            AssertAreEqual(packet, packet2);
        }

        public static void AssertAreEqual(VideoBasePacket exp, VideoBasePacket act)
        {
            Assert.AreEqual(exp.Header.Length, act.Header.Length);
            Assert.AreEqual(exp.Time, act.Time);
            Assert.AreEqual(exp.Length, act.Length);
        }
    }
}
