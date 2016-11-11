using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public enum PlatformType
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 下级平台
        /// </summary>
        Lower = 1,
        /// <summary>
        /// 上级平台
        /// </summary>
        Superior = 2
    }
}
