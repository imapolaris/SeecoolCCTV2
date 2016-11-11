using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels
{
    public static class GBTCommandTypes
    {
        /// <summary>
        /// 状态报送信息。
        /// </summary>
        public const string KeepAlive = "Keepalive";
        /// <summary>
        /// 设备目录查询
        /// </summary>
        public const string CataLog = "CataLog";
        /// <summary>
        /// 设备状态查询
        /// </summary>
        public const string DeviceStatus = "DeviceStatus";
        /// <summary>
        /// 设备信息查询。
        /// </summary>
        public const string DeviceInfo = "DeviceInfo";

        public static bool IsCommand(string cmdType,string cmdStr)
        {
            return cmdType.Equals(cmdStr, StringComparison.OrdinalIgnoreCase);
        }
    }
}
