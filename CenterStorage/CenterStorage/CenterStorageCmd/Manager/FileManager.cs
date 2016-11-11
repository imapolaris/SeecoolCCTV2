using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public static class FileManager
    {
        /// <summary>获取某文件中视频的简单索引</summary>
        /// <param name="fileName">简单索引所在路径</param>
        /// <returns>该路径下简单索引中记录的所有视频录像对应的时间段</returns>
        public static TimePeriodPacket[] GetTimePeriods(string fileName)
        {
            try
            {
                using (FileStream fs = readStream(fileName))
                {
                    return readTimePeriodArray(fs).ToArray();
                }
            }
            catch { }
            return null;
        }
        
        /// <summary>获取视频头文件</summary>
        /// <returns>头文件信息</returns>
        public static StreamPacket GetVideoHeader(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var header = readStreamPacket(fs);
                    if (header != null && header.Type == DataType.SysHead)
                        return header;
                }
            }
            return null;
        }

        /// <summary>获取视频资源片段</summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="startIndex">文件读取起始位置</param>
        /// <returns>视频流</returns>
        public static StreamPacket[] GetStreamPacket(string fileName, long startIndex)
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = readStream(fileName))
                {
                    fs.Position = startIndex;
                    return readStreamPackets(fs).ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某索引文件中指定时间点索引信息
        /// </summary>
        /// <param name="fileName">索引文件地址</param>
        /// <param name="time">查找时间</param>
        /// <returns>该索引文件中指定时间点的索引信息</returns>
        public static IndexesPacket GetIndexesPacket(string fileName, DateTime time)
        {
            try
            {
                return GetIndexesPackets(fileName).FirstOrDefault(_ => TimePeriodManager.IsValid(_,time));
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return null;
        }

        public static IndexesPacket[] GetIndexesPackets(string fileName)
        {
            using (FileStream fs = readStream(fileName))
            {
                return readIndexesDatas(fs).ToArray();
            }
        }

        private static FileStream readStream(string fileName)
        {
            if(new FileInfo(fileName).Exists)
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Console.WriteLine("{0} 文件不存在", fileName);
            return null;
        }

        private static List<StreamPacket> readStreamPackets(Stream fs)
        {
            List<StreamPacket> streamPackets = new List<StreamPacket>();
            while (fs.Position < fs.Length)
            {
                var data = readStreamPacket(fs);
                if (data.Type == DataType.StreamDataKey && streamPackets.Any(_ => _.Type == DataType.StreamDataKey))
                    break;
                streamPackets.Add(data);
            }
            return streamPackets;
        }

        private static StreamPacket readStreamPacket(Stream fs)
        {
            try
            {
                byte[] buffer = readStream(fs);
                return StreamPacket.Decode(buffer);
            }
            catch { }
            return null;
        }

        private static List<IndexesPacket> readIndexesDatas(Stream fs)
        {
            List<IndexesPacket> indexesDatas = new List<IndexesPacket>();
            while (fs != null && fs.Position < fs.Length)
            {
                byte[] buffer = readStream(fs);
                var data = IndexesPacket.Decode(buffer);
                indexesDatas.Add(data);
            }
            return indexesDatas;
        }

        private static List<TimePeriodPacket> readTimePeriodArray(Stream fs)
        {
            List<TimePeriodPacket> shortIndexesDatas = new List<TimePeriodPacket>();
            while (fs.Position < fs.Length)
            {
                byte[] buffer = readStream(fs);
                var data = TimePeriodPacket.Decode(buffer);
                shortIndexesDatas.Add(data);
            }
            return shortIndexesDatas;
        }

        private static byte[] readStream(Stream ms)
        {
            int length = readInt(ms);
            return PacketBase.ReadByteArray(ms, length - 4);
        }

        private static int readInt(Stream fs)
        {
            byte[] buffer = new byte[4];
            fs.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}