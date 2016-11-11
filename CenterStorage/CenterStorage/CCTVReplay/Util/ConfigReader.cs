using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Util
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
        public string CenterStorageHost { get; private set; }
        public int CenterStoragePort { get; private set; }
        public int WebApiPort { get; private set; }

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

            port = -1;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CenterStoragePort"], out port);
            CenterStoragePort = port;
            CenterStorageHost = System.Configuration.ConfigurationManager.AppSettings["CenterStorageHost"];

            port = -1;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["WebApiPort"], out port);
            WebApiPort = port;

        }
    }
}
