using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public static class TimeCmd
    {
        public static DateTime GTMToTime(ulong gtm)
        {
            return new DateTime(1970, 1, 1, 8, 0, 0).AddMilliseconds(gtm);
        }
    }
}
