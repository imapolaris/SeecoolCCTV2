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
    public class UserInfoPersistence : BasePersistence<CCTVUserInfo>
    {
        public static UserInfoPersistence Instance { get; private set; }
        static UserInfoPersistence()
        {
            Instance = new UserInfoPersistence();
        }

        private UserInfoPersistence() : base("CCTVUserInfo")
        {
        }
    }
}
