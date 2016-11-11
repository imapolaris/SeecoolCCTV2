using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoDataParamTest
    {
        [TestMethod]
        public void TestVideoDataParam()
        {
            VideoDataParam param = new VideoDataParam(new VideoInfo("video", 2), new DateTime(2016, 5, 5));
            var buffer = VideoDataParam.Encode(param);
            VideoDataParam param2 = VideoDataParam.Decode(buffer);
            Assert.AreEqual(param.VideoId, param2.VideoId);
            Assert.AreEqual(param.StreamId, param2.StreamId);
            Assert.AreEqual(param.Time, param2.Time);
        }
    }
}
