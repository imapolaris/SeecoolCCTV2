using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class OfflinePlayManager :IOfflinePlayback, IDownloadManager
    {
        VideoBaseFileRecorder _baseRec;
        string _path;
        LocalDownloadInfoPacket _loadInfo;
        public OfflinePlayManager(LocalDownloadInfoPacket packet)
        {
            _loadInfo = packet;
            _path = System.IO.Path.Combine(packet.Path, $"{packet.Info.VideoId}_{packet.Info.StreamId}");
            _baseRec = new VideoBaseFileRecorder(_path);
        }

        public VideoBasePacket GetVideoBasePacket()
        {
            return _baseRec.VideoBase;
        }

        public VideoTimePeriodsPacket GetVideoTimePeriods()
        {
            return _baseRec.TimePeriods;
        }

        public VideoTimePeriodsPacket GetCompletedTimePeriods()
        {
            IndexesPacket[] indexesPackets = FolderManager.GetIndexesPackets(_path);
            var downloadedTPPs = TimePeriodManager.GetIntersections(indexesPackets, GetVideoTimePeriods()?.TimePeriods);
            return new VideoTimePeriodsPacket(_loadInfo.Info, downloadedTPPs);
        }

        public VideoStreamsPacket GetVideoStreamsPacket(DateTime time)
        {
            return FolderManager.GetVideoStreamsPacket(_path, time);
        }
    }
}
