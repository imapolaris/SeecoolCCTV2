using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;
using System.Collections.Generic;
using System.Linq;

namespace TestCenterStorageService.CenterStorageCmd
{
    [TestClass]
    public class TimePeriodManagerTest
    {
        DateTime beginTime = new DateTime(2016, 3, 24, 11, 20, 30, 123);
        List<TimePeriodPacket> tis = new List<TimePeriodPacket>();
        [TestInitialize]
        public void Setup()
        {
            tis.Add(new TimePeriodPacket(beginTime, beginTime.AddMinutes(15)));
            for (int i = 0; i < 40; i++)
                tis.Add(new TimePeriodPacket(tis.Last().EndTime, tis.Last().EndTime.AddMinutes(15)));
        }

        [TestMethod]
        public void TestTimePeriodManager_Combine_Empty()
        {
            Assert.AreEqual(0, TimePeriodManager.Combine(null).Length);
            Assert.AreEqual(0, TimePeriodManager.Combine(new TimePeriodPacket[0]).Length);
        }

        [TestMethod]
        public void TestTimePeriodManager_Combine_Continuous()//测试连续情况
        {
            var combine = TimePeriodManager.Combine(tis.ToArray());
            Assert.AreEqual(1, combine.Length);
            assertEqual(new TimePeriodPacket(tis.First().BeginTime, tis.Last().EndTime), combine[0]);
        }

        [TestMethod]
        public void TestTimePeriodManager_Combine_Segmentation()//测试被分段情况
        {
            tis.RemoveAt(10);
            tis.RemoveAt(20);
            var combine = TimePeriodManager.Combine(tis.ToArray());
            Assert.AreEqual(3, combine.Length);
            assertEqual(new TimePeriodPacket(beginTime, tis[9].EndTime), combine[0]);
            assertEqual(new TimePeriodPacket(tis[10].BeginTime, tis[19].EndTime), combine[1]);
            assertEqual(new TimePeriodPacket(tis[20].BeginTime, tis.Last().EndTime), combine[2]);
        }

        [TestMethod]
        public void TestTimePeriodManager_GetValidArray()
        {
            int validLength = tis.Count;
            Assert.AreEqual(validLength, TimePeriodManager.GetValidArray(tis.ToArray()).Length);
            tis.Add(null);
            Assert.AreEqual(validLength, TimePeriodManager.GetValidArray(tis.ToArray()).Length);
            tis.Add(new TimePeriodPacket(beginTime.AddDays(1), beginTime));
            Assert.AreEqual(validLength, TimePeriodManager.GetValidArray(tis.ToArray()).Length);
            tis.Add(new TimePeriodPacket(beginTime, beginTime.AddDays(1)));
            Assert.AreEqual(validLength + 1, TimePeriodManager.GetValidArray(tis.ToArray()).Length);
        }

        [TestMethod]
        public void TestTimePeriodManager_Intersection()
        {
            TimePeriodPacket ti1 = new TimePeriodPacket(beginTime, beginTime.AddDays(1));
            TimePeriodPacket ti2 = new TimePeriodPacket(beginTime.AddHours(1), beginTime.AddHours(2));
            TimePeriodPacket ti3 = new TimePeriodPacket(beginTime.AddHours(1), beginTime.AddDays(2));
            TimePeriodPacket ti1_3 = new TimePeriodPacket(beginTime.AddHours(1), beginTime.AddDays(1));
            TimePeriodPacket invalid = new TimePeriodPacket(beginTime.AddDays(1), beginTime);

            assertEqual(ti2, TimePeriodManager.Intersection(ti1, ti2));
            assertEqual(ti2, TimePeriodManager.Intersection(ti2, ti1));
            assertEqual(ti1_3, TimePeriodManager.Intersection(ti3, ti1));
            assertEqual(ti1_3, TimePeriodManager.Intersection(ti1, ti3));
            Assert.IsNull(TimePeriodManager.Intersection(ti1, invalid));
            Assert.IsNull(TimePeriodManager.Intersection(invalid, ti1));
        }

        [TestMethod]
        public void TestTimePeriodManager_GetIntersections()
        {
            TimePeriodPacket ti1 = new TimePeriodPacket(beginTime.AddMinutes(5), beginTime.AddMinutes(40));
            TimePeriodPacket[] results = TimePeriodManager.GetIntersections(tis.ToArray(), ti1);
            Assert.AreEqual(3, results.Length);
            TimePeriodPacket[] results2 = TimePeriodManager.GetIntersections(new TimePeriodPacket[0], ti1);
            Assert.AreEqual(0, results2.Length);
        }

        [TestMethod]
        public void TestTimePeriodManager_GetIntersectionsArray()
        {
            List<TimePeriodPacket> tis = new List<TimePeriodPacket>();
            tis.Add(new TimePeriodPacket(beginTime, beginTime.AddMinutes(15)));
            for (int i = 0; i < 40; i++)
                tis.Add(new TimePeriodPacket(tis.Last().EndTime.AddSeconds(1), tis.Last().EndTime.AddMinutes(15)));

            TimePeriodPacket[] results = TimePeriodManager.GetIntersections(tis.ToArray(), new TimePeriodPacket[0]);
            Assert.AreEqual(0, results.Length);
            results = TimePeriodManager.GetIntersections(tis.ToArray(), tis.ToArray());
            Assert.AreEqual(tis.Count, results.Length);
        }

