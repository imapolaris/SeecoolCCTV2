using CCTVReplay.Util;
using CCTVReplay.VideoTree.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.VideoTree.Search
{
    public class VideoSearcher
    {
        static char[] separator = new char[] { ' ' };

        public static List<CctvNode> Search(CctvNode cctvNode, string info)
        {
            List<CctvNode> result = new List<CctvNode>();
            List<string> searchInfo = info.ToLower().Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
            updateSearch(cctvNode, searchInfo, result);
            return result;
        }

        private static void updateSearch(CctvNode cctvNode, List<string> searchInfo, List<CctvNode> result, string nodeName = "")
        {
            if (cctvNode == null)
                return;
            if (isSearched(cctvNode.Name, searchInfo))
            {
                var clone = CctvNode.ToDeepClone(cctvNode);
                if (clone.Type == CctvNode.NodeType.Video && !string.IsNullOrWhiteSpace(nodeName))
                    clone.Name = string.Format("{0}-{1}", nodeName, clone.Name);
                result.Add(clone);
                if (cctvNode.Children.Any(_ => _.Type == CctvNode.NodeType.Video))//
                    return;
            }
            if (cctvNode.Children.Length > 0)
            {
                for (int i = 0; i < cctvNode.Children.Length; i++)
                {
                    updateSearch(cctvNode.Children[i], searchInfo, result, cctvNode.Name);
                }
            }
        }

        private static bool isSearched(string name, List<string> searchInfos)
        {
            if (searchInfos.Count == 0)
                return true;
            ShouZiMuArray szms = PinYinConverter.ToShouZiMuArray(name);
            foreach (var info in searchInfos)
            {
                int index = szms.IndexOf(info);
                if (index < 0)
                    return false;
            }
            return true;
        }
    }
}
