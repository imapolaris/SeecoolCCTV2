using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCTVDownloadService;
using System.IO;
using CenterStorageCmd;

namespace TestCenterStorageService.CCTVDownloadService
{
    [TestClass]
    public class DownloadsConfigFileTest
    {
        [TestMethod]
        public void TestDownloadsConfigFile()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Seecool\\CCTVDownload");
            string fileName = System.IO.Path.Combine(path, "data.memory");
            new FileInfo(fileName).Delete();
            //empty
            var array = DownloadsConfigFile.Instance.ReadConfig();
            Assert.IsNotNull(array);
            Console.WriteLine(array.Length);
            DownloadsConfigFile.Instance.SetConfig(array);
            new FileInfo(fileName).Delete();
            ////not empty
            DownloadMemoryData[] memory = new DownloadMemoryData[] {
                new DownloadMemoryData(new DownloadInfoParam("127.0.0.1", 10001, new DateTime(2010,1,1), new DateTime(2010,1,1,1,0,0), "videoId1",2, "path1", null), DownloadStatus.Deleted, true, null),
                new DownloadMemoryData(new DownloadInfoParam("127.0.0.1", 10002, new DateTime(2011,1,1), new DateTime(2011,1,1,1,0,0), "videoId2",2, "path2", null), DownloadStatus.Error, true, "error"),
            };
            DownloadsConfigFile.Instance.SetConfig(memory);
            var act = DownloadsConfigFile.Instance.ReadConfig();
            Assert.AreEqual(2, act.Length);
            new FileInfo(fileName).Delete();
        }
    }
}