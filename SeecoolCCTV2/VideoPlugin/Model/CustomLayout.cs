using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.Model
{
    internal class CustomLayout:SplitScreenInfo
    {
        [AutoNotify]
        public string LayoutName { get; set; }
    }
}
