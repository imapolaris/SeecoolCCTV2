using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTV2InfoDeploy.Models;
using CCTVModels.User;
using Persistence;

namespace CCTV2InfoDeploy.Persistence
{
    public class PrivilegePersistence : BasePersistence<CCTVPrivilege>
    {
        public static PrivilegePersistence Instance { get; private set; }
        static PrivilegePersistence()
        {
            Instance = new PrivilegePersistence();
        }

        private PrivilegePersistence():base("CCTVPrivilege")
        {

        }
    }
}
