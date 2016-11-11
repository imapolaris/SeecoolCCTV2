using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public class StorageDownloader : IDownloader
    {
        public static IDownloader Instance { get; private set; }
        static StorageDownloader()
        {
            Instance = new StorageDownloader();
        }

        public VideoBasePacket GetVideoBaseInfom(string videoId, int streamId, DateTime start, DateTime end)
        {
            return VideoStoragerManager.GetVideoBaseInfom(videoId, streamId, start, end);
        }

        public VideoStreamsPacket GetVideoPacket(string videoId, int streamId, DateTime time)
        {
            return VideoStoragerManager.GetVideoPacket(videoId, streamId, time);
        }
    }
}
