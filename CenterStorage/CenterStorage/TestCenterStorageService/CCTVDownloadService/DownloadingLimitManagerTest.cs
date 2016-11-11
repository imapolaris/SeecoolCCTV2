using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCTVDownloadService;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class DownloadingLimitManagerTest
    {
        [TestMethod]
        public void TestDownloadingLimitManager()
        {
            Assert.IsTrue(DownloadingLimitManager.Instance.IsDownloadingLess());
            Assert.IsFalse(DownloadingLimitManager.Instance.IsDownloadingMore());
            DownloadingLimitManager.Instance.DownloadingSup = 1;
            DownloadingLimitManager.Instance.DownloadingNum = 2;
            Assert.IsFalse(DownloadingLimitManager.Instance.IsDownloadingLess());
            Assert.IsTrue(DownloadingLimitManager.Instance.IsDownloadingMore());
        }
    }
}
