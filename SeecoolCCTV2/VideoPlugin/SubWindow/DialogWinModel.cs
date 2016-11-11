using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AopUtil.WpfBinding;

namespace VideoNS.SubWindow
{
    public class DialogWinModel:ObservableObject
    {
        public DialogWinModel()
        {
            Title = "提示";
            Content = "测试提示内容";
            Image = DialogWinImage.None;
        }

        [AutoNotify]
        public string Title { get; set; }

        [AutoNotify]
        public string Content { get; set; }

        [AutoNotify]
        public Visibility ButtonCancelVisible { get; set; }

        [AutoNotify]
        public DialogWinImage Image { get; set; }
    }
}
