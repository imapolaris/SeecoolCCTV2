using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AopUtil.WpfBinding;
using Newtonsoft.Json;
using VideoNS.Json;

namespace VideoNS.Model
{

    /// <summary>
    /// 分屏区域节点。
    /// </summary>
    public class SplitScreenNode : ObservableObject
    {
        private int _rowSpan = 1;
        private int _columnSpan = 1;

        public SplitScreenNode()
        {
            this.PropertyChanged += SplitScreenNode_PropertyChanged;
        }
        private void SplitScreenNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnDataChanged(new EventArgs());
        }

        public event EventHandler DataChanged;

        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }

        /// <summary>
        /// 行编号
        /// </summary>
        [AutoNotify]
        public int Row { get; set; }

        /// <summary>
        /// 列编号
        /// </summary>
        [AutoNotify]
        public int Column { get; set; }

        /// <summary>
        /// 行跨度。
        /// </summary>
        public int RowSpan
        {
            get { return _rowSpan; }
            set
            {
                int val = value < 1 ? 1 : value;
                updateProperty(ref _rowSpan, val);
            }
        }

        /// <summary>
        /// 列跨度。
        /// </summary>
        public int ColumnSpan
        {
            get { return _columnSpan; }
            set
            {
                int val = value < 1 ? 1 : value;
                updateProperty(ref _columnSpan, val);
            }
        }

        /// <summary>
        /// 视频ID
        /// </summary>
        [AutoNotify]
        public string VideoId { get; set; }
    }
}
