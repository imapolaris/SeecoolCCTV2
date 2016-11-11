using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface IDownloader
    {
        VideoBasePacket GetVideoBaseInfom(string videoId, int streamId, DateTime start, DateTime end);
        VideoStreamsPacket GetVideoPacket(string videoId, int streamId, DateTime time);
    }
}
