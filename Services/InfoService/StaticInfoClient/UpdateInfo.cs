using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticInfoClient
{
    public class ObjectItem<T>
    {
        public string Key;
        public bool IsDeleted;
        public T Item;
    }

    public class InfoItem
    {
        public string Key;
        public bool IsDeleted;
        public string Info;
    }

    public class UpdateInfo
    {
        public string Section;
        public bool IsWhole;
        public long Version;
        public IEnumerable<InfoItem> Items;
    }
}
