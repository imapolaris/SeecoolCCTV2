using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVReplay.Util
{
    public static class UrlCommands
    {
        static UrlCommands()
        {
            Import = new RoutedUICommand("Import URL", "导入URL", typeof(UrlCommands));
            Export = new RoutedUICommand("Export URL", "导出URL", typeof(UrlCommands));
        }

        public static RoutedUICommand Import { get; private set; }
        public static RoutedUICommand Export { get; private set; }
    }
}
