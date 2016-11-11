using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using CCTVDownloadService;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class StorageProgressDownloaderTest
    {
        TimePeriodPacket ti = new TimePeriodPacket(new DateTime(2016, 4, 8, 10, 0, 0, 123), new DateTime(2016, 4, 8, 11, 0, 0, 0));
        [TestMethod]
        public void TestStorageProgressDownloader_Empty()
        {
            assertIsDownLoaded(new DownloadProgressManager(null, null));
            assertIsDownLoaded(new DownloadProgressManager(new TimePeriodPacket[0], null));
            assertIsDownLoaded(new DownloadProgressManager(null, new TimePeriodPacket[0]));
            assertIsDownLoaded(new DownloadProgressManager(new TimePeriodPacket[] { ti }, new TimePeriodPacket[] { ti }));
        }

        [TestMethod]
        public void TestStorageProgressDownloader_First()
        {
            DownloadProgressManager downloader = newStorageProgressDownloaderOne();
            assertIsDownloading(downloader, ti.BeginTime);
        }

        [TestMethod]
        public void TestStorageProgressDownloader_Probe()
        {
            DownloadProgressManager downloader = newStorageProgressDownloaderOne();
            assertProbes(downloader, addMinutes(5), addMinutes(5));
            assertProbes(downloader, addMinutes(30), addMinutes(30));
            assertProbes(downloader, addMinutes(60), addMinutes(0));
        }

        [TestMethod]
        public void TestStorageProgressDownloader_Probes()
        {
            DownloadProgressManager downloader = newStorageProgressDownloaderMore();
            assertIsDownloading(downloader, ti.BeginTime);
            assertProbes(downloader, addMinutes(5), addMinutes(5));
            assertProbes(downloader, addMinutes(25), addMinutes(45));
            assertProbes(downloader, addMinutes(58), addMinutes(0));
        }

        [TestMethod]
        public void TestStorageProgressDownloader_Download()
        {
            DownloadProgressManager downloader = newStorageProgressDownloaderMore();
            downloader.Download(getTi(0, 1));
            assertIsDownloading(downloader, addMinutes(1));
            downloader.Download(getTi(3, 4));
            assertIsDownloading(downloader, addMinutes(1));
            assertProbes(downloader, addMinutes(3), addMinutes(4));
            downloader.Download(getTi(4, 5));
            assertIsDownloading(downloader, addMinutes(5));
        }

        private DownloadProgressManager newStorageProgressDownloaderOne()
        {
            return new DownloadProgressManager(new TimePeriodPacket[] { ti });
        }

        private DownloadProgressManager newStorageProgressDownloaderMore()
        {
            TimePeriodPacket[] tisAll = new TimePeriodPacket[] { getTi(0, 10), getTi(20, 30), getTi(40, 50), getTi(55, 58) };
            TimePeriodPacket[] tisCompleted = new TimePeriodPacket[] { getTi(56, 57), getTi(25, 30), getTi(40, 45) };
            return new DownloadProgressManager(tisAll, tisCompleted);
        }

        private DateTime addMinutes(int minutes)
        {
            return ti.BeginTime.AddMinutes(minutes);
        }

        private TimePeriodPacket getTi(int minutesFrom, int minutesTo)
        {
            return new TimePeriodPacket(addMinutes(minutesFrom), addMinutes(minutesTo));
        }
        
        private static void assertIsDownLoaded(DownloadProgressManager downloader)
        {
            Assert.AreEqual(DateTime.MaxValue, downloader.ProbeTime);
            Assert.IsTrue(downloader.IsDownloaded);
        }

        private static void assertProbes(DownloadProgressManager downloader, DateTime probe, DateTime expProbe)
        {
            downloader.UpdateProbeTime(probe);
            assertIsDownloading(downloader, expProbe);
        }

        private static void assertIsDownloading(DownloadProgressManager downloader, DateTime probeTime)
        {
            Assert.AreEqual(probeTime, downloader.ProbeTime);
            Assert.IsFalse(downloader.IsDownloaded);
        }
    }
}