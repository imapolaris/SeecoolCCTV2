using CenterStorageCmd;
using Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public static class VideoStoragerManager
    {
        /// <summary>获取某通道视频在某时间点对应的视频数据包</summary>
        public static VideoStreamsPacket GetVideoPacket(string videoId, int streamId, DateTime time)
        {
            string path = getPath(videoId, streamId, time);
            return FolderManager.GetVideoStreamsPacket(path, time);
        }

        /// <summary>获取某通道视频在某时间点对应文件的视频头</summary>
        public static StreamPacket GetVideoHeader(string videoId, int streamId, DateTime time)
        {
            string indexesFile = null;
            string path = getPath(videoId, streamId, time);
            IndexesPacket packet = FolderManager.GetIndexesPacket(path, time, ref indexesFile);
            if (packet != null)
                return FileManager.GetVideoHeader(GlobalProcess.GetRecFileName(indexesFile));
            return null;
        }

        /// <summary>获取某通道视频在某时段对应文件的首个视频头</summary>
        public static VideoBasePacket GetVideoBaseInfom(string videoId, int streamId, DateTime beginTime, DateTime endTime)
        {
            string path = GlobalData.VideoPath(videoId, streamId);
            DateTime[] dates = GetFolderPaths(videoId, streamId, beginTime, endTime);
            foreach (var date in dates)
            {
                var header = FolderManager.GetVideoHeader(Path.Combine(path, GlobalProcess.FolderPath(date)), beginTime, endTime);
                if (header != null && header.Type == DataType.SysHead)
                {
                    long length = getLength(path, dates, beginTime, endTime, StreamPacket.Encode(header).Length + 4);
                    return new VideoBasePacket(header.Buffer, header.Time, length);
                }
            }
            return null;
        }

        /// <summary>获取某通道视频在某时段对应的有效的文件夹列表</summary>
        public static DateTime[] GetFolderPaths(string videoId, int streamId, DateTime start, DateTime end)
        {
            List<DateTime> list = new List<DateTime>();
            string path = GlobalData.VideoPath(videoId, streamId);
            if (Directory.Exists(path))
            {
                DateTime cur = start.Date;
                while (cur < end)
                {
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(path, GlobalProcess.FolderPath(cur)));
                    if (dir.Exists)
                    {
                        list.Add(cur);
                        cur = cur.AddDays(1);
                    }
                    else if (dir.Parent.Exists)
                        cur = cur.AddDays(1);
                    else if (dir.Parent.Parent.Exists)
                        cur = new DateTime(cur.Year, cur.Month, 1).AddMonths(1);
                    else
                        cur = new DateTime(cur.Year + 1, 1, 1);
                }
            }
            return list.ToArray();
        }
        
        ///<summary>删除最早的历史录像数据</summary>
        public static void DeleteEarliestVideo()
        {
            HistoryFolderArrayInfo earliestFolders = SearchEarliestSubfolders();
            if (earliestFolders != null)
            {
                Logger.Default.Trace($"删除 {earliestFolders.Time.ToShortDateString()} 视频！");
                Parallel.ForEach(earliestFolders.Paths, folder =>
                {
                    FolderManager.ClearDirectoryInfoAll(folder);
                });
            }
        }

        /// <summary>
        /// 获取指定目录所有视频节点中的时间最早的子目录的名称（包括其路径）。
        /// </summary>
        /// <param name="folder">目录文件夹</param>
        /// <returns>目录文件夹下最早的视频子文件夹</returns>
        public static HistoryFolderArrayInfo SearchEarliestSubfolders()
        {
            if (Directory.Exists(GlobalData.Path))
            {
                string[] subfolders = Directory.GetDirectories(GlobalData.Path);//获取所有的视频流文件夹路径
                List<string> hfList = new List<string>();
                DateTime earliestTime = DateTime.MaxValue;
                object lockobj = new object();
                Parallel.ForEach(subfolders, subfolder =>
                {
                    var info = SearchEarliestSubfolder(subfolder);
                    if (info != null)
                    {
                        lock (lockobj)
                        {
                            if (earliestTime >= info.Time)
                            {
                                if (earliestTime != info.Time)
                                {
                                    earliestTime = info.Time;
                                    hfList = new List<string>();
                                }
                                hfList.Add(info.Path);
                            }
                        }
                    }
                });
                if (hfList.Count > 0)
                {
                    return new HistoryFolderArrayInfo(hfList.ToArray(), earliestTime);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定目录所有视频节点中的时间最早的子目录的名称（包括其路径）。
        /// </summary>
        /// <param name="folder">目录文件夹</param>
        /// <returns>目录文件夹下最早的视频子文件夹</returns>
        public static HistoryFolderInfo SearchEarliestSubfolder(string videoInfoPath)
        {
            try
            {
                string path = videoInfoPath;
                string pathYear = Directory.GetDirectories(path).Where(_ => GlobalProcess.IsYear(getFolderName(_))).Min();
                int nYear = int.Parse(getFolderName(pathYear));
                string pathMonth = Directory.GetDirectories(pathYear).Where(_ => GlobalProcess.IsMonth(getFolderName(_))).Min();
                int nMonth = int.Parse(getFolderName(pathMonth));
                string pathDay = Directory.GetDirectories(pathMonth).Where(_ => GlobalProcess.IsDay(getFolderName(_))).Min();
                int nDay = int.Parse(getFolderName(pathDay));
                return new HistoryFolderInfo(pathDay, new DateTime(nYear, nMonth, nDay));
            }
            catch { }
            return null;
        }
        
        /// <summary>读取某通道视频在指定时间内视频流的字节数</summary>
        public static long GetLength(string videoId, int streamId, DateTime beginTime, DateTime endTime, int headerLength)
        {
            string path = GlobalData.VideoPath(videoId, streamId);
            DateTime[] dates = GetFolderPaths(videoId, streamId, beginTime, endTime);
            return getLength(path, dates, beginTime, endTime, headerLength);
        }

        static long getLength(string path, DateTime[] dates, DateTime beginTime, DateTime endTime, int headerLength)
        {
            long size = 0;
            for (int i = 0; i < dates.Length; i++)
            {
                var date = dates[i];
                string folder = Path.Combine(path, GlobalProcess.FolderPath(date));
                if (date > beginTime.Date && date < endTime.Date)
                    size += getAllSize(folder, headerLength);
                else
                    size += getValidSize(folder, beginTime, endTime, headerLength);
            }
            return size;
        }

        static long getAllSize(string folderPath, int headerLength)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles("*" + GlobalProcess.RecFormat());
            long length = files.Sum(_ => getStreamSize(_, headerLength));
            return length;
        }

        static long getValidSize(string folderPath, DateTime beginTime, DateTime endTime, int headerLength)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles("*" + GlobalProcess.RecFormat());
            long length = 0;
            foreach (var file in files)
            {
                length += getSize(file, beginTime, endTime, headerLength);
            }
            return length;
        }

        static long getSize(FileInfo fileInfo, DateTime beginTime, DateTime endTime, int headerLength)
        {
            long size = 0;
            try
            {
                string indexes = GlobalProcess.GetIndexesFileName(fileInfo.FullName);
                var indexesPackets = FileManager.GetIndexesPackets(indexes);
                IndexesPacket first = indexesPackets.FirstOrDefault(_ => _.EndTime >= beginTime);
                if (first != null)
                {
                    long beginIndex = first.StartIndex;
                    long endIndex = fileInfo.Length;
                    IndexesPacket end = indexesPackets.FirstOrDefault(_ => _.BeginTime >= endTime);
                    if (end != null)
                        endIndex = end.StartIndex;
                    size = endIndex - beginIndex;
                }
            }
            catch { }
            return size;
        }

        static long getStreamSize(FileInfo fileInfo, int headerSize)
        {
            return Math.Max(0, fileInfo.Length - headerSize);
        }

        private static string getFolderName(string path)
        {
            return new FileInfo(path).Name;
        }

        private static string getPath(string videoId, int streamId, DateTime time)
        {
            return Path.Combine(GlobalData.VideoPath(videoId, streamId), GlobalProcess.FolderPath(time));
        }
    }
}