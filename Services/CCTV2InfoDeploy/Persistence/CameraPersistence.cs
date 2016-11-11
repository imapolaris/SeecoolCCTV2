using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class CameraPersistence : BasePersistence<CCTVCameraLimits>
    {
        public static CameraPersistence Instance { get; private set; }
        static CameraPersistence()
        {
            Instance = new CameraPersistence();
        }

        private CameraPersistence() : base("CCTVCameraLimits")
        {
        }
    }
}
