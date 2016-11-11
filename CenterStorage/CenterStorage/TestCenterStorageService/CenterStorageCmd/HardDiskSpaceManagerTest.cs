using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class HardDiskSpaceManagerTest
    {
        [TestMethod]
        public void TestHardDiskSpaceManager_GetHardDiskSpace()
        {
            long lengthDisk = HardDiskSpaceManager.GetHardDiskSpace(@"D:\");
            Console.WriteLine($"D盘总空间: {lengthDisk}");
            long lengthFree = HardDiskSpaceManager.GetHardDiskFreeSpace(@"D:\");
            Console.WriteLine($"D盘剩余空间: {lengthFree}");
        }
    }
}
