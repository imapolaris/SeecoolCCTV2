using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace CCTV2InfoDeploy.Models
{
    public class PhysicalNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public NodeType Type { get; set; }
        public PhysicalNode[] Children { get; set; } = EmptyChildren;

        private static readonly PhysicalNode[] EmptyChildren = new PhysicalNode[0];
    }
}
