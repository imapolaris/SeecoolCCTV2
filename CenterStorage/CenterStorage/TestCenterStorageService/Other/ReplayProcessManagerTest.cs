using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCenterStorageService.Other
{
    [TestClass]
    public class ReplayProcessManagerTest
    {
        DateTime startTime = new DateTime(2016, 5, 16, 9, 0, 0);
        [TestMethod]
        public void TestReplayProcessManager()
        {
            ReplayProcessManager _replay = new ReplayProcessManager(startTime);
            Assert.AreEqual(startTime, _replay.GetPlayingTime());
            System.Threading.Thread.Sleep(100);
            Assert.AreEqual(startTime, _replay.GetPlayingTime());

            //正常播放10秒
            DateTime time = startTime;
            DateTime playStartTime = DateTime.Now;
            _replay.Playing = true;
            for(int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(100);
                time = startTime.AddTicks((DateTime.Now - playStartTime).Ticks);
                DateTime replayTime = _replay.GetPlayingTime();
                Console.WriteLine("{0} - {1}", i, time.TimeOfDay);
                Assert.IsTrue((replayTime - time) < TimeSpan.FromMilliseconds(10));
            }
            //暂停等待1秒
            _replay.Playing = false;
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue((_replay.GetPlayingTime() - time) < TimeSpan.FromMilliseconds(10));

            //8倍速播放10秒
            _replay.JumpTo(startTime);
            playStartTime = DateTime.Now;
            _replay.Playing = true;
            _replay.PlaySpeed = 8;
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(100);
                time = startTime.AddTicks((long)((DateTime.Now - playStartTime).Ticks * _replay.PlaySpeed));
                DateTime replayTime = _replay.GetPlayingTime();
                Console.WriteLine("{0} - {1}", i, time.TimeOfDay);
                Assert.IsTrue((replayTime - time) < TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
