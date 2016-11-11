using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;

namespace GBTModels.Response
{
    public class DeviceStatusResp : AbstractDeviceObject
    {
        public override string CmdType
        {
            get
            {
                return GBTCommandTypes.DeviceStatus;
            }
            set
            {
            }
        }

        /// <summary>
        /// 查询结果标志（必选）
        /// </summary>
        public ResultType Result { get; set; }
        /// <summary>
        /// 是否在线（必选）
        /// </summary>
        public OnlineType Online { get; set; }
        /// <summary>
        /// 是否正常工作（必选）
        /// </summary>
        public ResultType Status { get; set; }
        /// <summary>
        /// 不正常工作原因（可选）
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 是否编码（可选） 
        /// </summary>
        public StatusType Encode { get; set; }
        /// <summary>
        /// 设备时间和日期（可选）
        /// </summary>
        public DateTime DeviceTime { get; set; }
        /// <summary>
        /// 报警设备状态列表， num 表示列表项个数（可选）
        /// </summary>
        [XmlElement("Alarmstatus")]
        public AlarmItemsCollection Alarms { get; set; }
    }

    public enum DutyStatus
    {
        ONDUTY,
        OFFDUTY,
        ALARM
    }

    public class AlarmItem
    {
        public string DeviceID { get; set; }
        public DutyStatus DutyStatus { get; set; }
    }

    public class AlarmItemsCollection
    {
        public AlarmItemsCollection()
        {
            Items = new List<AlarmItem>();
        }

        [XmlAttribute("Num")]
        public int Count
        {
            get { return Items.Count; }
            set { }
        }

        [XmlElement("Item")]
        public List<AlarmItem> Items { get; set; }

        public void Add(AlarmItem item)
        {
            Items.Add(item);
        }

        public void Remove(AlarmItem item)
        {
            Items.Remove(item);
        }
    }
}
