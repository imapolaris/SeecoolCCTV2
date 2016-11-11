using AopUtil.WpfBinding;
using CCTVReplay.Proxy;
using CCTVReplay.Util;
using CCTVReplay.VideoTree.Model;
using CCTVReplay.VideoTree.Thumbnail;
using CenterStorageCmd;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CCTVReplay.StaticInfo;
using CCTVReplay.Source;
using Common.Util;
using Common.Log;

namespace CCTVReplay.VideoTree.Search
{
    public class SearcherViewModel : ObservableObject
    {
        [AutoNotify]
        public CollectionViewSource ResultsSource { get; private set; }
        [AutoNotify]
        public SearchNodeHistoryViewModel SelectedNode { get; set; }
        [AutoNotify]
        public TimePeriodPacket SearchTimePeriod { get; set; }
        [AutoNotify]
        public bool AllowDrag { get; set; }

        static int SearchedResultShowThumbnailsSup = 50;
        private VideoDataInfoProxy _diProxy;
        public SearcherViewModel()
        {
            _diProxy = new VideoDataInfoProxy();
            _diProxy.VideoDataInfoReceived += diProxy_VideoDataInfoReceived;
            _searcherManager = new SearcherManager();
            ResultsSource = new CollectionViewSource();
            IsVisible = true;
            PropertyChanged += onPropertyChanged;
            _searcherManager.SearchResultEvent += onSearchResult;
            HomeCommand = new DelegateCommand(_ => onHome());
            BackToPrevCommand = new DelegateCommand(_ => onBackToPrevCommand());
            AllowDrag = true;
            SearchContext = string.Empty; //此举将导致树节点刷新
            VideoInfoManager.Instance.DataSourceChanged += onDataSourceChanged;
            VideoInfoManager.Instance.LocalSourceInfoReceived += onNodeUpdated;
        }

        private void onNodeUpdated(LocalVideosInfoPacket packet)
        {
            //更新节点树。
            if (string.IsNullOrWhiteSpace(SearchContext))
                updateFilteredVideos();
            else
                SearchContext = string.Empty;
        }

        private void onDataSourceChanged(object sender, EventArgs e)
        {
            //更新节点树。
            if (string.IsNullOrWhiteSpace(SearchContext))
                updateFilteredVideos();
            else
                SearchContext = string.Empty;

            VideoDataSource src = VideoInfoManager.Instance.GetStorageSource();
            //设置集中存储数据源
            UpdateSource(src);
        }

        private void diProxy_VideoDataInfoReceived(object sender, VideoDataInfoEventArgs e)
        {
            WindowUtil.BeginInvoke(() =>
            {
                updateVideoDataInfo(e.VideoId, e.StreamId, e.TimePeriods);
            });
        }

        private void updateVideoDataInfo(string videoId, int streamId, TimePeriodPacket[] periods)
        {
            foreach (SearchNodeHistoryViewModel vm in ResultsSource.Source as List<SearchNodeHistoryViewModel>)
            {
                if (vm.IsUnfoldVideo)
                {
                    var nodes = vm.VideoChildren.Source as List<VideoThumbnailsViewModel>;
                    var pNode = nodes.Find(n => n.ID == videoId);
                    if (pNode != null)
                    {
                        pNode.StreamId = streamId;
                        pNode.VideoDataInfo.UpdateValidRange(periods);
                    }
                }
            }
        }

