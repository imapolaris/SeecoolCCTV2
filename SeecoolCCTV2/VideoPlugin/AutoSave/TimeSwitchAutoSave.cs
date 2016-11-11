using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Configuration.CustomSetting;
using VideoNS.Helper;
using VideoNS.Json;
using VideoNS.TimeSwitch;

namespace VideoNS.AutoSave
{
    public static class TimeSwitchAutoSave
    {
        public const string TimingSwitchFile = "autotimeswitch.switchscheme";
        private const string KeyString = "timeswitch";

        public static void SaveData(TimeSwitchInfo[] infos)
        {
            try
            {
                //JsonParser.SerializeToFile(infos, DirectoryHelper.GetConfigFilePath(TimingSwitchFile), true);
                var file = new AggregateCustomSettingFile<string, TimeSwitchInfo[]>(TimingSwitchFile);
                file[ProcessHelper.Instance.FormatKey(KeyString)] = infos;
            }
            catch (Exception e)
            {
                Console.WriteLine("保存定时切换方案失败:" + e.Message);
            }
        }

        private static TimeSwitchInfo[] _infoData;

        public static void LazySaveData(TimeSwitchInfo[] info)
        {
            _infoData = info;
            LazySave();
        }
        #region 【执行LazySave】
        private static Thread _saveThread;
        private static void LazySave()
        {
            if (_saveThread != null && _saveThread.IsAlive)
                return;
            _saveThread = new Thread(DoSave);
            _saveThread.IsBackground = true;
            _saveThread.Start();
        }

        private static void DoSave()
        {
            try
            {
                Thread.Sleep(50); //延时50ms执行。
                SaveData(_infoData);
                _infoData = null;
            }
            catch (ThreadAbortException) { }
        }
        #endregion 【执行LazySave】

        public static TimeSwitchInfo[] LoadData()
        {
            TimeSwitchInfo[] schemes = null;
            //schemes = JsonParser.DeserializeFromFile<List<TimeSwitchInfo>>(DirectoryHelper.GetConfigFilePath(TimingSwitchFile));
            var file = new AggregateCustomSettingFile<string, TimeSwitchInfo[]>(TimingSwitchFile);
            schemes = file[ProcessHelper.Instance.FormatKey(KeyString)];
            return schemes;
        }
    }
}
