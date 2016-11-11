using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Source;
using CenterStorageCmd;
using CCTVModels;

namespace CCTVReplay.StaticInfo
{
    public interface IVideoInfoManager : IDisposable
    {
        CCTVHierarchyNode GetHierarchyRoot();

        CCTVStaticInfo GetStaticInfo(string videoId);

        CCTVDynamicInfo GetDynamicInfo(string videoId);

        CCTVOnlineStatus GetOnlineStatus(string videoId);

        VideoDataSource GetStorageSource();

        void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler);

        void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler);

        event Action<LocalVideosInfoPacket> LocalSourceInfoReceived;
    }
}
