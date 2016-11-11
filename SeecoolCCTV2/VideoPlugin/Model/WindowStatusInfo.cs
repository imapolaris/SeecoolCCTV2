using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VideoNS.Model
{
    internal class WindowStatusInfo
    {
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public WindowState WindowState { get; set; } = WindowState.Normal;
    }
}
