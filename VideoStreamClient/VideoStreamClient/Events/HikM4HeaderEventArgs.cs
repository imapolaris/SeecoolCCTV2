using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class HikM4HeaderEventArgs : EventArgs
    {
        public HikM4Header Header { get; private set; }

        public HikM4HeaderEventArgs(HikM4Header header)
        {
            this.Header = header;
        }
    }
}
