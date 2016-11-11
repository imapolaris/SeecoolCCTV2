using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageService;

namespace TestCenterStorageService.CenterStorageService
{
    [TestClass]
    public class MultiVideoDisplayerTest
    {
        MultiVideoDisplayer disps = new MultiVideoDisplayer();
        [TestMethod]
        public void TestMultiVideoDisplayer_Empty()
        {
            Assert.AreEqual(0, disps.Count);
        }
        [TestMethod]
        public void TestMultiVideoDisplayer()
        {
            //disps.Add("CCTV1_50BAD15900010301", 2);
            //disps.Add("CCTV1_50BAD15900010302", 1);
            //disps.Add("CCTV1_50BAD15900010303", 2);
            //Assert.AreEqual(3, disps.Count);

            //disps.Add("CCTV1_50BAD15900010301", 2);
            //Assert.AreEqual(3, disps.Count);

            //Assert.IsTrue(disps.Exist("CCTV1_50BAD15900010301", 2));
            //Assert.IsTrue(disps.Exist("CCTV1_50BAD15900010302", 1));
            //Assert.IsTrue(disps.Exist("CCTV1_50BAD15900010303", 2));
            //Assert.IsFalse(disps.Exist("CCTV1_50BAD15900010301", 1));

            //disps.Remove("CCTV1_50BAD15900010301", 1);
            //Assert.AreEqual(3, disps.Count);
            //disps.Remove("CCTV1_50BAD15900010301", 2);
            //Assert.AreEqual(2, disps.Count);
            //Assert.IsFalse(disps.Exist("CCTV1_50BAD15900010301", 2));
            //disps.Dispose();
            //Assert.AreEqual(0, disps.Count);
        }
    }
}