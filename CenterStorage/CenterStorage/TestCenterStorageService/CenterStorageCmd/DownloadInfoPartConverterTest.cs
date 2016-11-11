using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class DownloadInfoPartConverterTest
    {
        IDownloadInfoExpand down = DownloadInfoExpandPacketTest.NewDownloadInfoExpandPacket();
        [TestMethod]
        public void TestDownloadInfoPartConverter_DownloadInfo()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.DownloadInfo));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.DownloadInfo, part.Code);
            DownloadInfoParamTest.AssertAreEqual(down.DownloadInfo, (IDownloadInfo)part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_Name()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.Name));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.Name, part.Code);
            Assert.AreEqual(down.Name, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_Quality()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.Quality));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.Quality, part.Code);
            Assert.AreEqual(down.Quality, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_Size()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.Size));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.Size, part.Code);
            Assert.AreEqual(down.Size, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_IsLocalDownload()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.IsLocalDownload));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.IsLocalDownload, part.Code);
            Assert.AreEqual(down.IsLocalDownload, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_DownloadStatus()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.DownloadStatus));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.Status, part.Code);
            Assert.AreEqual(down.DownloadStatus, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_TimePeriodsAll()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.TimePeriodsAll));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.TimePeriodsAll, part.Code);
            TimePeriodPacketTest.AssertAreEqualArray(down.TimePeriodsAll, (TimePeriodPacket[])part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_TimePeriodsCompleted()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.TimePeriodsCompleted));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.TimePeriodsCompleted, part.Code);
            TimePeriodPacketTest.AssertAreEqualArray(down.TimePeriodsCompleted, (TimePeriodPacket[])part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_ErrorInfo()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.ErrorInfo));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.ErrorInfo, part.Code);
            Assert.AreEqual(down.ErrorInfo, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_UpdatedLastestTime()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.UpdatedLastestTime));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.UpdatedLastestTime, part.Code);
            Assert.AreEqual(down.UpdatedLastestTime, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_Speed()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(down.Speed));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.Speed, part.Code);
            Assert.AreEqual(down.Speed, part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_GoTop()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, nameof(DownloadCode.GoTop));
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(down.GuidCode, part.GuidCode);
            Assert.AreEqual(DownloadCode.GoTop, part.Code);
            Assert.IsNull(part.Value);
        }

        [TestMethod]
        public void TestDownloadInfoPartConverter_Invalid()
        {
            byte[] buffer = DownloadInfoPartConverter.Encode(down, "invalidtest");
            DownloadExpandPart part = DownloadInfoPartConverter.Decode(buffer);
            Assert.AreEqual(0, (int)part.Code);
        }
    }
}
