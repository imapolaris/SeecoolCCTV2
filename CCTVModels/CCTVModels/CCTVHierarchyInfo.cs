using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVHierarchyInfo
    {
        public string Id { get; set; } = null;
        public string Name { get; set; } = null;
        public NodeType Type { get; set; } = NodeType.Unknow;
        /// <summary>
        /// 该属性表示虚拟节点的关联物理节点ID，
        /// <para>如果<see cref="Type"/>的值是<see cref="NodeType.Server"/>,则此属性值表示节点服务器ID。</para>
        /// <para>如果<see cref="Type"/>的值是<see cref="NodeType.Video"/>，则此属性值表示视频节点ID。</para>
        /// </summary>
        public string ElementId { get; set; }
        public string ParentId { get; set; } = null;
    }

    public enum NodeType
    {
        Unknow = 0,
        Server = 1,
        Video = 2,
    }
}
