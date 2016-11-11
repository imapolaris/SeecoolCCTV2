using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class LocalVideosInfoPacketTest
    {
        [TestMethod]
        public void TestLocalVideosInfoPacket()
        {
            TimePeriodPacket tpp = new TimePeriodPacket(new DateTime(2016, 7, 25), new DateTime(2016, 7, 25, 17, 0, 0));
            VideoTimePeriodsPacket[] vtpps = new VideoTimePeriodsPacket[]
            {
                    new VideoTimePeriodsPacket(new VideoInfo("id", 2), new TimePeriodPacket[0]),
                    new VideoTimePeriodsPacket(new VideoInfo("id2", 2), new TimePeriodPacket[]
                    {
                        new TimePeriodPacket(new DateTime(2016, 7, 25), new DateTime(2016, 7, 25, 16, 40, 0))
                    })
            };
            LocalVideosInfoPacket packet = new LocalVideosInfoPacket(tpp, vtpps);
            byte[] buffer = LocalVideosInfoPacket.Encode(packet);
            var packet2 = LocalVideosInfoPacket.Decode(buffer);
            Assert.AreNotSame(packet, packet2);
            TimePeriodPacketTest.AssertAreEqual(packet.TimePeriod, packet2.TimePeriod);

            Assert.AreNotSame(packet.ValidTimePeriods, packet2.ValidTimePeriods);
            Assert.AreEqual(packet.ValidTimePeriods.Length, packet2.ValidTimePeriods.Length);
            for(int i = 0; i < packet.ValidTimePeriods.Length; i++)
                VideoTimePeriodsPacketTest.AssertAreEqual(packet.ValidTimePeriods[i], packet2.ValidTimePeriods[i]);
        }
    }
}
