using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class UniviewStreamEventArgs:EventArgs
    {
        public UniviewPackage Packet{ get; private set; }

        public UniviewStreamEventArgs(UniviewPackage pkt)
        {
            Packet = pkt;
        }
    }
}
