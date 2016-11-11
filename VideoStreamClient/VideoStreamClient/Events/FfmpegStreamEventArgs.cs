using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class FfmpegStreamEventArgs:EventArgs
    {
        public int CodecID { get { return (int)Package.CodecID; } }
        public FfmpegPackage Package { get; private set; }

        public FfmpegStreamEventArgs(FfmpegPackage package)
        {
            this.Package = package;
        }
    }
}
