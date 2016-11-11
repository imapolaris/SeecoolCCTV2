using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoNS.Model
{
    public class SplitScreenLayoutModel: ObservableObject
    {
        [AutoNotify]
        public string Header { get; set; }

        [AutoNotify]
        public SplitScreenInfo SplitScreenInfom { get; set; }

        public bool IsValibleCloseButton { get; set; }

        public SplitScreenType SplitType { get; set; }

        public ICommand DeleteCommand { get; set; }
    }

    public enum SplitScreenType
    {
        等分屏,
        一大多小,
        两大多小,
        自定义
    }
}
