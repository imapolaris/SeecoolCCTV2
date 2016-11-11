using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS
{
    public class VideoInfoMessageViewModel : ObservableObject
    {
        public VideoInfoMessageViewModel()
        {
            IsViewMessage = false;
            VideoMessage = "消息面板展示";
        }
        [AutoNotify]
        public bool IsViewMessage { get; set; }
        [AutoNotify]
        public string VideoMessage { get; set; }
    }
}
