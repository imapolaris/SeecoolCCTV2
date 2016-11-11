using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.Util
{
    public class Configurations
    {
        public static Configurations Instance { get; private set; }
        static Configurations()
        {
            Instance = new Configurations();
        }

        private Configurations()
        {
            init();
        }

        private void init()
        {
            Encoding = System.Configuration.ConfigurationManager.AppSettings["Encoding"];
            InfoServiceAddress = System.Configuration.ConfigurationManager.AppSettings["InfoServiceAddress"];
            GatewayPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["GatewayPort"]);
        }

        public string Encoding { get; private set; }
        public string InfoServiceAddress { get; private set; }
        public int GatewayPort { get; private set; }
    }
}
