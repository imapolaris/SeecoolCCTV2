using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    /// <summary>
    /// 文件目录项类型
    /// </summary>
    public class ItemFileType
    {
        /// <summary>
        /// 设备/区域编码（必选）
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// 设备/区域名称（必选）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件路径名（ 可选）
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 录像地址（ 可选）
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 录像开始时间（ 可选）
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 录像结束时间（ 可选） 
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 保密属性（必选）缺省为 0； 0：不涉密， 1：涉密
        /// </summary>
        public int Secrecy { get; set; }
        /// <summary>
        ///  录像产生类型（可选） time 或 alarm 或 manual 或 all 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 录像触发者 ID（可选）
        /// </summary>
        public string RecorderID { get; set; }
    }
}
