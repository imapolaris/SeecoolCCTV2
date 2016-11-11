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
using VideoNS.Model;

namespace VideoNS.AutoSave
{
    public static class LayoutAutoSave
    {
        public const string LayoutFile = "autolayout.layoutscheme";
        private const string KeyString = "layout";

        public static void SaveData(SplitScreenInfo info)
        {
            try
            {
                //JsonParser.SerializeToFile(info, DirectoryHelper.GetConfigFilePath(LayoutFile), true);
                var file = new AggregateCustomSettingFile<string, SplitScreenInfo>(LayoutFile);
                file[ProcessHelper.Instance.FormatKey(KeyString)] = info;
            }
            catch (Exception e)
            {
                Console.WriteLine("保存布局方案失败:" + e.Message);
            }
        }

        private static SplitScreenInfo _infoData;
        public static void LazySaveData(SplitScreenInfo info)
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

        public static SplitScreenInfo LoadData()
        {
            SplitScreenInfo info = null;
            //info = JsonParser.DeserializeFromFile<SplitScreenInfo>(DirectoryHelper.GetConfigFilePath(LayoutFile));
            var file = new AggregateCustomSettingFile<string, SplitScreenInfo>(LayoutFile);
            info = file[ProcessHelper.Instance.FormatKey(KeyString)];
            return info;
        }
    }
}
