using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;
using GBTModels.Query;

namespace GBTModels.Response
{
    public class DeviceInfoResp : AbstractDeviceObject
    {
        public override string CmdType
        {
            get
            {
                return GBTCommandTypes.DeviceInfo;
            }
            set
            {
            }
        }

        /// <summary>
        /// 查询结果（必选）
        /// </summary>
        public ResultType Reuslt{ get; set; }
        /// <summary>
        /// 设备生产商（可选）
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 设备型号（可选）
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        ///  设备固件版本（可选）
        /// </summary>
        public string Firmware { get; set; }
        /// <summary>
        /// 视频输入通道数（可选）
        /// </summary>
        public int Channel { get; set; }
    }
}
