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
    public class UserPrivilegePersistence : BasePersistence<CCTVUserPrivilege>
    {
        public static UserPrivilegePersistence Instance { get; private set; }
        static UserPrivilegePersistence()
        {
            Instance = new UserPrivilegePersistence();
        }

        private UserPrivilegePersistence() : base("CCTVUserPrivilege")
        {
        }
    }
}
