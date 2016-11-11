using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class HikM4StreamEventArgs:EventArgs
    {
        public HikM4Package Package { get; private set; }

        public HikM4StreamEventArgs(HikM4Package package)
        {
            this.Package = package;
        }
    }
}
