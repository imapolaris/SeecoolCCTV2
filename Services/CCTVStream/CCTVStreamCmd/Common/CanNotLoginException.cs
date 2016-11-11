using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVStreamCmd
{
    public class CanNotLoginException : Exception
    {
        public CanNotLoginException()
            : base("登陆失败。")
        {
        }

        public CanNotLoginException(string message)
            : base(message)
        {
        }
    }
}
