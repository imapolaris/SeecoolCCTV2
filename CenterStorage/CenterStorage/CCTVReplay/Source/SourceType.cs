using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Source
{
    public enum SourceType
    {
        Local,
        Remote
    }

    public class SourceTypeView
    {
        public static readonly SourceTypeView Local = new SourceTypeView(SourceType.Local, "本地数据");
        public static readonly SourceTypeView Remote = new SourceTypeView(SourceType.Remote, "远程数据");

        private SourceTypeView(SourceType st,string stName)
        {
            SourceType = st;
            SourceTypeName = stName;
        }
        public SourceType SourceType { get; private set; }
        public string SourceTypeName { get; private set; }
    }
}
