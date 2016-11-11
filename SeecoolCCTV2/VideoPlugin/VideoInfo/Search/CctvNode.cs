using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace VideoNS.VideoInfo.Search
{
    public class CctvNode
    {
        public enum NodeType { Unknown = 0, Video, Front, Server };

        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; } = NodeType.Unknown;
        public CctvNode[] Children { get; set; } = _zeroChild;

        private static CctvNode[] _zeroChild = new CctvNode[0];

        public bool IsCctvNode()
        {
            return Type == NodeType.Front || Type == NodeType.Server;
        }

        public static CctvNode ToDeepClone(CctvNode cctvNode)
        {
            CctvNode node = ToShallowClone(cctvNode);
            if (cctvNode.Children.Length > 0)
            {
                node.Children = new CctvNode[cctvNode.Children.Length];
                for (int i = 0; i < cctvNode.Children.Length; i++)
                    node.Children[i] = ToDeepClone(cctvNode.Children[i]);
            }
            return node;
        }

        public static CctvNode ToShallowClone(CctvNode cctvNode)
        {
            return new CctvNode()
            {
                ID = cctvNode.ID,
                Type = cctvNode.Type,
                Name = cctvNode.Name
            };
        }

        public static CctvNode DeepClone(CCTVHierarchyNode hNode)
        {
            try
            {
                if (hNode == null)
                    return null;
                CctvNode node = cctvNodeDeepClone(hNode);
                return node;
            }
            catch
            {
                return null;
            }
        }

        public static CctvNode ShallowClone(CCTVHierarchyNode cctvNode)
        {
            return new CctvNode()
            {
                ID = cctvNode.ElementId,
                Type = stringToNodeType(cctvNode.Type),
                Name = cctvNode.Name
            };
        }

        private static CctvNode cctvNodeDeepClone(CCTVHierarchyNode cctvNode)
        {
            CctvNode node = ShallowClone(cctvNode);
            if (cctvNode.Children.Length > 0)
            {
                node.Children = new CctvNode[cctvNode.Children.Length];
                for (int i = 0; i < cctvNode.Children.Length; i++)
                    node.Children[i] = cctvNodeDeepClone(cctvNode.Children[i]);
            }
            return node;
        }

        private static CctvNode.NodeType stringToNodeType(CCTVModels.NodeType type)
        {
            CctvNode.NodeType nodeType = CctvNode.NodeType.Unknown;
            switch (type)
            {
                case CCTVModels.NodeType.Server:
                    nodeType = NodeType.Front;
                    break;
                case CCTVModels.NodeType.Video:
                    nodeType = NodeType.Video;
                    break;
                default:
                    break;
            }
            return nodeType;
        }

        internal static CctvNode DeepClone(object p)
        {
            throw new NotImplementedException();
        }
    }
}
