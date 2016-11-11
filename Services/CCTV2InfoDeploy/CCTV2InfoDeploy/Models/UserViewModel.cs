using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels.User;

namespace CCTV2InfoDeploy.Models
{
    public class UserViewModel
    {
        public CCTVUserInfo User { get; set; }
        public CCTVUserPrivilege Privilege { get; set; }
    }
}
