using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using CenterStorageCmd.Url;

namespace TestCenterStorageService.CenterStorageCmd.URL
{
    [TestClass]
    public class RemoteUrlTest
    {
        [TestMethod]
        public void TestRemoteUrl()
        {
            VideoInfo[] vInfos = new VideoInfo[]
           {
                new VideoInfo("video1", 2, "Name1"),
                new VideoInfo("video2", 2),
           };
            IUrl url = new RemoteUrl("127.0.0.1", 10000, new DateTime(2016,1,1), new DateTime(2016,1,2), vInfos, @"d:\path");
            string urlString = url.ToString();
            Console.WriteLine(urlString);
            var urlNew = RemoteUrl.Parse(urlString);
            Assert.AreEqual(url.LocalPath, urlNew.LocalPath);
            Assert.AreEqual(url.VideoInfos.Length, urlNew.VideoInfos.Length);
            (url as RemoteUrl).CheckValid();
        }

        [TestMethod]
        public void TestRemoteUrl_CheckValid()
        {
            VideoInfo[] vInfos = new VideoInfo[]
            {
                new VideoInfo("CCTV1_50BAD15900010301", 1, "Name1"),
                new VideoInfo("CCTV1_50BAD15900010302", 1),
            };
            new RemoteUrl("127.0.0.1", 10000, new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), vInfos, @"d:\path").CheckValid();
            RemoteUrl url = new RemoteUrl("127.0.0.1", 100000, new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), vInfos, @"d:\path");
            ExceptionManager.CheckInvalidOperationException(url.CheckValid);
            ExceptionManager.CheckInvalidOperationException(new RemoteUrl("127.0.0.1", 10, new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), vInfos, @"d:\path").CheckValid);
            url = new RemoteUrl("192.168.257.1", 10000, new DateTime(2016, 1, 1), new DateTime(2016, 1, 2), vInfos, @"d:\path");
            ExceptionManager.CheckInvalidOperationException(url.CheckValid);
            url = new RemoteUrl("192.168.1.1", 10000, new DateTime(2016, 1, 1), new DateTime(2015, 1, 2), vInfos, @"d:\path");
            ExceptionManager.CheckInvalidOperationException(url.CheckValid);
        }
        [TestMethod]
        public void TestRemoteUrl_Pause()
        {
            string urlStr = @"CCTV2:REMOTE/time=63587203200-63587289600/source=127.0.0.1:10000/path=d:\path/videos=video1,2,Name1|video2,2";
            var url = RemoteUrl.Parse(urlStr);
            Assert.AreEqual(2, url.VideoInfos.Length);
            string invalidUrlStr0 = @"CCTV1:REMOTE/time=63587203200-63587289600/source=127.0.0.1:10000/path=d:\path/videos=video1,2,Name1|video2,2";
            ExceptionManager.CheckInvalidOperationException(()=>RemoteUrl.Parse(invalidUrlStr0));
            string invalidUrlStr1 = @"CCTV2:LOCAL/time=63587203200-63587289600/source=127.0.0.1:10000/path=d:\path/videos=video1,2,Name1|video2,2";
            Assert.IsNull(RemoteUrl.Parse(invalidUrlStr1));
            string invalidUrlStr2 = @"CCTV2:REMOTE/time=63587203200-63587289600/source=127.0.0.1:10000/path=d:\path/videos=video1,2,Name1|video2";
            ExceptionManager.CheckInvalidOperationException(() => RemoteUrl.Parse(invalidUrlStr2));
            string invalidUrlStr3 = @"CCTV2:REMOTE/time=63587203200-63587289600/source=127.0.0.1:10000/";
            var url3 = RemoteUrl.Parse(invalidUrlStr3);
            Assert.AreEqual(invalidUrlStr3, url3.ToString());
        }
    }
}
