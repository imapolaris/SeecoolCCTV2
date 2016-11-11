using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay
{
    public class ErrorMessageException: Exception
    {
        public ErrorMessageException()
            :base()
        { }

        public ErrorMessageException(string message):
            base(message)
        {
        }
    }
}
