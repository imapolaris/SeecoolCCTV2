using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public class PlatformStatus
    {
        public PlatformStatus()
        {

        }

        public PlatformStatus(Platform plat)
        {
            Id = plat.Id;
            Name = plat.Name;
            SipNumber = plat.SipNumber;
            Ip = plat.Ip;
            Port = plat.Port;
            UserName = plat.UserName;
            Password = plat.Password;
            Realm = plat.Realm;
            Type = plat.Type;
            Online = false;
            RefreshTime = DateTime.Now;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SipNumber { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; }
        public PlatformType Type { get; set; }
        public bool Online { get; set; }
        public DateTime RefreshTime { get; set; }
    }
}
