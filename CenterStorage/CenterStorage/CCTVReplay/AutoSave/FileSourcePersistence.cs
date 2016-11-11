using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Interface;
using CCTVReplay.Source;
using CCTVReplay.Util;
using Newtonsoft.Json;

namespace CCTVReplay.AutoSave
{
    public class FileSourcePersistence : ISourcePersistence
    {
        private static readonly string FilePath = Path.Combine(ConstSettings.ConfigPath, "DataSourcList.json");
        private List<DataSource> _srcList;
        public FileSourcePersistence()
        {
            readSrcList();
        }
        private void readSrcList()
        {
            _srcList = new List<DataSource>();
            try
            {
                if (File.Exists(FilePath))
                {
                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string str = sr.ReadToEnd();
                            _srcList = JsonConvert.DeserializeObject<List<DataSource>>(str);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取数据源配置文件失败:{ex.Message}");
            }
        }

        private void updateFile()
        {
            try
            {
                FileInfo fi = new FileInfo(FilePath);
                if (!fi.Directory.Exists)
                    fi.Directory.Create();
                using (FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.SetLength(0);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string str = JsonConvert.SerializeObject(_srcList);
                        sw.Write(str);
                    }
                }
                readSrcList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入数据源配置文件失败:{ex.Message}");
            }
        }

        public int Count()
        {
            return _srcList.Count;
        }

        public bool Delete(string siName)
        {
            int index = _srcList.FindIndex(ds => ds.SourceName == siName);
            if (index >=0)
            {
                _srcList.RemoveAt(index);
                updateFile();
                return true;
            }
            return false;
        }

        public bool Insert(DataSource si)
        {
            DataSource old = Select(si.SourceName);
            if (old != null)
            {
                return false;
            }
            else
            {
                _srcList.Add(si);
                updateFile();
                return true;
            }

        }

        public DataSource Select(string siName)
        {
            return _srcList.FirstOrDefault(ds => ds.SourceName == siName);
        }

        public IEnumerable<DataSource> SelectAll()
        {
            return _srcList.ToArray();
        }

        public bool Update(string oldName, DataSource newSI)
        {
            DataSource old = Select(oldName);
            if (old == null)
            {
                return false;
            }
            else
            {
                old.LocalSourcePath = newSI.LocalSourcePath;
                old.Username = newSI.Username;
                old.Password = newSI.Password;
                old.RemoteSourceIp = newSI.RemoteSourceIp;
                old.SourceName = newSI.SourceName;
                updateFile();
                return true;
            }
        }
    }
}
