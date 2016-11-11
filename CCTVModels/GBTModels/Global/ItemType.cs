using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GBTModels.Global
{
    /// <summary>
    /// 设备目录项类型
    /// </summary>
    public class ItemType
    {
        /// <summary>
        /// 设备/区域/系统编码（必选）
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// 状态改变事件 ON：上线，OFF：离线，VLOST：视频丢失，DEFECT：
        /// 故障，ADD：增加，DEL：删除，UPDATE：更新（必选）
        /// </summary>
        public StatusEvent Event { get; set; }
        /// <summary>
        /// 设备/区域/系统名称（必选）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 当为设备时，设备厂商（必选）
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 当为设备时，设备型号（必选）
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 当为设备时，设备归属（必选）
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// 行政区域（必选）
        /// </summary>
        public string CivilCode { get; set; }
        /// <summary>
        /// 警区（可选）
        /// </summary>
        public string Block { get; set; }
        /// <summary>
        /// 当为设备时，安装地址（必选）
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 当为设备时，是否有子设备（必选） 1 有， 0 没有
        /// </summary>
        public int Parental { get; set; }
        /// <summary>
        /// 父设备/区域/系统 ID（可选，有父设备需要填写）
        /// </summary>
        public string ParentID { get; set; }
        /// <summary>
        /// 信令安全模式（可选）缺省为 0； 
        /// 0：不采用； 
        /// 2： S/MIME 签名方式； 
        /// 3： S/MIME 加密签名同时采用方式； 
        /// 4：数字摘要方式
        /// </summary>
        public int SafetyWay { get; set; }
        /// <summary>
        /// 注方册式（必选）缺省为 1； 
        /// 1： 符合 sip3261 标准的认证注册模式； 
        /// 2： 基于口令的双向认证注册模式； 
        /// 3： 基于数字证书的双向认证注册模式
        /// </summary>
        public int RegisterWay { get; set; }
        /// <summary>
        /// 证书序列号（ 有证书的设备必选）
        /// </summary>
        public string CertNum { get; set; }
        /// <summary>
        /// 证书有效标识（ 有证书的设备必选）缺省为 0； 证书有效标识： 
        /// 0： 无效 1：有效
        /// </summary>
        public int Certifiable { get; set; }
        /// <summary>
        /// 无效原因码（有证书且证书无效的设备必选）
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 证书终止有效期（有证书的设备必选） 
        /// </summary>
        [XmlElement(DataType = "dateTime")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 保密属性（必选）缺省为 0； 
        /// 0： 不涉密， 
        /// 1： 涉密
        /// </summary>
        public int Secrecy { get; set; }
        /// <summary>
        /// 设备/区域/系统 IP 地址（可选）
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 设备/区域/系统端口（可选）
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 设备口令（可选）
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 设备状态（必选）
        /// </summary>
        public StatusType Status { get; set; }
        /// <summary>
        /// 经度（可选）
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度（可选）
        /// </summary>
        public double Latitude { get; set; }
    }
}
