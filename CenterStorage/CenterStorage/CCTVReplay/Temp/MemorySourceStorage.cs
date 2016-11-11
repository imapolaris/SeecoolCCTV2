using CCTVReplay.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Source;

namespace CCTVReplay.Temp
{
    public class MemorySourceStorage : ISourcePersistence
    {
        private List<DataSource> _srcInfos;

        public MemorySourceStorage()
        {
            _srcInfos = new List<DataSource>();
            init();
        }

        private void init()
        {
            for (int i = 0; i < 5; i++)
            {
                DataSource si = new DataSource();
                si.SourceName = i % 3 == 0 ? "本地数据源" + i : "远程数据源" + i;
                si.SourceType = i % 3 == 0 ? SourceType.Local : SourceType.Remote;
                si.LocalSourcePath = "D:/视频文件/Test";
                si.RemoteSourceIp = "127.0.0.1";
                if (i == 4)
                    si.RemoteSourceIp = "192.168.9.222";
                si.Username = "admin";
                si.Password = "123456789";
                _srcInfos.Add(si);
            }
        }

        public int Count()
        {
            return _srcInfos.Count;
        }

        public bool Delete(string siName)
        {
            int index = _srcInfos.FindIndex(si => si.SourceName == siName);
            if (index >= 0)
            {
                _srcInfos.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool Insert(DataSource si)
        {
            if (si != null && si.SourceName != null)
            {
                _srcInfos.Add(si);
                return true;
            }
            return false;
        }

        public IEnumerable<DataSource> SelectAll()
        {
            return _srcInfos.ToArray();
        }

        public DataSource Select(string siName)
        {
            return _srcInfos.Find(si => si.SourceName == siName);
        }

        public bool Update(string oldName, DataSource newSI)
        {
            DataSource item = _srcInfos.Find(si => si.SourceName == oldName);
            if (item != null)
            {
                item.SourceName = newSI.SourceName;
                item.LocalSourcePath = newSI.LocalSourcePath;
                item.Password = newSI.Password;
                item.RemoteSourceIp = newSI.RemoteSourceIp;
                item.Username = newSI.Username;
                return true;
            }
            return false;
        }
    }
}
