using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AopUtil.WpfBinding;
using VideoNS.Json;

namespace VideoNS.Model
{
    /// <summary>
    /// 分屏显示视频屏幕信息。
    /// </summary>
    public class SplitScreenInfo: ObservableObject
    {
        public SplitScreenInfo()
        {
            this.PropertyChanged += SplitScreenInfo_PropertyChanged;
        }

        private void SplitScreenInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnDataChanged(new EventArgs());
        }

        /// <summary>
        /// 横纵向的分割数。
        /// </summary>
        [AutoNotify]
        public int Split { get; set; }

        private SplitScreenNode[] _nodes;
        /// <summary>
        /// 视频区域节点组。
        /// </summary>
        public SplitScreenNode[] Nodes {
            get { return _nodes; }
            set
            {
                UninstallSubEvent(_nodes);
                updateProperty(ref _nodes, value);
                InstallSubEvent(_nodes);
            }
        }

        public event EventHandler DataChanged;

        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }

        private void InstallSubEvent(SplitScreenNode[] nodes)
        {
            if (nodes != null)
            {
                foreach(SplitScreenNode node in nodes)
                    if(node!=null)
                        node.DataChanged += Node_DataChanged;
            }
        }

        private void UninstallSubEvent(SplitScreenNode[] nodes)
        {
            if (nodes != null)
            {
                foreach(SplitScreenNode node in nodes)
                    if(node!=null)
                        node.DataChanged -= Node_DataChanged;
            }
        }

        private void Node_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged(e);
        }

        public SplitScreenInfo Clone()
        {
            int len = Nodes != null ? Nodes.Length : 0;
            var clone = (SplitScreenInfo)MemberwiseClone();
            List<SplitScreenNode> nodes = new List<SplitScreenNode>();
            for (int i = 0; i < len; i++)
            {
                var node = Nodes[i];
                var newNode = new SplitScreenNode()
                {
                    Column = node.Column,
                    Row = node.Row,
                    ColumnSpan = node.ColumnSpan,
                    RowSpan = node.RowSpan,
                    VideoId = node.VideoId
                };
                nodes.Add(newNode);
            }
            clone.Nodes = nodes.ToArray();
            return clone;
        }
    }
}