        public void ClearSearcher()
        {
            removeOldSource();
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SearchContext):
                    if (IsVisible)
                        updateFilteredVideos();
                    break;
                case nameof(IsVisible):
                    ResetSearchContext();
                    break;
                case nameof(SearchTimePeriod):
                    RefreshVideoDataSummary();
                    break;
            }
        }

        private void RefreshVideoDataSummary()
        {
            if (SearchTimePeriod == null || !_diProxy.SourceValid)
                return;
            try
            {
                foreach (SearchNodeHistoryViewModel vm in ResultsSource.Source as List<SearchNodeHistoryViewModel>)
                {
                    if (vm.IsUnfoldVideo)
                    {
                        List<VideoInfo> viList = new List<VideoInfo>();
                        var nodes = vm.VideoChildren.Source as List<VideoThumbnailsViewModel>;
                        foreach (CctvNode cn in vm.Node.Children)
                        {
                            if (cn.Type == CctvNode.NodeType.Video)
                            {
                                var pNode = nodes.Find(n => n.ID == cn.ID);
                                if (pNode != null)
                                {
                                    viList.Add(new VideoInfo(cn.ID, pNode.StreamId));
                                    pNode.VideoDataInfo.UpdateTimePacket(SearchTimePeriod.BeginTime, SearchTimePeriod.EndTime);
                                }
                            }
                        }
                        _diProxy.GetVideoDataInfoAsync(viList.ToArray(), SearchTimePeriod.BeginTime, SearchTimePeriod.EndTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Default.Error("刷新视频录像信息失败!", ex);
                string msg = "刷新视频录像信息失败!\n" + ex.Message;
                DialogUtil.ShowError(msg);
            }
        }

        public void UpdateSource(VideoDataSource src)
        {
            _diProxy.UpdateSource(src);
            if (src.SrcType == SourceType.Local)
            {
                try
                {
                    _diProxy.GetVideoTreeNodesAsync();
                }
                catch (Exception ex)
                {
                    Logger.Default.Error("刷新本地数据源视频录像信息失败！", ex);
                    string msg = "刷新本地数据源视频录像信息失败！\n" + ex.Message;
                    DialogUtil.ShowError(msg);
                    return;
                }
            }
            RefreshVideoDataSummary();
        }

        public void ResetSearchContext()
        {
            if (IsVisible)
                SearchContext = SearchContext == string.Empty ? null : string.Empty;
        }

        public void OpenSelectedNode()
        {
            if (SelectedNode != null && (!SelectedNode.IsUnfoldVideo || !SelectedNode.Node.Children.Any(_ => _.Type == CctvNode.NodeType.Video)))
                Update(SelectedNode.Node);
        }

        public void Update(List<CctvNode> nodes, bool showThumbnails = true)
        {
            if (nodes != null && nodes.Count == 1 && nodes.First().IsCctvNode())//单搜索结果节点展开
                Update(nodes[0], false);
            else
            {
                removeOldSource();
                NodeVisible = false;
                if (showThumbnails && tooMuchDatasToShow(nodes))
                    showThumbnails = false;
                updateNewSource(nodes, showThumbnails);
                updateDisplayVideos();
                RefreshVideoDataSummary();
            }
        }

        bool tooMuchDatasToShow(List<CctvNode> nodes)
        {
            if (nodes == null)
                return false;
            int count = nodes.Count;
            if (count > SearchedResultShowThumbnailsSup)
                return true;
            count = nodes.Sum(n => 1 + n.Children.Count(_ => _.Type == CctvNode.NodeType.Video));
            if (count > SearchedResultShowThumbnailsSup)
                return true;
            return false;
        }

        public void Update(CctvNode node, bool showThumbnails = false)
        {
            removeOldSource();
            if (node.IsCctvNode())
            {
                Node = node;
                NodeVisible = true;
                if (Node != null && !string.IsNullOrEmpty(Node.ID))
                {
                    var onlineStatus = VideoInfoManager.Instance.GetOnlineStatus(Node.ID);
                    if (onlineStatus != null && onlineStatus.Online)
                        IsOnline = true;
                    else
                        IsOnline = false;
                }
                ControlVisible = (Node.ID != getAllNodes()?.ID);
                updateNewSource(Node?.Children.ToList(), false);
            }
            else
            {
                NodeVisible = false;
                updateNewSource(new List<CctvNode>() { node }, showThumbnails);
            }
            updateDisplayVideos();
            RefreshVideoDataSummary();
        }

        public event Action<string, int, string> PlayVideoEvent;
        public void PlayVideo(string videoId, int streamId, string name = null)
        {
            if (!string.IsNullOrEmpty(videoId) && PlayVideoEvent != null)
                PlayVideoEvent(videoId, streamId, name);
        }

        List<string> _displayVideos = null;
        public void UpdateDisplayVideos(List<string> videoIds)
        {
            _displayVideos = videoIds;
            updateDisplayVideos();
        }

        void updateDisplayVideos()
        {
            var source = ResultsSource.Source as List<SearchNodeHistoryViewModel>;
            source?.ForEach(e => e.UpdateDisplayVideos(_displayVideos));
        }

        private void updateVideoIds(List<string> ids, CctvNode node)
        {
            if (node == null)
                return;
            if (node.Type == CctvNode.NodeType.Video)
                ids.Add(node.ID);
            else
            {
                foreach (var child in node.Children)
                    updateVideoIds(ids, child);
            }
        }

        private void removeOldSource()
        {
            var source = ResultsSource.Source as List<SearchNodeHistoryViewModel>;
            source?.ForEach(e => e.UpdateNode(null));
            ResultsSource.Source = null;
        }

        private void updateNewSource(List<CctvNode> nodes, bool showThumbnails)
        {
            List<SearchNodeHistoryViewModel> modellist = new List<SearchNodeHistoryViewModel>();
            addNewSourceAboutAllVideoChildren(nodes, modellist);
            addNewSourceAboutAllNodeChildren(nodes, modellist, showThumbnails);
            ResultsSource.Source = modellist;
            ResultsSource.View?.MoveCurrentTo(null);
        }

        void addNewSourceAboutAllVideoChildren(List<CctvNode> nodes, List<SearchNodeHistoryViewModel> modellist)
        {
            var videolist = nodes?.Where(e => e.Type == CctvNode.NodeType.Video).ToList();
            if (videolist != null && videolist.Count > 0)
            {
                var videoModel = new SearchNodeHistoryViewModel(AllowDrag);
                videoModel.UpdateNode(new CctvNode() { Name = "视频", Type = CctvNode.NodeType.Unknown, Children = videolist.ToArray() }, true);
                modellist.Add(videoModel);
            }
        }

        void addNewSourceAboutAllNodeChildren(List<CctvNode> nodes, List<SearchNodeHistoryViewModel> modellist, bool showThumbnails)
        {
            var nodelist = nodes?.Where(e => e.IsCctvNode()).ToList();
            if (nodelist != null && nodelist.Count > 0)
            {
                nodelist?.ForEach(e =>
                {
                    SearchNodeHistoryViewModel model = new SearchNodeHistoryViewModel(AllowDrag);
                    model.UpdateNode(e, showThumbnails);
                    modellist.Add(model);
                });
            }
        }

        #region 节点树返回、首页等信息

        [AutoNotify]
        public bool NodeVisible { get; set; }

        [AutoNotify]
        public bool ControlVisible { get; set; }

        [AutoNotify]
        public CctvNode Node { get; set; }
        [AutoNotify]
        public bool IsOnline { get; set; }

        #region HOME
        public ICommand HomeCommand { get; set; }

        private void onHome()
        {
            if (SearchContext != "")
                SearchContext = "";
            else
                firePropertyChanged(nameof(SearchContext));
        }
        #endregion HOME
        #region 返回上一级
        public ICommand BackToPrevCommand { get; set; }

        private void onBackToPrevCommand()
        {//返回上一级
            if (Node == null)
                return;
            var prev = getBackToPrev(getAllNodes(), Node.ID);
            if (prev != null)
                Update(prev);
        }

        private CctvNode getBackToPrev(CctvNode cctvNode, string id)
        {
            if (cctvNode == null)
                return null;
            if (cctvNode.ID == id)
                return null;
            var result = cctvNode.Children?.FirstOrDefault(e => e.ID == id);
            if (result != null)
                return cctvNode;
            foreach (var child in cctvNode.Children)
            {
                var node = getBackToPrev(child, id);
                if (node != null)
                    return node;
            }
            return null;
        }

        CctvNode getAllNodes()
        {
            return CctvNode.DeepClone(VideoInfoManager.Instance.GetHierarchyRoot());
        }
        #endregion 返回上一级
        #endregion 节点树返回、首页等信息

        #region 全局

        [AutoNotify]
        public bool IsVisible { get; set; }

        [AutoNotify]
        public String SearchContext { get; set; }

        [AutoNotify]
        public bool IsSearching { get; set; }

        #endregion 全局

        #region 搜索结果
        SearcherManager _searcherManager;

        private void onSearchResult(string searchInfo, List<CctvNode> searched)
        {
            if (searchInfo == SearchContext)
            {
                IsSearching = false;
                try
                {
                    Common.Util.WindowUtil.BeginInvoke(() => Update(searched, !string.IsNullOrWhiteSpace(SearchContext)));
                }
                catch
                { }
            }
        }

        private void updateFilteredVideos()
        {
            if (!string.IsNullOrWhiteSpace(SearchContext))
            {
                Update(new List<CctvNode>());
                IsSearching = true;
            }
            else
                IsSearching = false;
            _searcherManager.UpdateSearch(SearchContext);
        }
        #endregion 搜索结果
    }
}