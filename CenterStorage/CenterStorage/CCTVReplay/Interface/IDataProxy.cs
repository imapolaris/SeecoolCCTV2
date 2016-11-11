using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Interface
{
    public interface IDataProxy
    {
        bool HasVideo(DateTime time);
        bool HasLoaded(DateTime time);
        bool HasLocalBuffered(DateTime time);
        void RefreshLocalBuffer(DateTime time);
        void SeekToPos(DateTime time);
        VideoStreamsPacket GetVideoPacket(DateTime time);
    }
}
