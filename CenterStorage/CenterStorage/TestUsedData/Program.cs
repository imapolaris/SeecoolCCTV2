using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUsedData
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            BaseInfo.AddSeconds();
            BaseInfo.AddSeconds2();
            BaseInfo.AddTextMore();//1分钟左右写完
            BaseInfo.AddLargeVideos();
            BaseInfo.AddErrorDate();
            BaseInfo.AddLargeVideoReal();
            Console.WriteLine("UsedTime: {0}", DateTime.Now - start);
            Console.ReadKey();
        }

        //private static void testMoveTo()
        //{
        //    string path = @"D:\读写测试\TestDownloadRecorder";
        //    var di = new System.IO.DirectoryInfo(path);
        //    di.MoveTo(@"D:\读写测试\DownloadRecorder");
        //}
        //private void moretest()
        //{
        //    var dir = System.IO.Directory.GetDirectories(@"D:\视频录像\VideoId_50BAD1590001030C_2\2000\03\23");
        //    Assert.AreEqual(0, dir.Length);
        //}
        //public string SizeStr;
        //public int SizeInt;
        //[TestMethod]
        //public void test()
        //{
        //    this.GetType().GetField("SizeStr").SetValue(this, "123");
        //    this.GetType().GetField("SizeInt").SetValue(this, 123);
        //    Console.WriteLine("SizeStr: " + SizeStr);
        //    Console.WriteLine("SizeInt: " + SizeInt);
        //}
    }
}
