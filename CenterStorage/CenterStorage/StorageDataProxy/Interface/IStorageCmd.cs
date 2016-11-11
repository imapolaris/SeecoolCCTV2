using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageDataProxy
{
    public interface IStorageCmd
    {
        void GetVideoStorageInfo(VideoBaseInfomParam packet);

        void TurnOnOffVideoStorage(StorageFlagParam packet);

        void TurnOnOffDiskStorage(StorageFlagParam packet);
    }
}
