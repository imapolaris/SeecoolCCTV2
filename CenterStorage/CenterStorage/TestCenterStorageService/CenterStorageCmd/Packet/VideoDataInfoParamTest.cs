using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoDataInfoParamTest 
    {
        [TestMethod]
        public void TestVideoDataInfoParam()
        {
            VideoInfo[] vis = new VideoInfo[] { new VideoInfo("videoId1", 2), new VideoInfo("videoId2", 1) };
            DateTime begin = new DateTime(2016, 5, 5, 13, 40, 1, 1);
            DateTime end = new DateTime(2016, 5, 5, 13, 45, 0);
            VideoDataInfoParam param = new VideoDataInfoParam("127.0.0.1", 10001, vis, begin, end);
            byte[] buffer = VideoDataInfoParam.Encode(param);
            VideoDataInfoParam param2 = VideoDataInfoParam.Decode(buffer);
            AssertAreEqual(param, param2);
        }

        private void AssertAreEqual(VideoDataInfoParam p1, VideoDataInfoParam p2)
        {
            Assert.AreEqual(p1.SourceIp, p2.SourceIp);
            Assert.AreEqual(p1.SourcePort, p2.SourcePort);
            Assert.AreEqual(p1.BeginTime, p2.BeginTime);
            Assert.AreEqual(p1.EndTime, p2.EndTime);
            Assert.AreEqual(p1.VideoInfos.Length, p2.VideoInfos.Length);
            for(int i = 0; i < p1.VideoInfos.Length; i++)
            {
                Assert.AreEqual(p1.VideoInfos[i].VideoId, p2.VideoInfos[i].VideoId);
                Assert.AreEqual(p1.VideoInfos[i].StreamId, p2.VideoInfos[i].StreamId);
            }
        }
    }
}
