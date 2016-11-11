using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.IO;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class DownloadInfoExpandPacketTest
    {
        [TestMethod]
        public void TestDownloadInfoExpandPacket()
        {
            var packet = NewDownloadInfoExpandPacket();
            byte[] buffer = DownloadInfoExpandPacket.Encode(packet);
            var packet2 = DownloadInfoExpandPacket.Decode(buffer);
            assertAreEqual(packet, packet2);
        }

        [TestMethod]
        public void TestDownloadInfoExpandPacketArrayEmpty()
        {
            DownloadInfoExpandPacket[] packets = new DownloadInfoExpandPacket[0];
            byte[] buffer = DownloadInfoExpandPacket.EncodeArray(packets);
            var packets2 = DownloadInfoExpandPacket.DecodeArray(buffer);
            Assert.AreEqual(0, packets2.Length);
        }

        [TestMethod]
        public void TestDownloadInfoExpandPacketArray()
        {
            var packet = NewDownloadInfoExpandPacket();

            DownloadInfoExpandPacket[] packets = new DownloadInfoExpandPacket[]
                {
                    packet,packet
                };
            byte[] buffer = DownloadInfoExpandPacket.EncodeArray(packets);
            var packets2 = DownloadInfoExpandPacket.DecodeArray(buffer);
            Assert.AreEqual(packets.Length, packets2.Length);
            for (int i = 0; i < packets.Length; i++)
                assertAreEqual(packets[i], packets2[i]);
        }

        public static DownloadInfoExpandPacket NewDownloadInfoExpandPacket()
        {
            Guid guid = Guid.NewGuid();
            DownloadInfoParam info = new DownloadInfoParam(Settings.DownloadHost, Settings.DownloadPort, new VideoBaseInfomParam(new VideoInfo("VideoId_Large_50BAD15900010701", 2), new TimePeriodPacket(new DateTime(2010, 1, 1), new DateTime(2016, 12, 31))), @"D:\视酷下载\Time_201604051240_201604051643\VideoId_Large_50BAD15900010701_2");
            string name = "";
            string quality = "标清";
            long size = 10000000;
            bool isLocalDownload = true;
            TimePeriodPacket[] timePeriods = new TimePeriodPacket[] {
                new TimePeriodPacket(new DateTime(2010,1,31), new DateTime(2010,12,31)),
                new TimePeriodPacket(new DateTime(2011,1,31), new DateTime(2011,12,31)),
                new TimePeriodPacket(new DateTime(2012,1,31), new DateTime(2012,12,31)),
                new TimePeriodPacket(new DateTime(2013,1,31), new DateTime(2016,12,31)),
            };
            TimePeriodPacket[] timePeriodsCompleted = new TimePeriodPacket[] {
                new TimePeriodPacket(new DateTime(2010,1,31), new DateTime(2010,6,3)),
                new TimePeriodPacket(new DateTime(2011,1,31), new DateTime(2011,5,3)),
                new TimePeriodPacket(new DateTime(2012,1,31), new DateTime(2012,12,1)),
                new TimePeriodPacket(new DateTime(2013,1,31), new DateTime(2016,12,1)),
            };
            DownloadStatus status = DownloadStatus.Downloading;
            string errorInfo = null;
            DateTime updatedLastestTime = new DateTime(2016, 5, 2);
            return new DownloadInfoExpandPacket(guid, info, name, quality, size, isLocalDownload, timePeriods, timePeriodsCompleted, status, errorInfo, updatedLastestTime, 0);
        }

        private void assertAreEqual(DownloadInfoExpandPacket p1, DownloadInfoExpandPacket p2)
        {
            Assert.AreEqual(p1.GuidCode, p2.GuidCode);
            Assert.AreEqual(p1.Name, p2.Name);
            Assert.AreEqual(p1.Quality, p2.Quality);
            Assert.AreEqual(p1.Size, p2.Size);
            Assert.AreEqual(p1.IsLocalDownload, p2.IsLocalDownload);
            Assert.AreEqual(p1.DownloadStatus, p2.DownloadStatus);
            TimePeriodPacketTest.AssertAreEqualArray(p1.TimePeriodsAll, p2.TimePeriodsAll);
            TimePeriodPacketTest.AssertAreEqualArray(p1.TimePeriodsCompleted, p2.TimePeriodsCompleted);
            Assert.AreEqual(p1.ErrorInfo, p2.ErrorInfo);
            Assert.AreEqual(p1.UpdatedLastestTime, p2.UpdatedLastestTime);
            Assert.AreEqual(p1.Speed, p2.Speed);

        }
    }
}
