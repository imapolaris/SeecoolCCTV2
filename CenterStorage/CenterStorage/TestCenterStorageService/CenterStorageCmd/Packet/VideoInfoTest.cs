using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class VideoInfoTest
    {
        [TestMethod]
        public void TestVideoInfo_Array()
        {
            VideoInfo[] vi = new VideoInfo[]
                {
                    new VideoInfo("id1", 1, "Test1"),
                    new VideoInfo("id2", 2, "Test2"),
                    new VideoInfo("id3", 3),
                };
            byte[] buffer = VideoInfo.EncodeArray(vi);
            var vi2 = VideoInfo.DecodeArray(buffer);
            Assert.AreEqual(vi.Length, vi2.Length);
            for (int i = 0; i < vi.Length; i++)
            {
                Assert.AreEqual(vi[i].VideoId, vi2[i].VideoId);
                Assert.AreEqual(vi[i].StreamId, vi2[i].StreamId);
                Assert.AreEqual(vi[i].VideoName, vi2[i].VideoName);
            }
        }

        [TestMethod]
        public void TestVideoInfo_ArrayIsNull()
        {
            byte[] buffer = VideoInfo.EncodeArray(null);
            var vi2 = VideoInfo.DecodeArray(buffer);
            Assert.AreEqual(0, vi2.Length);
        }
    }
}
