using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.IO;
using CenterStorageService;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class FileManagerTest
    {
        [TestMethod]
        public void TestFileManager_GetPacketFromFile()
        {
            Assert.IsNull(FileManager.GetStreamPacket("20160322235101156.rec", 1));
        }

        [TestMethod]
        public void TestFileManager_GetTimePeriods()
        {
            BaseInfo.AddData_videoId_003_2_20160330();
            DateTime time = new DateTime(2016, 03, 30);
            string fileName = Path.Combine(GlobalData.Path, @"videoID_003_2", GlobalProcess.FolderPath(time), $"{GlobalProcess.FileNameFromDate(time)}{GlobalProcess.SimpleIndexesFormat()}");
            Assert.IsNotNull(FileManager.GetTimePeriods(fileName));
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                Assert.IsNotNull(FileManager.GetTimePeriods(fileName));
        }

        [TestMethod]
        public void TestFileManager_GetVideoHeader()
        {
            Assert.IsNull(FileManager.GetVideoHeader("w:\\"));
            Assert.IsNull(FileManager.GetVideoHeader(@"D:\视频录像\videoId_2\2016\03\22\20100322235959999.Indexes"));
            Assert.IsNull(FileManager.GetVideoHeader(getPath("videoId", 2, new DateTime(3000, 1, 1))));
            var header = FileManager.GetVideoHeader(getPath("videoID_003", 2, new DateTime(2010, 3, 23, 21, 50, 1, 156)));
            Assert.IsNotNull(header);
            Assert.AreEqual(DataType.SysHead, header.Type);
            Assert.AreEqual(2, header.Buffer.Length);
            Assert.AreEqual(new DateTime(2010, 3, 23, 21, 50, 1, 156), header.Time);
        }

        static string getPath(string videoId, int streamId, DateTime time)
        {
            return Path.Combine(GlobalData.VideoPath(videoId, streamId), GlobalProcess.FolderPath(time), GlobalProcess.FileNameFromTime(time) + GlobalProcess.RecFormat());
        }

        //[TestMethod]
        //public void TestFileStreamReadWrite()
        //{
        //    string path = @"d:\读写测试\rw.text";
        //    FileInfo file = new FileInfo(path);
        //    file.Directory.Create();
        //    using (FileStream write = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
        //    {
        //        write.Write(new byte[4097], 0, 1);
        //        using (FileStream read = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            for (int i = 0; i < 100; i++)
        //            {
        //                write.Write(new byte[100000], 0, 100000);
        //                Assert.AreEqual(100000 * (i + 1) + 1, read.Length);
        //            }
        //        }
        //    }
        //}
        [TestMethod]
        public void TestFileManagerTest_GetIndexesPackets()
        {
            Assert.AreEqual(0, FileManager.GetIndexesPackets(@"d:\invalid.invalid").Length);
        }
    }
}