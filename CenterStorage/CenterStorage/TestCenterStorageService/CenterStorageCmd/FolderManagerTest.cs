using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageService;
using System.IO;
using CenterStorageCmd;
using System.Linq;
using TestCenterStorageService.CCTVDownloadService;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class FolderManagerTest
    {
        [TestMethod]
        public void TestFolderManager_GetVideoHeader()
        {
            Assert.IsNull(FolderManager.GetVideoHeader("", DateTime.MinValue, DateTime.MaxValue));
            Assert.IsNull(FolderManager.GetVideoHeader(@"D:\视频录像\videoId_2\", DateTime.MinValue, DateTime.MaxValue));
            DateTime start = new DateTime(2016, 3, 22, 23, 51, 01, 156);
            Assert.IsNull(FolderManager.GetVideoHeader(@"D:\视频录像\videoId_2\2016\03\22", start.Subtract(TimeSpan.FromMinutes(5)), start));
            Assert.IsNotNull(FolderManager.GetVideoHeader(@"D:\视频录像\videoId_2\2016\03\22", start, start.AddMinutes(3)));
        }

        [TestMethod]
        public void TestFolderManager_InvalidPath()
        {
            Assert.IsFalse(FolderManager.ClearDirectoryInfoAll("i:\\"));
            Assert.IsFalse(FolderManager.ClearDirectoryInfoAll("abc"));
            string path = @"D:\视频录像\invalid position";
            Assert.IsFalse(FolderManager.DeleteDirectoryInfo(path));
            Assert.IsFalse(FolderManager.ClearDirectoryInfoAll(path));
        }

        [TestMethod]
        public void TestFolderManager_DeleteDirectoryInfo()
        {
            BaseInfo.AddOldVideo();
            string path = @"D:\视频录像\videoID_003_2\2002\03\22";
            Directory.CreateDirectory(path);
            string filePath = Path.Combine(path, ".error");
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Assert.IsFalse(FolderManager.DeleteDirectoryInfo(path));
                Assert.IsTrue(Directory.Exists(path));
            }
            Assert.IsTrue(FolderManager.DeleteDirectoryInfo(path));
            Assert.IsFalse(Directory.Exists(path));
            Assert.IsTrue(Directory.Exists(@"d:\视频录像\videoID_003_2\2002\03"));
        }

        [TestMethod]
        public void TestFolderManager_ClearDirectoryInfoAll()
        {
            string path = @"i:\视频录像\videoID_003_2\2003\03\22";
            Assert.IsFalse(Directory.Exists(@"i:\视频录像"));
            Directory.CreateDirectory(path);
            string filePath = Path.Combine(path, ".error");
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                Assert.IsFalse(FolderManager.ClearDirectoryInfoAll(path));
                Assert.IsTrue(Directory.Exists(path));
            }
            Assert.IsTrue(FolderManager.ClearDirectoryInfoAll(path));
            Assert.IsFalse(Directory.Exists(@"i:\视频录像"));
        }

        [TestMethod]
        public void TestFolderManager_GetIndexesFiles()
        {
            Assert.AreEqual(0, FolderManager.GetIndexesFiles(@"w:\").Length);
            Assert.AreEqual(0, FolderManager.GetIndexesFiles(GlobalData.VideoPath("videoId2", 2)).Length);
        }

        [TestMethod]
        public void TestFolderManager_GetLastestTime()
        {
            string path = "D:\\读写测试\\GetLastestTime";
            FolderManager.DeleteDirectoryInfo(path);
            Assert.IsTrue(FolderManager.GetLastestTime(path) == DateTime.MinValue);
            new DirectoryInfo(path).Create();
            Assert.IsTrue(FolderManager.GetLastestTime(path) == DateTime.MinValue);
            string fileName = Path.Combine(path, "text.txt");
            new FileInfo(fileName).Create();
            DateTime lastestTime = FolderManager.GetLastestTime(path);
            Assert.IsTrue(lastestTime < DateTime.Now);
            Assert.IsTrue(lastestTime.AddSeconds(1) > DateTime.Now);
            FolderManager.DeleteDirectoryInfo(path);
        }

        [TestMethod]
        public void TestFolderManager_GetIndexesPackets()
        {
            string path = "D:\\读写测试\\GetIndexesPackets";
            var sPackets = DownloadRecorderTest.Recorder(path);
            var packets = FolderManager.GetIndexesPackets(path);
            Assert.AreEqual(1, packets.Length);
            Assert.AreEqual(50 + 8 + 4 + 8, packets[0].StartIndex);
            Assert.AreEqual(sPackets.First(_ => _.Type == DataType.StreamDataKey).Time, packets[0].BeginTime);
            Assert.AreEqual(sPackets.Last().Time.AddSeconds(1), packets[0].EndTime);
            FolderManager.DeleteDirectoryInfo(path);
        }

        [TestMethod]
        public void TestFolderManager_GetIndexesPacket()
        {
            string path = "D:\\读写测试\\GetIndexesPacketsInvalid";
            new DirectoryInfo(path).Create();
            string fileName = null;
            Assert.IsNull(FolderManager.GetIndexesPacket(path, new DateTime(2016, 1, 1), ref fileName));
        }

        [TestMethod]
        public void TestFolderManager_GetLocalVideoInfoPacket()
        {
            ExceptionManager.CheckException(() => FolderManager.GetLocalVideoInfoPacket(""));
            ExceptionManager.CheckException(() => FolderManager.GetLocalVideoInfoPacket(@"d:\"));
            var packet = FolderManager.GetLocalVideoInfoPacket(@"D:\读写测试\Time_201607190900_201607191000");
            Assert.AreEqual(new DateTime(2016, 7, 19, 9, 0, 0), packet.TimePeriod.BeginTime);
            Assert.AreEqual(2, packet.ValidTimePeriods.Length);
        }
    }
}