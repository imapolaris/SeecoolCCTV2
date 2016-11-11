using CenterStorageCmd;
using Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public static class FolderManager
    {
        /// <summary>
        /// 获取某文件夹下某段时间内首个视频头文件
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>视频头</returns>
        public static StreamPacket GetVideoHeader(string folder, DateTime beginTime, DateTime endTime)
        {
            StreamPacket header = null;
            string[] files = GetIndexesFiles(folder);
            if (files.Length > 0)
            {
                TimePeriodPacket validTi = new TimePeriodPacket(beginTime, endTime);
                TimePeriodPacket[] tis = FolderManager.GetTimePeriods(folder);
                if(TimePeriodManager.GetIntersections(tis, validTi).Length != 0)
                {
                    foreach (var file in files)
                    {
                        header = FileManager.GetVideoHeader(GlobalProcess.GetRecFileName(file));
                        if (header != null)
                            break;
                    }
                }
            }
            return header;
        }

        public static DateTime GetLastestTime(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                if (files.Length > 0)
                    return files.Max(_ => _.LastWriteTime);
            }
            return DateTime.MinValue;
        }

        /// <summary>获取某文件夹目录下所有索引文件名称列表</summary>
        public static string[] GetIndexesFiles(string path)
        {
            List<string> array = new List<string>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*.Indexes");
                return files.Select(_ => _.FullName).ToArray();
            }
            catch { }
            return array.ToArray();
        }

        /// <summary>
        /// 获取路径下指定时间点对应的视频流信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static VideoStreamsPacket GetVideoStreamsPacket(string path, DateTime time)
        {
            string indexesFileName = null;
            IndexesPacket indexesPacket = GetIndexesPacket(path, time, ref indexesFileName);
            if (indexesPacket != null && indexesFileName != null)
            {
                string videoFile = GlobalProcess.GetRecFileName(indexesFileName);
                StreamPacket[] streamPacket = FileManager.GetStreamPacket(videoFile, indexesPacket.StartIndex);
                return new VideoStreamsPacket(indexesPacket, streamPacket);
            }
            Logger.Default.Trace("{0} 未找到 {1} 对应的索引文件! {2}", path, time, indexesFileName);
            return null;
        }

        public static IndexesPacket GetIndexesPacket(string path, DateTime time, ref string fileName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                FileInfo[] files = dirInfo.GetFiles("*" + GlobalProcess.IndexesFormat());
                string startStr = $"{GlobalProcess.FileNameFromTime(time)}{GlobalProcess.IndexesFormat()}";
                var filesOrder = files.OrderBy(_ => _.Name).ToList();
                FileInfo last = filesOrder.LastOrDefault(_ => string.Compare(_.Name, startStr) <= 0);
                if (last == null)
                    return null;
                fileName = last.FullName;
                return FileManager.GetIndexesPacket(fileName, time);
            }
            return null;
        }

        /// <summary>
        /// 获取文件夹内所有简单索引文件中时间段列表
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <returns>时间段列表</returns>
        public static TimePeriodPacket[] GetTimePeriods(string folder)
        {
            List<TimePeriodPacket> list = new List<TimePeriodPacket>();
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles("*" + GlobalProcess.SimpleIndexesFormat());
            foreach (var file in files)
            {
                var array = FileManager.GetTimePeriods(file.FullName);
                if (array != null)
                    list.AddRange(array);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获取文件夹内所有索引文件中时间索引列表
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <returns>时间索引列表</returns>
        public static IndexesPacket[] GetIndexesPackets(string folder)
        {
            List<IndexesPacket> list = new List<IndexesPacket>();
            string[] files = GetIndexesFiles(folder);
            foreach (var file in files)
            {
                var array = FileManager.GetIndexesPackets(file);
                if (array != null)
                    list.AddRange(array);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 删除文件夹及其子文件
        /// </summary>
        /// <param name="path">要删除的非空目录</param>
        /// <returns>true表示文件夹已删除或不存在，false表示删除失败</returns>
        public static bool DeleteDirectoryInfo(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    Console.WriteLine("Delete : " + di.FullName);
                    di.Delete(true);
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 删除文件夹及其子文件，同时父文件夹为空时删除父文件夹（逐层）
        /// </summary>
        /// <param name="path">要删除的非空目录</param>
        /// <returns>true表示文件夹已删除或不存在，false表示删除失败</returns>
        public static bool ClearDirectoryInfoAll(string path)
        {
            if (DeleteDirectoryInfo(path))
            {
                DirectoryInfo fiParent = Directory.GetParent(path);
                if (fiParent != null)
                {
                    if (Directory.GetFileSystemEntries(fiParent.FullName).Length == 0)
                        ClearDirectoryInfoAll(fiParent.FullName);
                }
                return true;
            }
            else
                return false;
        }

        public static LocalVideosInfoPacket GetLocalVideoInfoPacket(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            ITimePeriod tp = null;
            DateTime endTime = DateTime.MaxValue;
            if (dirInfo.Exists)
            {
                List<VideoTimePeriodsPacket> vis = new List<VideoTimePeriodsPacket>();
                foreach (var fold in dirInfo.GetDirectories())
                {
                    VideoBaseFileRecorder recorder = new VideoBaseFileRecorder(fold.FullName);
                    if (recorder.DownloadInfo != null && recorder.TimePeriods != null)
                    {
                        if(tp == null)
                            tp = recorder.DownloadInfo;
                        vis.Add(recorder.TimePeriods);
                    }
                }
                if(tp != null)
                {
                    return new LocalVideosInfoPacket(tp, vis.ToArray());
                }
            }
            throw new IOException("指定路径下未找到视频信息！");
        }
    }
}