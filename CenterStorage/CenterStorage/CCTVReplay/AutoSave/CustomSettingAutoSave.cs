using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Util;
using Newtonsoft.Json;

namespace CCTVReplay.AutoSave
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


        private static readonly string ConfigFile = Path.Combine(ConstSettings.ConfigPath, "download.json");
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
            loadData();
        }

        private void loadData()
        {
            uninstallEvent(_setting);
            _setting = new CustomSetting();
            _setting.DownloadPath = "D:\\视酷下载";// Path.Combine(Environment.CurrentDirectory, "download");
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
            initDefaultDownloadPath(_setting);
            ConstSettings.CachePath = Path.Combine(_setting.DownloadPath, "Cache");
        }

        static long HardDiskInf = (long)10 * 1024 * 1024 * 1024;//硬盘分区大小最小为10G
        private void initDefaultDownloadPath(CustomSetting setting)
        {
            DirectoryInfo root = new DirectoryInfo(setting.DownloadPath).Root;
            if (root.Exists)
            {
                var di = CenterStorageCmd.HardDiskSpaceManager.GetHardDiskSpace(root.FullName);
                if (di > HardDiskInf)
                    return;
            }
            setting.DownloadPath = getDefaultDownloadPath();
        }

        private string getDefaultDownloadPath()
        {
            DriveInfo[] d = DriveInfo.GetDrives();
            //获取硬盘分区大小在10G以上的所有硬盘存储空间并排序
            DriveInfo[] list = d.Where(di => di.DriveType == DriveType.Fixed && di.RootDirectory.Exists && di.TotalSize > HardDiskInf).OrderBy(di => di.TotalSize).ToArray();
            //foreach (var di in list)
            //    Console.WriteLine(Path.Combine(di.RootDirectory.FullName, "视酷下载"));
            if (list.Length == 0)
                throw new InvalidOperationException("未找到合适的硬盘");
            else if (list.Length == 1)
                return Path.Combine(list.First().RootDirectory.FullName, "视酷下载");
            if (d[0] != list.First())
                return Path.Combine(list[0].RootDirectory.FullName, "视酷下载");
            else
                return Path.Combine(list[1].RootDirectory.FullName, "视酷下载");
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