        [TestMethod]
        public void TestTimePeriodManager_IsValid()
        {
            TimePeriodPacket ti = new TimePeriodPacket(new DateTime(2016, 1, 1), new DateTime(2016, 3, 1));
            Assert.IsFalse(TimePeriodManager.IsValid(ti, new DateTime(2015, 1, 1)));
            Assert.IsTrue(TimePeriodManager.IsValid(ti, new DateTime(2016, 1, 1)));
            Assert.IsTrue(TimePeriodManager.IsValid(ti, new DateTime(2016, 2, 1)));
            Assert.IsFalse(TimePeriodManager.IsValid(ti, new DateTime(2016, 3, 1)));
            Assert.IsFalse(TimePeriodManager.IsValid(ti, new DateTime(2016, 5, 1)));
        }

        [TestMethod]
        public void TestTimePeriodManager_Subtract()
        {
            assertSubtractEmpty(getTi(0,1), getTi(0, 2));
            assertSubtractEmpty(getTi(1, 2), getTi(0, 2));
            assertSubtractEmpty(getTi(1, 2), getTi(0, 3));

            assertSubtractAll(getTi(0, 1), getTi(1, 2));
            assertSubtractAll(getTi(0, 1), getTi(2, 3));
            assertSubtractAll(getTi(1, 2), getTi(0, 1));
            assertSubtractAll(getTi(2, 3), getTi(0, 1));

            assertSubtractAny(getTi(0, 2), getTi(1, 2), getTi(0, 1));
            assertSubtractAny(getTi(1, 5), getTi(0, 2), getTi(2, 5));
            assertSubtractAny(getTi(1, 5), getTi(2, 3), getTi(1, 2), getTi(3, 5));
        }

        [TestMethod]
        public void TestTimePeriodManager_Subtracts()
        {
            TimePeriodPacket[] tis = new TimePeriodPacket[] { getTi(1, 2), getTi(4, 7), getTi(10, 14) };
            assertSubtractsEmpty(null, getTi(0, 2));
            assertSubtractsEmpty(new TimePeriodPacket[0], getTi(0, 2));
            assertSubtractsEmpty(tis, getTi(0, 14));
            assertSubtractsEmpty(tis, getTi(1, 15));

            assertSubtractsAll(tis, getTi(0, 1));
            assertSubtractsAll(tis, getTi(2, 3));
            assertSubtractsAll(tis, getTi(3, 4));
            assertSubtractsAll(tis, getTi(14, 19));

            assertSubtractsAny(tis, getTi(1, 2), getTi(4, 7), getTi(10, 14));
            assertSubtractsAny(tis, getTi(0, 5), getTi(5, 7), getTi(10, 14));
            assertSubtractsAny(tis, getTi(11, 13), getTi(1, 2), getTi(4, 7), getTi(10, 11), getTi(13, 14));
        }

        static TimePeriodPacket getTi(int start, int end)
        {
            return new TimePeriodPacket(new DateTime(2000 + start, 1, 1), new DateTime(2000 + end, 1, 1));
        }

        static void assertSubtractAll(TimePeriodPacket ti1, TimePeriodPacket ti2)
        {
            assertSubtractAny(ti1, ti2, ti1);
        }

        static void assertSubtractEmpty(TimePeriodPacket ti1, TimePeriodPacket ti2)
        {
            assertSubtractAny(ti1, ti2);
        }

        static void assertSubtractsAll(TimePeriodPacket[] tis1, TimePeriodPacket ti2)
        {
            assertSubtractsAny(tis1, ti2, tis1);
        }

        static void assertSubtractsEmpty(TimePeriodPacket[] tis, TimePeriodPacket ti)
        {
            assertSubtractsAny(tis, ti);
        }

        static void assertSubtractAny(TimePeriodPacket ti1, TimePeriodPacket ti2, params TimePeriodPacket[] tis)
        {
            TimePeriodPacket[] tiSub = TimePeriodManager.Subtract(ti1, ti2);
            Assert.AreEqual(tis.Length, tiSub.Length);
            for (int i = 0; i < tis.Length; i++)
            {
                assertEqual(tis[i], tiSub[i]);
            }
        }

        static void assertSubtractsAny(TimePeriodPacket[] tis1, TimePeriodPacket ti2, params TimePeriodPacket[] tis)
        {
            TimePeriodPacket[] tiSub = TimePeriodManager.Subtracts(tis1, ti2);
            Assert.AreEqual(tis.Length, tiSub.Length);
            for (int i = 0; i < tis.Length; i++)
            {
                assertEqual(tis[i], tiSub[i]);
            }
        }

        private static void assertEqual(TimePeriodPacket expected, TimePeriodPacket actual)
        {
            Assert.AreEqual(expected.BeginTime, actual.BeginTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
        }
    }
}
