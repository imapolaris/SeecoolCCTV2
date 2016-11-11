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
    public class StaticPersistence : BasePersistence<CCTVStaticInfo>
    {
        public static StaticPersistence Instance { get;private set; }
        static StaticPersistence()
        {
            Instance = new StaticPersistence();
        }

        private StaticPersistence() : base("CCTVStatic")
        {
        }
    }
}
