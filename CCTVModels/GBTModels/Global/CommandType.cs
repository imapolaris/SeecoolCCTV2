using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    public enum CommandType
    {
        /// <summary>
        /// 表示一个控制的动作
        /// </summary>
        Control,
        /// <summary>
        /// 表示一个查询的动作
        /// </summary>
        Query,
        /// <summary>
        /// 表示一个通知的动作
        /// </summary>
        Notify,
        /// <summary>
        /// 表示一个请求动作的应答
        /// </summary>
        Response,
        /// <summary>
        /// 未知的。
        /// </summary>
        Unknown
    }
}
