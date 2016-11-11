using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    public enum StatusEvent
    {
        /// <summary>
        /// 上线
        /// </summary>
        ON,
        /// <summary>
        /// 离线
        /// </summary>
        OFF,
        /// <summary>
        /// 视频丢失
        /// </summary>
        VLOST,
        /// <summary>
        /// 故障
        /// </summary>
        DEFECT,
        /// <summary>
        /// 增加
        /// </summary>
        ADD,
        /// <summary>
        /// 删除
        /// </summary>
        DEL,
        /// <summary>
        /// 更新
        /// </summary>
        UPDATE
    }
}
