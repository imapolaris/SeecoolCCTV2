using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StorageDataProxy
{
    public interface IStreamDownload
    {
        void GetVideoData(VideoDataParam param);

        void GetVideoBaseInfo(IVideoBaseInfom param);
    }
}
