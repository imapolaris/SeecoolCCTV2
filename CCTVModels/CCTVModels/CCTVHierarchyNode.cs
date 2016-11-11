using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVHierarchyNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; } = NodeType.Unknow;
        public string ElementId { get; set; }
        public CCTVHierarchyNode[] Children { get; set; } = _zeroChild;

        private static CCTVHierarchyNode[] _zeroChild = new CCTVHierarchyNode[0];
    }
}
