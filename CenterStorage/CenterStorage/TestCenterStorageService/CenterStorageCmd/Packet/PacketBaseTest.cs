using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.IO;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class PacketBaseTest
    {
        //[TestMethod]
        //public void TestTime()
        //{
        //    DateTime time = new DateTime(2016, 5, 3, 11, 28, 10, 123);
        //    byte[] buffer = null;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        PacketBase.WriteBytes(ms, time);
        //        buffer = ms.ToArray();
        //    }
        //    DateTime t;
        //    using (MemoryStream ms = new MemoryStream(buffer))
        //    {
        //        t = PacketBase.ReadTime(ms);
        //    }
        //    Assert.AreEqual(time, t);
        //}

        [TestMethod]
        public void TestPacketBase_DateTime()
        {
            DateTime time = new DateTime(2016, 5, 25, 17, 28, 10, 123);
            var buffer = PacketBase.GetBytes(time);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                var time2 = PacketBase.ReadTime(ms);
                Assert.AreEqual(time, time2);
            }
        }

        [TestMethod]
        public void TestPacketBase_string_null()
        {
            string str = null;
            byte[] nullBuffer = PacketBase.GetBytes(str);
            using (MemoryStream ms = new MemoryStream(nullBuffer))
            {
                Assert.IsNull(PacketBase.ReadString(ms));
            }
            str = string.Empty;
            byte[] buffer = PacketBase.GetBytes(str);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                Assert.AreEqual(string.Empty, PacketBase.ReadString(ms));
            }
        }

        [TestMethod]
        public void TestPacketBase_string_more()
        {
            string str = "testString";
            byte[] nullBuffer = PacketBase.GetBytes(str);
            using (MemoryStream ms = new MemoryStream(nullBuffer))
            {
                Assert.AreEqual(str, PacketBase.ReadString(ms));
            }
        }
    }
}
