using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AopUtil.WpfBinding;
using System.Windows;

namespace VideoNS.SubWindow
{
    public class PopupWinModel : ObservableObject
    {
        public PopupWinModel()
        {
            ControlModel = new VideoControlModel(true) { CloseBtnVisibility = Visibility.Collapsed, FullScreenBtnVisibility = Visibility.Collapsed };
        }

        public PopupWinModel(string videoId) : this()
        {
            ControlModel.VideoId = videoId;
        }

        [AutoNotify]
        public VideoControlModel ControlModel { get; set; }
    }
}
