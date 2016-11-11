using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class OnlinePersistence : BasePersistence<CCTVOnlineStatus>
    {
        public static OnlinePersistence Instance { get; private set; }
        static OnlinePersistence()
        {
            Instance = new OnlinePersistence();
        }

        private OnlinePersistence():base("CCTVLogicalTree")
        {

        }
    }
}
