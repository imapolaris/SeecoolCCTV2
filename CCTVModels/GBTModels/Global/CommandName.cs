using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    public enum CommandName
    {
        /// <summary>
        /// 状态信息报送。
        /// </summary>
        KeepAlive,
        /// <summary>
        /// 设备控制。
        /// </summary>
        DeviceControl,
        /// <summary>
        /// 设备目录查询。
        /// </summary>
        Catalog,
        /// <summary>
        /// 设备状态查询。
        /// </summary>
        DeviceStatus,
        /// <summary>
        /// 设备信息查询。
        /// </summary>
        DeviceInfo,
        /// <summary>
        /// 文件目录检索。
        /// </summary>
        RecordInfo,
        /// <summary>
        /// 报警。
        /// </summary>
        Alarm,
        /// <summary>
        /// 未知的。
        /// </summary>
        Unknown
    }
}
