using Common.Configuration.CustomSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.Helper;
using VideoNS.Model;

namespace VideoNS.AutoSave
{
    internal static class WindowStatusAutoSave
    {
        public const string WindowStateFile = "windowstatus.wins";
        private const string KeyString = "windowstatus";

        internal static void SaveData(WindowStatusInfo data)
        {
            try
            {
                var file = new AggregateCustomSettingFile<string, WindowStatusInfo>("windowstatus.wins");
                file[ProcessHelper.Instance.FormatKey(KeyString)] = data;
            }
            catch(Exception e)
            {
                Console.WriteLine("保存窗口状态失败:" + e.Message);
            }
        }

        internal static WindowStatusInfo LoadData()
        {
            var file = new AggregateCustomSettingFile<string, WindowStatusInfo>("windowstatus.wins");
            return file[ProcessHelper.Instance.FormatKey(KeyString)];
        }
    }
}
