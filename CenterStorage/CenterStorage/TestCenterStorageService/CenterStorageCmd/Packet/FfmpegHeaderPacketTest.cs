using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class FfmpegHeaderPacketTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            FfmpegHeaderPacket packet1 = new FfmpegHeaderPacket(1, "streamName", "streamUrl", 80, 1000, 600);
            byte[] buffer = FfmpegHeaderPacket.Encode(packet1);
            Assert.IsNull(HikHeaderPacket.Decode(buffer));
            FfmpegHeaderPacket packet2 = FfmpegHeaderPacket.Decode(buffer);
            Assert.AreNotSame(packet1, packet2);
            Assert.AreEqual(packet1.CodecID, packet2.CodecID);
            Assert.AreEqual(packet1.StreamId, packet2.StreamId);
            Assert.AreEqual(packet1.StreamName, packet2.StreamName);
            Assert.AreEqual(packet1.Width, packet2.Width);
            Assert.AreEqual(packet1.Height, packet2.Height);
            Assert.AreEqual(packet1.StreamUrl, packet2.StreamUrl);
        }
    }
}
