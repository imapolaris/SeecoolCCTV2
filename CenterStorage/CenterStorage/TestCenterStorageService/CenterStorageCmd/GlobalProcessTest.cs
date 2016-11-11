using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class GlobalProcessTest
    {
        [TestMethod]
        public void Test_GlobalProcess_FolderPath()
        {
            Assert.AreEqual(@"2016\03\29", GlobalProcess.FolderPath(new DateTime(2016, 3, 29, 11, 33, 20, 555)));
        }

        [TestMethod]
        public void TestGlobalProcess_IsYear()
        {
            Assert.IsTrue(GlobalProcess.IsYear("2016"));
            Assert.IsFalse(GlobalProcess.IsYear("201A"));
            Assert.IsFalse(GlobalProcess.IsYear("0000"));
            Assert.IsFalse(GlobalProcess.IsYear("10000"));
            Assert.IsFalse(GlobalProcess.IsYear("997"));
            Assert.IsFalse(GlobalProcess.IsYear("04"));;
        }

        [TestMethod]
        public void TestGlobalProcess_IsMonth()
        {
            Assert.IsTrue(GlobalProcess.IsMonth("12"));
            Assert.IsFalse(GlobalProcess.IsMonth("2016"));
            Assert.IsFalse(GlobalProcess.IsMonth("1A"));
            Assert.IsFalse(GlobalProcess.IsMonth("13"));
            Assert.IsTrue(GlobalProcess.IsMonth("01"));
            Assert.IsFalse(GlobalProcess.IsMonth("00"));
        }

        [TestMethod]
        public void TestGlobalProcess_IsDay()
        {
            Assert.IsTrue(GlobalProcess.IsDay("31"));
            Assert.IsFalse(GlobalProcess.IsDay("2016"));
            Assert.IsFalse(GlobalProcess.IsDay("1A"));
            Assert.IsFalse(GlobalProcess.IsDay("32"));
            Assert.IsTrue(GlobalProcess.IsDay("01"));
            Assert.IsFalse(GlobalProcess.IsDay("00"));
        }

        [TestMethod]
        public void TestGlobalProcess_FileNameTrans()
        {
            string indexesName = @"D:\视频录像\videoId_2\2016\03\22\20160322235101156.Indexes";
            string recName = @"D:\视频录像\videoId_2\2016\03\22\20160322235101156.rec";
            Assert.AreEqual(recName, GlobalProcess.GetRecFileName(indexesName));
            Assert.AreEqual(indexesName, GlobalProcess.GetIndexesFileName(recName));
        }

        [TestMethod]
        public void TestGlobalProcess_TimeFormatOfCn()
        {
            string cn = GlobalProcess.TimeFormatOfCn(new DateTime(2016, 5, 6, 10, 14, 15, 1));
            Assert.AreEqual("2016年05月06日10时14分15秒", cn);
        }

        [TestMethod]
        public void TestGlobalProcess_GetFolderName()
        {
            TimePeriodPacket tpp = new TimePeriodPacket(new DateTime(2016, 7, 25, 18, 15, 0), new DateTime(2016, 7, 25, 18, 16, 10));
            string folderName = GlobalProcess.GetFolderName(tpp);
            Assert.AreEqual("录像_201607251815_201607251816", folderName);
        }
    }
}