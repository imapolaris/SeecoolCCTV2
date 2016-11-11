using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Source
{
    public class DataSource
    {
        public static readonly DataSource Empty = new DataSource()
        {
            SourceType = SourceType.Remote
        };

        public SourceType SourceType { get; set; }
        public string SourceName { get; set; }
        public string LocalSourcePath { get; set; }
        public string RemoteSourceIp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static DataSource DeepClone(DataSource si)
        {
            DataSource rst = new DataSource();
            rst.SourceType = si.SourceType;
            rst.SourceName = si.SourceName;
            rst.LocalSourcePath = si.LocalSourcePath;
            rst.RemoteSourceIp = si.RemoteSourceIp;
            rst.Username = si.Username;
            rst.Password = si.Password;
            return rst;
        }
    }
}
