using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class IndexesDataTest
    {
        [TestMethod]
        public void TestIndexesData＿IndexesPacket()
        {
            DateTime beginTime = new DateTime(2020, 1, 2, 3, 4, 5, 6);
            DateTime endTime = new DateTime(2026, 5, 4, 3, 2, 1, 9);
            long beginIndex = 10000;
            IndexesPacket iData = new IndexesPacket(beginTime, endTime, beginIndex);
            byte[] buffer = IndexesPacket.Encode(iData);
            IndexesPacket newData = IndexesPacket.Decode(buffer);
            Assert.AreNotSame(iData, newData);
            Assert.AreEqual(iData.BeginTime, newData.BeginTime);
            Assert.AreEqual(iData.EndTime, newData.EndTime);
            Assert.AreEqual(iData.StartIndex, newData.StartIndex);
        }
    }
}
