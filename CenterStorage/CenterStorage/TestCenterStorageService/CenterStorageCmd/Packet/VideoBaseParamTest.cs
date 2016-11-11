using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoBaseParamTest
    {
        [TestMethod]
        public void TestVideoBaseParam()
        {
            IVideoBaseInfom param = new VideoBaseInfomParam(new VideoInfo("id", 2,"测试"), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2011, 2, 2)));
            var buffer = VideoBaseInfomParam.Encode(param);
            IVideoBaseInfom param2 = VideoBaseInfomParam.Decode(buffer);
            Assert.AreEqual(param.VideoId, param2.VideoId);
            Assert.AreEqual(param.StreamId, param2.StreamId);
            Assert.AreEqual(param.BeginTime, param2.BeginTime);
            Assert.AreEqual(param.EndTime, param2.EndTime);
            Assert.AreEqual(param.VideoName, param2.VideoName);
        }
    }
}
