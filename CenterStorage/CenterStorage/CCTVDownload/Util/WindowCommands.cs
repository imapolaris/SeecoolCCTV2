using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVDownload.Util
{
    public static class WindowCommands
    {
        static WindowCommands()
        {
            Close = new RoutedUICommand("Close MainWindow", "关闭", typeof(WindowCommands));
            Minimize = new RoutedUICommand("Minimize MainWindow", "最小化", typeof(WindowCommands));
            Maximize = new RoutedUICommand("Maximize MainWindow", "最大化", typeof(WindowCommands));
            Restore = new RoutedUICommand("Restore MainWindow", "还原", typeof(WindowCommands));
        }

        public static RoutedUICommand Close{ get; private set; }
        public static RoutedUICommand Minimize { get; private set; }
        public static RoutedUICommand Maximize { get; private set; }
        public static RoutedUICommand Restore { get; private set; }
    }
}
