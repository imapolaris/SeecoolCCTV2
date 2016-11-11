using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Collections.Generic;
using System.IO;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class TimePeriodPacketTest
    {
        [TestMethod]
        public void TestTimePeriodPacket()
        {
            TimePeriodPacket packet = new TimePeriodPacket(new DateTime(2010, 5, 3, 13, 35, 20, 123), new DateTime(2016, 5, 3, 13, 36, 0, 3));
            byte[] buffer = TimePeriodPacket.Encode(packet);
            var act = TimePeriodPacket.Decode(buffer);
            AssertAreEqual(packet, act);
        }

        [TestMethod]
        public void TestTimePeriodPacketArrayNull()
        {
            var buffer = TimePeriodPacket.EncodeArray(null);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                var tpps = TimePeriodPacket.DecodeArray(ms);
                Assert.AreEqual(0, tpps.Length);
            }
            var buffer2 = TimePeriodPacket.EncodeArray(new TimePeriodPacket[0]);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                var tpps = TimePeriodPacket.DecodeArray(ms);
                Assert.AreEqual(0, tpps.Length);
            }
        }

        [TestMethod]
        public void TestTimePeriodPacketArray()
        {
            List<TimePeriodPacket> packets = new List<TimePeriodPacket>();
            for(int i = 2010; i < 2016; i++)
                packets.Add(new TimePeriodPacket(new DateTime(i, 5, 3, 13, 35, 20, 123), new DateTime(i+1, 5, 3, 13, 36, 0, 3)));
            byte[] buffer = TimePeriodPacket.EncodeArray(packets.ToArray());
            var p = TimePeriodPacket.DecodeArray(new MemoryStream(buffer));
            AssertAreEqualArray(packets.ToArray(), p);
        }

        [TestMethod]
        public void TestTimePeriodPacket_CompareTo()
        {
            TimePeriodPacket packet = new TimePeriodPacket(new DateTime(2010, 5, 3, 13, 35, 20, 123), new DateTime(2016, 5, 3, 13, 36, 0, 3));
            Assert.AreEqual(1, packet.CompareTo(null));
            Assert.AreEqual(0, packet.CompareTo(packet));
        }

        [TestMethod]
        public void TestTimePeriodPacket_IsInRange()
        {
            DateTime begin = new DateTime(2010, 5, 3, 13, 35, 20, 123);
            DateTime end = new DateTime(2016, 5, 3, 13, 36, 0, 3);
            TimePeriodPacket packet = new TimePeriodPacket(begin, end);
            Assert.IsTrue(packet.IsInRange(begin));
            Assert.IsFalse(packet.IsInRange(end));
            Assert.IsTrue(packet.IsInRange(end.Subtract(TimeSpan.FromMilliseconds(1))));
            Assert.IsFalse(packet.IsInRange(begin.Subtract(TimeSpan.FromMilliseconds(1))));
            Assert.IsFalse(packet.IsInRange(end.Add(TimeSpan.FromHours(1))));
            Assert.AreEqual(0, packet.CompareTo(packet));
        }

        public static void AssertAreEqual(ITimePeriod p1, ITimePeriod p2)
        {
            Assert.AreNotSame(p1, p2);
            Assert.AreEqual(p1.BeginTime, p2.BeginTime);
            Assert.AreEqual(p1.EndTime, p2.EndTime);
        }

        public static void AssertAreEqualArray(ITimePeriod[] ps1, ITimePeriod[] ps2)
        {
            Assert.AreEqual(ps1.Length, ps2.Length);
            for (int i = 0; i < ps1.Length; i++)
                AssertAreEqual(ps1[i], ps2[i]);
        }
    }
}
