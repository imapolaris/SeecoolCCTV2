using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.IO;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class ConfigFileTest
    {
        DateTime[] times = new DateTime[]
                  {
                    new DateTime(2010,1,1,2,3,4,5),
                    new DateTime(2011,1,1),
                    new DateTime(2012,1,1),
                    new DateTime(2013,1,1),
                  };
        [TestMethod]
        public void TestConfigFile()
        {
            string fileName = @"D:\读写测试\test.test";
            Assert.IsTrue(ConfigFile<DateTime[]>.SaveToFile(fileName, times));
            var newtimes = ConfigFile<DateTime[]>.FromFile(fileName);
            Assert.AreEqual(times.Length, newtimes.Length);
            for (int i = 0; i < times.Length; i++)
                Assert.AreEqual(times[i], newtimes[i]);
            new FileInfo(fileName).Delete();
            Assert.IsNull(ConfigFile<DateTime[]>.FromFile(fileName));
            new FileInfo(fileName).Delete();
        }

        [TestMethod]
        public void TestConfigFile_Error()
        {
            string fileName = @"D:\读写测试\test1.test";
            Assert.IsTrue(ConfigFile<DateTime[]>.SaveToFile(fileName, times));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                Assert.IsNull(ConfigFile<DateTime[]>.FromFile(fileName));
                Assert.IsFalse(ConfigFile<DateTime[]>.SaveToFile(fileName, times));
            }
            new FileInfo(fileName).Delete();
        }
    }
}
