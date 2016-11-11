using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenterStorageCmd;

namespace CCTVDownload.Util
{
    public class ConfigReader
    {
        public static ConfigReader Instance { get; private set; }
        static ConfigReader()
        {
            Instance = new ConfigReader();
        }

        public string DownloadHost { get; private set; }
        public int DownloadPort { get; private set; }
        public string CenterStorageHost { get; private set; } = "127.0.0.1";
        public int CenterStoragePort { get; private set; } = 50101;

        private ConfigReader()
        {
            readConfig();
        }

        private void readConfig()
        {
            int port = -1;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["DownloadPort"], out port);
            DownloadPort = port;
            DownloadHost = System.Configuration.ConfigurationManager.AppSettings["DownloadHost"];

            port = 50101;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CenterStoragePort"], out port);
            if(port > 0)
                CenterStoragePort = port;
            string centerStorageHost= System.Configuration.ConfigurationManager.AppSettings["CenterStorageHost"];
            if (!string.IsNullOrWhiteSpace(centerStorageHost))
                CenterStorageHost = centerStorageHost;
        }
    }
}
