using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class ControlPersistence:BasePersistence<CCTVControlConfig>
    {
        public static ControlPersistence Instance { get; private set; }
        static ControlPersistence()
        {
            Instance = new ControlPersistence();
        }

        private ControlPersistence() : base("CCTVControl")
        {
        }
    }
}
