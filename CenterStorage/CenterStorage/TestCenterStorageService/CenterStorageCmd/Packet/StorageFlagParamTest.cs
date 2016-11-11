using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class StorageFlagParamTest
    {
        [TestMethod]
        public void TestStorageFlagParam()
        {
            StorageFlagParam param = new StorageFlagParam(new VideoInfo("objectId", 2), true);
            byte[] buffer = StorageFlagParam.Encode(param);
            StorageFlagParam param2 = StorageFlagParam.Decode(buffer);
            AssertAreEqual(param, param2);

            StorageFlagParam param3 = new StorageFlagParam(new VideoInfo("objectId", 2, "test"), false);
            byte[] buffer2 = StorageFlagParam.Encode(param3);
            StorageFlagParam param4 = StorageFlagParam.Decode(buffer2);
            AssertAreEqual(param3, param4);
        }

        static void AssertAreEqual(StorageFlagParam param, StorageFlagParam param2)
        {
            Assert.AreEqual(param.VideoId, param2.VideoId);
            Assert.AreEqual(param.StreamId, param2.StreamId);
            Assert.AreEqual(param.VideoName, param2.VideoName);
            Assert.AreEqual(param.StorageOn, param2.StorageOn);
        }
    }
}
