using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.Helper
{
    internal static class DirectoryHelper
    {
        public const string ConfigDirectory = ".\\Plugins\\VideoPlugin\\config";
        private const string ConfigDirRegex = ConfigDirectory + "\\{0}";
        public const string TimeSwitchSchemeName = "定时切换方案";
        public const string TimeSwitchFileExt = "switch";
        public const string LayoutSchemeName = "布局方案文件";
        public const string LayoutFileExt = "layout";
        

        public static string GetConfigFilePath(string fileName)
        {
            return string.Format(ConfigDirRegex, fileName);
        }
    }
}
