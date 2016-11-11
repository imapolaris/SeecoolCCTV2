using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class HikHeaderPacketTest
    {
        [TestMethod]
        public void TestHikHeaderPacket()
        {
            HikHeaderPacket packet1 = new HikHeaderPacket(1, "test", "streamUrl", 1, new byte[10]);
            byte[] buffer = HikHeaderPacket.Encode(packet1);
            var packet2 = HikHeaderPacket.Decode(buffer);
            Assert.IsNull(FfmpegHeaderPacket.Decode(buffer));

            Assert.AreNotSame(packet1, packet2);
            Assert.AreEqual(packet1.Header.Length, packet2.Header.Length);
            Assert.AreEqual(packet1.StreamId, packet2.StreamId);
            Assert.AreEqual(packet1.StreamName, packet2.StreamName);
            Assert.AreEqual(packet1.StreamUrl, packet2.StreamUrl);
            Assert.AreEqual(packet1.Type, packet2.Type);
        }
    }
}
