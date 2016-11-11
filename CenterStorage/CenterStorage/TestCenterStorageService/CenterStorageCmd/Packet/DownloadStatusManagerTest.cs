using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Linq;

namespace TestCenterStorageService.CenterStorageCmd
{
    delegate bool StatusDelegate(DownloadStatus status);
    [TestClass]
    public class DownloadStatusManagerTest
    {
        [TestMethod]
        public void TestDownloadStatusManager()
        {
            foreach (var status in Enum.GetValues(typeof(DownloadStatus)))
                Assert.IsFalse(string.IsNullOrEmpty(DownloadStatusManager.ToHanZi((DownloadStatus)status)));
            string.IsNullOrEmpty(DownloadStatusManager.ToHanZi((DownloadStatus)10));
            statusCheck(DownloadStatusManager.IsWaitingOrPause, DownloadStatus.Waiting, DownloadStatus.Paused);
            statusCheck(DownloadStatusManager.IsDownloading, DownloadStatus.Downloading, DownloadStatus.Ready);
            statusCheck(DownloadStatusManager.IsDownloadingOrWaiting, DownloadStatus.Waiting, DownloadStatus.Downloading, DownloadStatus.Ready);
            statusCheck(DownloadStatusManager.IsPauseOrEnd, DownloadStatus.Completed, DownloadStatus.Error, DownloadStatus.Deleted, DownloadStatus.Paused);
            statusCheck(DownloadStatusManager.IsEndOfDownload, DownloadStatus.Completed, DownloadStatus.Error, DownloadStatus.Deleted);
        }

        static void statusCheck(StatusDelegate action, params DownloadStatus[] statusArray)
        {
            foreach (var obj in Enum.GetValues(typeof(DownloadStatus)))
            {
                DownloadStatus status = (DownloadStatus)obj;
                if (statusArray.Any(_=>_ == status))
                    Assert.IsTrue(action(status));
                else
                    Assert.IsFalse(action(status));
            }
        }
        
    }
}
