using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    public abstract class AbstractDeviceObject : AbstractGBTCommand
    {
        public string DeviceID { get; set; }
    }
}
