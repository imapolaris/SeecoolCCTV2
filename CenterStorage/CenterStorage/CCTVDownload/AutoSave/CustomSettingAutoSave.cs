using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownload.AutoSave
{
    public class CustomSettingAutoSave
    {
        private static CustomSettingAutoSave _instance;
        public static CustomSettingAutoSave Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomSettingAutoSave();
                return _instance;
            }
        }
        
        private const string ConfigFile = "config\\download.json";
        private CustomSetting _setting;

        public CustomSetting Setting
        {
            get
            {
                if (_setting == null)
                    loadData();
                return _setting;
            }
        }

        private CustomSettingAutoSave()
        {

        }

        private void loadData()
        {
            uninstallEvent(_setting);
            _setting = new CustomSetting();
            try
            {
                FileInfo fi = new FileInfo(ConfigFile);
                if (fi.Exists)
                {
                    using (StreamReader sr = fi.OpenText())
                    {
                        string jStr = sr.ReadToEnd();
                        _setting = JsonConvert.DeserializeObject<CustomSetting>(jStr);
                    }
                }
            }
            catch { }
            installEvent(_setting);
        }

        private void installEvent(CustomSetting setting)
        {
            if (setting != null)
                setting.DataChanged += Setting_DataChanged;
        }

        private void uninstallEvent(CustomSetting setting)
        {
            if (setting != null)
                setting.DataChanged -= Setting_DataChanged;
        }

        private void Setting_DataChanged(object sender, EventArgs e)
        {
            saveData();
        }

        private void saveData()
        {
            if (Setting != null)
            {
                FileInfo fi = new FileInfo(ConfigFile);
                if (!fi.Directory.Exists)
                    fi.Directory.Create();
                using (FileStream fs = new FileStream(fi.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string str = JsonConvert.SerializeObject(Setting);
                        sw.Write(str);
                    }
                }
            }
        }
    }
}
