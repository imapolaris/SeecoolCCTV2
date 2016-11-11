using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoFlashback
{
    internal class Record<T>
    {
        public DateTime Time;
        public bool IsKey;
        public T Package;
        public byte[] Header;
    }
}
