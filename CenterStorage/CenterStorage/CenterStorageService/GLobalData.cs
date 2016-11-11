using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public static class GlobalData
    {
        /// <summary>
        /// 静态信息服务地址。
        /// </summary>
        public static string StaticInfoAddress;
        public static string Path = @"D:\视频录像\";
        public static long HardDiskFreeSpaceInf = (long)3 * 1024 * 1024 * 1024;//3G
        public static TimeSpan FileLengthSup = new TimeSpan(0, 15, 0);
        public static string VideoPath(string videoId, int streamId)
        {
            return System.IO.Path.Combine(Path, $"{videoId}_{streamId}");
        }
    }
}