using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class GlobalPersistence : BasePersistence<CCTVGlobalInfo>
    {
        public static GlobalPersistence Instance { get; private set; }
        static GlobalPersistence()
        {
            Instance = new GlobalPersistence();
        }

        private GlobalPersistence() : base("CCTVGlobal")
        {

        }
    }
}
