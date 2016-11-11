using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Source
{
    public class VideoDataSource
    {
        public SourceType SrcType { get; set; }
        public StorageSource Storage { get; set; }
        public string LocalSourcePath { get; set; }

    }
}
