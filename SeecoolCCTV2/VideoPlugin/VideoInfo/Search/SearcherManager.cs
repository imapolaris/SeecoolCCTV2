using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoNS.VideoInfo.Search
{
    public class SearcherManager
    {
        string _searchInfo;
        public Action<string, List<CctvNode>> SearchResultEvent;
        public void UpdateSearch(string searchInfo)
        {
            _searchInfo = searchInfo;
            new Thread(run).Start();
        }

        private void run()
        {
            search(_searchInfo);
        }

        private void search(string searchInfo)
        {
            CctvNode node = getAllNodes();
            List<CctvNode> searched = null;
            if (node != null && searchInfo == _searchInfo)
                searched = string.IsNullOrWhiteSpace(searchInfo) ? new List<CctvNode>() { node } : VideoSearcher.Search(node, searchInfo);
            if (searchInfo == _searchInfo)
                updateSearch(_searchInfo, searched);
        }
        

        private void updateSearch(string searchInfo, List<CctvNode> searched)
        {
            if (SearchResultEvent != null)
                SearchResultEvent(searchInfo, searched);
        }

        CctvNode getAllNodes()
        {
            return CctvNode.DeepClone(CCTVInfoManager.Instance.GetHierarchy());
        }
    }
}
