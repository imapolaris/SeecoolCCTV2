using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVInfoHub.Entity
{
    public enum CCTVInfoType
    {
        /// <summary>
        /// 目标跟踪
        /// </summary>
        TargetTrackInfo,
        /// <summary>
        /// 视频分析
        /// </summary>
        VideoAnalyzeInfo,
        /// <summary>
        /// 视频跟踪
        /// </summary>
        VideoTrackInfo,
        /// <summary>
        /// 摄像机角度方位限制。
        /// </summary>
        CameraLimitsInfo,
        /// <summary>
        /// 动态信息
        /// </summary>
        DynamicInfo,
        /// <summary>
        /// 全局信息
        /// </summary>
        GlobalInfo,
        /// <summary>
        /// 逻辑树节点信息
        /// </summary>
        HierarchyInfo,
        /// <summary>
        /// 逻辑树
        /// </summary>
        LogicalTree,
        /// <summary>
        /// 视频在线状态
        /// </summary>
        OnlineStatus,
        /// <summary>
        /// 节点服务器信息
        /// </summary>
        ServerInfo,
        /// <summary>
        /// 视频静态信息
        /// </summary>
        StaticInfo,
        /// <summary>
        /// 云镜控制配置信息。
        /// </summary>
        ControlConfig,
        /// <summary>
        /// 视频设备信息。
        /// </summary>
        DeviceInfo,
        /// <summary>
        /// 用户信息。
        /// </summary>
        UserInfo,
        /// <summary>
        /// 配置权限信息
        /// </summary>
        Privilege,
        /// <summary>
        /// 用户权限。
        /// </summary>
        UserPrivilege
    }
}
