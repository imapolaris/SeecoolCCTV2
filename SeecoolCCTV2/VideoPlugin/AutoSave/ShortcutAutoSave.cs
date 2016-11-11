using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Configuration.CustomSetting;
using VideoNS.Helper;
using VideoNS.SubWindow;

namespace VideoNS.AutoSave
{
    public static class ShortcutAutoSave
    {
        public const string ShortcutFile = "splitshortcut.ssct";
        private const string KeyString = "shortcut";

        public static void SaveData(List<ShortcutItem> info)
        {
            try
            {
                var file = new AggregateCustomSettingFile<string, List<ShortcutItem>>(ShortcutFile);
                file[ProcessHelper.Instance.FormatKey(KeyString)] = info;
            }
            catch (Exception e)
            {
                Console.WriteLine("保存快捷方式设置失败:" + e.Message);
            }
        }

        public static ShortcutItem[] LoadData()
        {
            ShortcutItem[] info = null;
            var file = new AggregateCustomSettingFile<string, ShortcutItem[]>(ShortcutFile);
            info = file[ProcessHelper.Instance.FormatKey(KeyString)];
            return info;
        }
    }
}
