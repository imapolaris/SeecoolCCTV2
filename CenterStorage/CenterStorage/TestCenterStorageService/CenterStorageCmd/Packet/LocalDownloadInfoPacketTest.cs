using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class LocalDownloadInfoPacketTest
    {
        [TestMethod]
        public void TestLocalDownloadInfoPacket()
        {
            LocalDownloadInfoPacket packet = new LocalDownloadInfoPacket(new VideoInfo("videoId",1), "path");
            var buffer = LocalDownloadInfoPacket.Encode(packet);
            var packet2 = LocalDownloadInfoPacket.Decode(buffer);
            Assert.AreNotSame(packet, packet2);
            Assert.AreEqual(packet.Path, packet2.Path);
            Assert.AreNotSame(packet.Info, packet2.Info);
            Assert.AreEqual(packet.Info.VideoId, packet2.Info.VideoId);
            Assert.AreEqual(packet.Info.VideoName, packet2.Info.VideoName);
            Assert.AreEqual(packet.Info.StreamId, packet2.Info.StreamId);
        }
    }
}
