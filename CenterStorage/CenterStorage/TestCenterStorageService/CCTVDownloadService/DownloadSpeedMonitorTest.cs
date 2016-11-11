using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCTVDownloadService;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class DownloadSpeedMonitorTest
    {
        DateTime beginTime = new DateTime(2016, 5, 6, 11, 26, 0);
        [TestMethod]
        public void TestDownloadSpeedMonitor()
        {
            DownloadSpeedMonitor monitor = new DownloadSpeedMonitor();
            Assert.AreEqual(0, monitor.Speed(beginTime));
            addAndCheck(monitor, 0, 10, 0);
            addAndCheck(monitor, 1, 100, 100);
            addAndCheck(monitor, 2, 200, 150);
            addAndCheck(monitor, 3, 300, 200);
            for (int i = 4; i <= 10; i++)
                addAndCheck(monitor, i, 200, 200);
            addAndCheck(monitor, 11, 200, 210);
            addAndCheck(monitor, 12, 200, 210);
            addAndCheck(monitor, 13, 200, 200);
            for (int i = 14; i <= 1000; i++)
            {
                addAndCheck(monitor, i, 200, 200);
            }
        }

        private void addAndCheck(DownloadSpeedMonitor monitor, int seconds, int size, long speed)
        {
            DateTime time = beginTime.AddSeconds(seconds);
            monitor.Add(time, size);
            Assert.AreEqual(speed, monitor.Speed(time));
        }
    }
}
