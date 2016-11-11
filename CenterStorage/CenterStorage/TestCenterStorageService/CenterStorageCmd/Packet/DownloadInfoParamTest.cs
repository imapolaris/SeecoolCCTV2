using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class DownloadInfoParamTest
    {
        [TestMethod]
        public void TestDownloadInfoParam_AreEqual()
        {
            Assert.IsTrue(DownloadInfoParam.AreEqual(null, null));
            DownloadInfoParam p1 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsTrue(DownloadInfoParam.AreEqual(p1, p1));
            Assert.IsFalse(DownloadInfoParam.AreEqual(p1, null));
            Assert.IsFalse(DownloadInfoParam.AreEqual(null, p1));
            DownloadInfoParam p2 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsTrue(DownloadInfoParam.AreEqual(p1, p2));
            p2.UpdatePath("");
            Assert.IsFalse(DownloadInfoParam.AreEqual(p1, p2));
        }

        [TestMethod]
        public void TestDownloadInfoParam_AreEqualIgnorePath()
        {
            DownloadInfoParam p1 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            DownloadInfoParam p2 = new DownloadInfoParam(p1);
            Assert.IsTrue(DownloadInfoParam.AreEqualIgnorePath(null, null));
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, null));
            Assert.IsTrue(DownloadInfoParam.AreEqualIgnorePath(p1, p1));
            Assert.IsTrue(DownloadInfoParam.AreEqualIgnorePath(p1, p2));
            p2.UpdatePath(@"D:\视酷下载");
            Assert.IsTrue(DownloadInfoParam.AreEqualIgnorePath(p1, p2));

            DownloadInfoParam p3 = new DownloadInfoParam("", Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p3));
            DownloadInfoParam p4 = new DownloadInfoParam(Settings.DownloadHost, 0, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p4));
            DownloadInfoParam p5 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p5));
            DownloadInfoParam p6 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 1), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p6));
            DownloadInfoParam p7 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 2, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p7));
            DownloadInfoParam p8 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 2, 13))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            Assert.IsFalse(DownloadInfoParam.AreEqualIgnorePath(p1, p8));
        }
        [TestMethod]
        public void TestDownloadInfoParam_Trans()
        {
            DownloadInfoParam p1 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\ScDownload\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            byte[] buffer = DownloadInfoParam.Encode(p1);
            DownloadInfoParam p2 = DownloadInfoParam.Decode(buffer);
            AssertAreEqual(p1, p2);
        }

        [TestMethod]
        public void TestDownloadInfoParam_Clone()
        {
            DownloadInfoParam p1 = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            DownloadInfoParam p2 = new DownloadInfoParam(p1);
            Assert.AreNotEqual(p1, p2);
            AssertAreEqual(p1, p2);
        }

        [TestMethod]
        public void TestDownloadInfoParamArrayEmpty()
        {
            IDownloadInfo[] p1 = new DownloadInfoParam[0];
            byte[] buffer = DownloadInfoParam.EncodeArray(p1);
            IDownloadInfo[] p2 = DownloadInfoParam.DecodeArray(buffer);
            Assert.AreEqual(p1.Length, p2.Length);

            p1 = null;
            buffer = DownloadInfoParam.EncodeArray(p1);
            p2 = DownloadInfoParam.DecodeArray(buffer);
            Assert.AreEqual(0, p2.Length);
        }

        [TestMethod]
        public void TestDownloadInfoParamArrayMore()
        {
            IDownloadInfo[] p1 = new DownloadInfoParam[]
                {
                    new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2"),
                    new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010702", 2), new TimePeriodPacket(new DateTime(2011, 2, 1), new DateTime(2013, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010702_2"),
                    new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010703", 2), new TimePeriodPacket(new DateTime(2013, 3, 1), new DateTime(2013, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010703_2"),
                };
            byte[] buffer = DownloadInfoParam.EncodeArray(p1);
            IDownloadInfo[] p2 = DownloadInfoParam.DecodeArray(buffer);
            Assert.AreEqual(p1.Length, p2.Length);
            for (int i = 0; i < p1.Length; i++)
                AssertAreEqual(p1[i], p2[i]);
        }

        public static void AssertAreEqual(IDownloadInfo p1, IDownloadInfo p2)
        {
            DownloadInfoParam.AreEqual(p1, p2);
            Assert.AreEqual(p1.SourceIp, p2.SourceIp);
            Assert.AreEqual(p1.SourcePort, p2.SourcePort);
            Assert.AreEqual(p1.DownloadPath, p2.DownloadPath);
            Assert.AreEqual(p1.BeginTime, p2.BeginTime);
            Assert.AreEqual(p1.EndTime, p2.EndTime);
            Assert.AreEqual(p1.VideoId, p2.VideoId);
            Assert.AreEqual(p1.StreamId, p2.StreamId);
        }
    }
}
