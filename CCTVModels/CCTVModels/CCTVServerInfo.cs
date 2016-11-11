using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVServerInfo
    {
        public string ServerId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// InfoService地址。
        /// </summary>
        public string InfoServiceIp { get; set; }
        /// <summary>
        /// InfoService端口。
        /// </summary>
        public int InfoServicePort { get; set; } = 27010;
        /// <summary>
        /// 流媒体服务IP地址。
        /// </summary>
        public string StreamServerIp { get; set; }
        /// <summary>
        /// 流媒体服务端口。
        /// </summary>
        public int StreamServerPort { get; set; } = 37010;
        /// <summary>
        /// 控制服务IP地址。
        /// </summary>
        public string ControlServerIp { get; set; }
        /// <summary>
        /// 控制服务端口。
        /// </summary>
        public int ControlServerPort { get; set; } = 47010;
    }
}
