using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class ServerPersistence:BasePersistence<CCTVServerInfo>
    {
        public static ServerPersistence Instance { get; private set; }
        static ServerPersistence()
        {
            Instance = new ServerPersistence();
        }

        private ServerPersistence():base("CCTVServer")
        {

        }
    }
}
