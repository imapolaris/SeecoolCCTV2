using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd.Url;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.URL
{
    [TestClass]
    public class LocalUrlTest
    {
        [TestMethod]
        public void TestLocalUrl()
        {
            VideoInfo[] vInfos = new VideoInfo[]
            {
                new VideoInfo("video1", 2, "Name1"),
                new VideoInfo("video2", 2),
            };
            IUrl url = new LocalUrl(@"d:\path", vInfos);
            string urlString = url.ToString();
            Console.WriteLine(urlString);
            var urlNew = LocalUrl.Parse(urlString);
            Assert.AreEqual(url.LocalPath, urlNew.LocalPath);
            Assert.AreEqual(url.VideoInfos.Length, urlNew.VideoInfos.Length);
        }

        [TestMethod]
        public void TestLocalUrl_CheckValid()
        {
            VideoInfo[] vInfos = new VideoInfo[]
            {
                new VideoInfo("CCTV1_50BAD15900010301", 1, "Name1"),
                new VideoInfo("CCTV1_50BAD15900010302", 1),
            };
            LocalUrl url = new LocalUrl(@"", vInfos);
            ExceptionManager.CheckInvalidOperationException(url.CheckValid);
            Assert.AreNotEqual("", url.ToString());
        }
    }
}
