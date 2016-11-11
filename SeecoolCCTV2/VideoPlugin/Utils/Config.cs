using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.Utils
{
    public static class Config
    {
        public static bool StringToBool(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                bool b = false;
                if (bool.TryParse(str, out b))
                    return b;

                int i = 0;
                if (int.TryParse(str, out i))
                    return i != 0;
            }

            return false;
        }
    }
}
