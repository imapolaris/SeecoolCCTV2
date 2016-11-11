using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVModels.User
{
    public class CCTVUserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ChineseName { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
