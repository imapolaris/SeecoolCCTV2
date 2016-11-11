using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVModels.User
{
    public class CCTVUserPrivilege
    {
        public string UserName { get; set; }
        public string[] AccessibleNodes { get; set; }
        public CCTVPrivilege[] Privileges { get; set; }
    }
}
