using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVLogicalTree
    {
        public const string DefaultName = "Default";

        public string LogicalName { get; set; }
        public string DisplayName { get; set; }

        public static CCTVLogicalTree GetDefault()
        {
            return new CCTVLogicalTree()
            {
                LogicalName = DefaultName,
                DisplayName = "默认节点树"
            };
        }
    }
}
