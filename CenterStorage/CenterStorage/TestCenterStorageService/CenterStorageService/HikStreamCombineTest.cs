using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageService;
using System.Linq;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class HikStreamCombineTest
    {
            DateTime beginTime = new DateTime(2016, 6, 2, 11, 00, 10);
        [TestMethod]
        public void TestHikStreamCombine_ShortOrLarge()
        {
            HikStreamCombine hsc = new HikStreamCombine();

            var array = hsc.Update(beginTime, new byte[5]);
            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(5, array[0].First().Buffer.Length);
            Assert.AreEqual(beginTime, array[0].First().Time);
            beginTime = beginTime.AddSeconds(1);
            var largeArr = hsc.Update(beginTime, new byte[50000000]);
            Assert.AreEqual(1, largeArr.Count);
            Assert.AreEqual(50000000, largeArr[0].First().Buffer.Length);
            Assert.AreEqual(beginTime, largeArr[0].First().Time);
        }

        [TestMethod]
        public void TestHikStreamCombine_Combine()
        {
            HikStreamCombine hsc = new HikStreamCombine();
            Assert.AreEqual(0, hsc.Update(beginTime, new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(1), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(2), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(3), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(4), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(5), new byte[5120]).Count);
            var array = hsc.Update(beginTime.AddMilliseconds(6), new byte[1120]);
            Assert.AreEqual(1, array.Count);
            Assert.AreEqual(beginTime, array[0].First().Time);
            Assert.AreEqual(5120*6 + 1120, array[0].Sum(_=>_.Buffer.Length));
        }
        
        [TestMethod]
        public void TestHikStreamCombine_CombineAndLarge()
        {
            HikStreamCombine hsc = new HikStreamCombine();
            Assert.AreEqual(0, hsc.Update(beginTime, new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(1), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(2), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(3), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(4), new byte[5120]).Count);
            Assert.AreEqual(0, hsc.Update(beginTime.AddMilliseconds(5), new byte[5120]).Count);
            var array = hsc.Update(beginTime.AddMilliseconds(6), new byte[100000000]);
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(beginTime, array[0].First().Time);
            Assert.AreEqual(5120 * 6, array[0].Sum(_=>_.Buffer.Length));
            Assert.AreEqual(beginTime.AddMilliseconds(6), array[1].First().Time);
            Assert.AreEqual(100000000, array[1].First().Buffer.Length);
        }
    }
}
