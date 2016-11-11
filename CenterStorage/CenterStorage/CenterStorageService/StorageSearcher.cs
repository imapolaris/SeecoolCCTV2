using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public class StorageSearcher:ISearcher
    {
        public static StorageSearcher Instance { get; private set; }
        static StorageSearcher()
        {
            Instance = new StorageSearcher();
        }

        public VideoTimePeriodsPacket Search(DateTime beginTime, DateTime endTime, IVideoInfo videoInfo)
        {
            string path = GlobalData.VideoPath(videoInfo.VideoId, videoInfo.StreamId);
            DateTime[] dates = VideoStoragerManager.GetFolderPaths(videoInfo.VideoId, videoInfo.StreamId, beginTime, endTime);
            List<TimePeriodPacket> tis = new List<TimePeriodPacket>();
            Parallel.ForEach(dates, date =>
            {
                string folder = Path.Combine(path, GlobalProcess.FolderPath(date));
                var array = FolderManager.GetTimePeriods(folder);
                if (array.Length > 0)
                {
                    lock (tis)
                        tis.AddRange(array);
                }
            });
            if(endTime > DateTime.Now.Date)
                tis.AddRange(findTodayLastest(path, endTime));
            var timeCombines = TimePeriodManager.Combine(tis.ToArray());
            var validArray = TimePeriodManager.GetIntersections(timeCombines, new TimePeriodPacket(beginTime, endTime));
            return new VideoTimePeriodsPacket(videoInfo, validArray);
        }

        public VideoTimePeriodsPacket[] Search(DateTime beginTime, DateTime endTime, IVideoInfo[] videoInfos)
        {
            int length = videoInfos.Length;
            VideoTimePeriodsPacket[] array = new VideoTimePeriodsPacket[length];
            if (length > 0)
            {
                Parallel.For(0, length, index =>
                {
                    array[index] = Search(beginTime, endTime, videoInfos[index]);
                });
            }
            return array;
        }

        TimePeriodPacket[] findTodayLastest(string path, DateTime end)
        {
            string folder = Path.Combine(path, GlobalProcess.FolderPath(DateTime.Now));
            var array = FolderManager.GetIndexesFiles(folder);
            if (array.Length > 0)
            {
                string file = array.Max();
                var packets = FileManager.GetIndexesPackets(file);
                return TimePeriodManager.Combine(packets);
            }
            return new TimePeriodPacket[0];
        }
    }
}
