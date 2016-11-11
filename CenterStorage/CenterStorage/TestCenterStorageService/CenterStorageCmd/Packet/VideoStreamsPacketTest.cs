using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Collections.Generic;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class VideoStreamsPacketTest
    {
        TimePeriodPacket ti;
        List<StreamPacket> sps;
        [TestMethod]
        public void TestVideoStreamsPacket_Encode()
        {
            VideoStreamsPacket vp = newVideoStreamsPacket();
             byte[] buffer = VideoStreamsPacket.Encode(vp);
            VideoStreamsPacket decodeVp = VideoStreamsPacket.Decode(buffer);
            Assert.AreEqual(ti.BeginTime, decodeVp.TimePeriod.BeginTime);
            Assert.AreEqual(ti.EndTime, decodeVp.TimePeriod.EndTime);
            Assert.AreEqual(sps.Count, vp.VideoStreams.Length);
            for (int i = 0; i < sps.Count; i++)
            {
                Assert.AreEqual(sps[i].Time, vp.VideoStreams[i].Time);
                Assert.AreEqual(sps[i].Type, vp.VideoStreams[i].Type);
                Assert.AreEqual(sps[i].Buffer.Length, vp.VideoStreams[i].Buffer.Length);
            }
        }

        [TestMethod]
        public void TestVideoStreamsPacket_CompareTo()
        {
            VideoStreamsPacket vp = newVideoStreamsPacket();
            Assert.AreEqual(1, vp.CompareTo(null));
            Assert.AreEqual(0, vp.CompareTo(vp));
        }

        VideoStreamsPacket newVideoStreamsPacket()
        {
            ti = new TimePeriodPacket(new DateTime(2010, 1, 2, 3, 4, 5, 6), new DateTime(2011, 2, 3, 4, 5, 6, 7));
            sps = new List<StreamPacket>();
            sps.Add(new StreamPacket(ti.BeginTime, DataType.StreamDataKey, new byte[2000]));
            sps.Add(new StreamPacket(ti.BeginTime.AddSeconds(1), DataType.StreamData, new byte[200]));
            sps.Add(new StreamPacket(ti.BeginTime.AddSeconds(2), DataType.StreamData, new byte[190]));
            sps.Add(new StreamPacket(ti.BeginTime.AddSeconds(3), DataType.StreamData, new byte[100]));
            sps.Add(new StreamPacket(ti.BeginTime.AddSeconds(4), DataType.StreamData, new byte[200]));
            return new VideoStreamsPacket(ti, sps.ToArray());
        }
    }
}
