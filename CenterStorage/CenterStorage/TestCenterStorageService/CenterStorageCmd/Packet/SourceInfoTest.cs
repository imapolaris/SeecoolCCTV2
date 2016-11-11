using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class SourceInfoTest
    {
        [TestMethod]
        public void TestSourceInfo()
        {
            string sourceIp = "test";
            int sourcePort = 10001;
            ISourceInfo sourceInfo = new SourceInfo(sourceIp, sourcePort);
            Assert.AreEqual(sourceIp, sourceInfo.SourceIp);
            Assert.AreEqual(sourcePort, sourceInfo.SourcePort);
        }
    }
}
