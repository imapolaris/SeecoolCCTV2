using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GBTModels.Util;

namespace GBTModels
{
    public abstract class AbstractGBTCommand : IGBTCommand
    {
        public abstract string CmdType { get; set; }
        private int _sn = -1;
        public int SN
        {
            get
            {
                if (_sn < 0)
                    _sn = SNGenner.Instance.CreateSN();
                return _sn;
            }
            set { _sn = value; }
        }
    }
}
