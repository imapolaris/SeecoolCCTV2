using Common.Configuration.CustomSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoNS.Model;

namespace VideoNS.AutoSave
{
    internal static class CustomLayoutAutoSave
    {
        public const string LayoutDesignFile = "customlayout.cslayout";
        private const string KeyString = "customLayout";

        public static void SaveData(List<CustomLayout> layouts)
        {
            try
            {
                if (layouts != null)
                {
                    var file = new AggregateCustomSettingFile<string, List<CustomLayout>>(LayoutDesignFile);
                    file[KeyString] = layouts;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("保存自定义布局方案失败:" + e.Message);
            }
        }

        public static List<CustomLayout> LoadData()
        {
            var file = new AggregateCustomSettingFile<string, List<CustomLayout>>(LayoutDesignFile);
            List<CustomLayout> layouts = file[KeyString];
            return layouts;
        }
    }
}
