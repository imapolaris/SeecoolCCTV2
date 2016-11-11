using AopUtil.WpfBinding;
using CCTVReplay.Util;
using CCTVReplay.Video;
using CCTVReplay.VideoTree.Model;
using CCTVReplay.VideoTree.Thumbnail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CCTVReplay.StaticInfo;

namespace CCTVReplay.VideoTree.Search
{
    public class SearchNodeHistoryViewModel : ObservableObject
    {
        [AutoNotify]
        public CctvNode Node { get; set; }
        [AutoNotify]
        public bool IsOnline { get; set; }

        internal SearchNodeHistoryViewModel(bool allowDrag)
        {
            VideoChildren = new CollectionViewSource();
            this.AllowDrag = allowDrag;
        }

        [AutoNotify]
        public CollectionViewSource VideoChildren { get; set; }
        [AutoNotify]
        public VideoThumbnailsViewModel SelectedVideo { get; set; }

        [AutoNotify]
        public CctvNode SelectedNode { get; set; }
        [AutoNotify]
        public bool IsUnfoldVideo { get; private set; }
        [AutoNotify]
        public bool AllowDrag { get; private set; }

        public void UpdateNode(CctvNode node, bool isUnfoldVideo = true)
        {
            Node = node;
            IsUnfoldVideo = isUnfoldVideo;
            updateOnlineStatus();
            updateVideo();
        }
        private void updateOnlineStatus()
        {
            if (Node != null && !string.IsNullOrEmpty(Node.ID))
            {
                var onlineStatus = VideoInfoManager.Instance.GetOnlineStatus(Node.ID);
                if (onlineStatus != null && onlineStatus.Online)
                    IsOnline = true;
                else
                    IsOnline = false;
            }
        }

        public void UpdateDisplayVideos(List<string> videoIds)
        {
            var source = VideoChildren.Source as List<VideoThumbnailsViewModel>;
            source?.ForEach(e => e.IsOnPlaying = (videoIds != null && videoIds.Exists(str => str == e.ID)));
        }

        void updateVideo()
        {
            var oldSource = VideoChildren.Source as List<VideoThumbnailsViewModel>;
            oldSource?.ToList().ForEach(e => e.Dispose());
            if (Node != null && Node.Children.Length > 0)
            {
                if (IsUnfoldVideo)
                {
                    var videoList = Node.Children.Where(e => e.Type == CctvNode.NodeType.Video).Select(e => new VideoThumbnailsViewModel() { ID = e.ID, Name = e.Name }).ToList();
                    VideoChildren.Source = videoList;
                }
                else
                    VideoChildren.Source = null;
            }
            else
            {
                VideoChildren.Source = null;
            }
            SelectedNode = null;
            SelectedVideo = null;
            VideoChildren.View?.MoveCurrentTo(null);
        }
    }
}

