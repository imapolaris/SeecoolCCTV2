using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class DynamicPersistence : BasePersistence<CCTVDynamicInfo>
    {
        public static DynamicPersistence Instance { get; private set; }
        static DynamicPersistence()
        {
            Instance = new DynamicPersistence();
        }

        private DynamicPersistence() : base("CCTVDynamic")
        {
        }
    }
}
