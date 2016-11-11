using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class DownloadExpandPart
    {
        public Guid GuidCode { get; private set; }
        public DownloadCode Code { get; private set; }
        public object Value { get; private set; }
        public DownloadExpandPart(Guid guid, DownloadCode code, object obj)
        {
            GuidCode = guid;
            Code = code;
            Value = obj;
        }
    }
}
