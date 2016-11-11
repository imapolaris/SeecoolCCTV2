using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVInfoHub.Entity
{
    public class UpdateInfo
    {
        public string Section;
        public bool IsWhole;
        public long Version;
        public IEnumerable<InfoItem> Items;
    }

    public class InfoItem
    {
        public string Key;
        public bool IsDeleted;
        public string Info;
    }
}
