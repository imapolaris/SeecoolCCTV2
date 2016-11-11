using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVClient;
using CCTVModels;

namespace CCTVInfoAdapter
{
    public class HierarchyClassifier
    {
        private Dictionary<string, CCTVHierarchyInfo> _dictHier = new Dictionary<string, CCTVHierarchyInfo>();

        public Dictionary<string,CCTVHierarchyInfo> Hierarchies
        {
            get { return _dictHier; }
        }

        public HierarchyClassifier()
        {

        }

        public void Classify(VideoParser.Node node)
        {
            getHierarchyInfo(null,node);
        }

        static string getNodeId(ulong id)
        {
            return $"CCTV1_{id:X}";
        }

        static NodeType getNodeType(VideoParser.Node node)
        {
            if (node is VideoParser.Server)
                return NodeType.Server;
            else if (node is VideoParser.Front)
                return NodeType.Server;
            else
                return NodeType.Video;
        }

        private void getHierarchyInfo(string parentId, VideoParser.Node node)
        {
            string nodeId = getNodeId(node.Id);
            _dictHier[nodeId] = new CCTVHierarchyInfo()
            {
                Id = nodeId,
                Name = node.Name,
                Type = getNodeType(node),
                ParentId = parentId,
                ElementId = nodeId
            };

            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                if (server.Childs != null)
                    foreach (VideoParser.Node child in server.Childs)
                        getHierarchyInfo(nodeId, child);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    if (front.Childs != null)
                        foreach (VideoParser.Video child in front.Childs)
                            getHierarchyInfo(nodeId, child);
                }
            }
        }

        private void getHierarchyInfo(string parentId, VideoParser.Video video)
        {
            string videoId = getNodeId(video.Id);
            _dictHier[videoId] = new CCTVHierarchyInfo()
            {
                Id = videoId,
                Name = video.Name,
                Type = NodeType.Video,
                ParentId = parentId,
                ElementId = videoId
            };
        }
    }
}
