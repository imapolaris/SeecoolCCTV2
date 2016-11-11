using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Collections.Generic;
using System.Linq;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class TimeProbeManagerTest
    {
        [TestMethod]
        public void TestTimeProbeManager()
        {
            DateTime time = new DateTime(2016, 7, 25, 17, 15, 0);
            Assert.AreEqual(DateTime.MaxValue, TimeProbeManager.GetProbeTime(null, time));

            List<TimePeriodPacket> tpps = new List<TimePeriodPacket>();
            Assert.AreEqual(DateTime.MaxValue, TimeProbeManager.GetProbeTime(tpps.ToArray(), time));

            tpps.Add(new TimePeriodPacket(time.AddSeconds(10), time.AddSeconds(60)));
            Assert.AreEqual(time.AddSeconds(10), TimeProbeManager.GetProbeTime(tpps.ToArray(), time));

            tpps.Add(new TimePeriodPacket(time.Subtract(TimeSpan.FromSeconds(60)), time.Subtract(TimeSpan.FromSeconds(10))));
            tpps = tpps.OrderBy(_ => _.BeginTime).ToList();
            Assert.AreEqual(time.AddSeconds(10), TimeProbeManager.GetProbeTime(tpps.ToArray(), time));

            tpps.Add(new TimePeriodPacket(time.Subtract(TimeSpan.FromSeconds(5)), time.AddSeconds(5)));
            tpps = tpps.OrderBy(_ => _.BeginTime).ToList();
            Assert.AreEqual(time, TimeProbeManager.GetProbeTime(tpps.ToArray(), time));
        }
    }
}
