using AopUtil.WpfBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.Model;

namespace VideoNS.SubWindow
{
    internal class GridBlockModel:SplitScreenNode
    {
        public GridBlockModel():this(true)
        {

        }

        public GridBlockModel(bool isDefault)
        {
            this.IsDefault = isDefault;
        }

        [AutoNotify]
        [JsonIgnore]
        public bool IsDefault { get; private set; }
    }
}
