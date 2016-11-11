using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class StreamPacketTest
    {
        [TestMethod]
        public void TestStreamPacket()
        {
            DateTime beginTime = new DateTime(2020, 1, 2, 3, 4, 5, 6);
            byte[] sBuffer = new byte[] { 11, 12, 13, 14, 15 };
            StreamPacket sData = new StreamPacket(beginTime, DataType.StreamData, sBuffer);
            byte[] buffer = StreamPacket.Encode(sData);
            StreamPacket newData = StreamPacket.Decode(buffer);

            Assert.AreNotSame(sData, newData);
            Assert.AreNotSame(sData.Buffer, newData.Buffer);

            Assert.AreEqual(sData.Time, newData.Time);
            Assert.AreEqual(sData.Type, sData.Type);
            Assert.AreEqual(sData.Buffer.Length, newData.Buffer.Length);
            for (int i = 0; i < sData.Buffer.Length; i++)
                Assert.AreEqual(sData.Buffer[i], newData.Buffer[i]);
        }
    }
}
