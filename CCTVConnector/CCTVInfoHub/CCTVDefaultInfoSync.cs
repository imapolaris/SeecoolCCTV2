using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub.Entity;
using CCTVInfoHub.Util;
using CCTVModels;
using CCTVModels.User;
using StaticInfoClient;

namespace CCTVInfoHub
{
    public class CCTVDefaultInfoSync : CCTVInfoSync
    {
        private Dictionary<CCTVInfoType, object> _syncParams;

        public CCTVDefaultInfoSync(string baseAddress) : base(baseAddress)
        {
            _syncParams = new Dictionary<CCTVInfoType, object>();
        }

        #region 【附加刷新回掉】
        public void AddUpdateHandler(CCTVInfoType type, SyncUpdateHandler updateHandler)
        {
            if (_syncParams.ContainsKey(type) && updateHandler != null)
            {
                IUpdate iu = _syncParams[type] as IUpdate;
                iu.UpdateHandler += updateHandler;
            }
        }

        public void RemoveUpdateHandler(CCTVInfoType type, SyncUpdateHandler updateHandler)
        {
            if (_syncParams.ContainsKey(type) && updateHandler != null)
            {
                IUpdate iu = _syncParams[type] as IUpdate;
                iu.UpdateHandler -= updateHandler;
            }
        }
        #endregion 【附加刷新回掉】

        #region 【注册默认信息刷新服务】
        public bool HasRegisteredDefault(CCTVInfoType type)
        {
            return _syncParams.ContainsKey(type);
        }

        public void RegisterDefaultWithoutUpdate(CCTVInfoType type)
        {
            RegisterDefault(type, TimeSpan.Zero, null, null);
        }
        public void RegisterDefault(CCTVInfoType type, TimeSpan interval)
        {
            RegisterDefault(type, interval, null, null);
        }

        public void RegisterDefault(CCTVInfoType type, TimeSpan interval, SyncUpdateHandler updateHandler)
        {
            RegisterDefault(type, interval, updateHandler, null);
        }

        public void RegisterDefault(CCTVInfoType type, TimeSpan interval, SyncUpdateHandler updateHandler, string savePath)
        {
            if (_syncParams.ContainsKey(type))
            {
                IDisposable dis = _syncParams[type] as IDisposable;
                dis.Dispose();
            }
            switch (type)
            {
                case CCTVInfoType.TargetTrackInfo:
                    {
                        SyncParams<CCTVTargetTrack> sp = new SyncParams<CCTVTargetTrack>(DefaultSections.TargetTrackInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVTargetTrack> sync = new StaticInfoSynchronizer<CCTVTargetTrack>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVTargetTrack>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.VideoAnalyzeInfo:
                    {
                        SyncParams<CCTVVideoAnalyze> sp = new SyncParams<CCTVVideoAnalyze>(DefaultSections.VideoAnalyzeInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVVideoAnalyze> sync = new StaticInfoSynchronizer<CCTVVideoAnalyze>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVVideoAnalyze>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.VideoTrackInfo:
                    {
                        SyncParams<CCTVVideoTrack> sp = new SyncParams<CCTVVideoTrack>(DefaultSections.VideoTrackInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVVideoTrack> sync = new StaticInfoSynchronizer<CCTVVideoTrack>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVVideoTrack>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.CameraLimitsInfo:
                    {
                        SyncParams<CCTVCameraLimits> sp = new SyncParams<CCTVCameraLimits>(DefaultSections.CameraLimitsInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVCameraLimits> sync = new StaticInfoSynchronizer<CCTVCameraLimits>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVCameraLimits>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.DynamicInfo:
                    {
                        SyncParams<CCTVDynamicInfo> sp = new SyncParams<CCTVDynamicInfo>(DefaultSections.DynamicInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVDynamicInfo> sync = new StaticInfoSynchronizer<CCTVDynamicInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVDynamicInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.GlobalInfo:
                    {
                        SyncParams<CCTVGlobalInfo> sp = new SyncParams<CCTVGlobalInfo>(DefaultSections.GlobalInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVGlobalInfo> sync = new StaticInfoSynchronizer<CCTVGlobalInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVGlobalInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.HierarchyInfo:
                    {
                        SyncParams<CCTVHierarchyInfo> sp = new SyncParams<CCTVHierarchyInfo>(HierarchyInfoUtil.CreateSection("Default"), interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVHierarchyInfo> sync = new StaticInfoSynchronizer<CCTVHierarchyInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVHierarchyInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.LogicalTree:
                    {
                        SyncParams<CCTVLogicalTree> sp = new SyncParams<CCTVLogicalTree>(DefaultSections.LogicalTree, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVLogicalTree> sync = new StaticInfoSynchronizer<CCTVLogicalTree>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVLogicalTree>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.OnlineStatus:
                    {
                        SyncParams<CCTVOnlineStatus> sp = new SyncParams<CCTVOnlineStatus>(DefaultSections.OnlineStatus, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVOnlineStatus> sync = new StaticInfoSynchronizer<CCTVOnlineStatus>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVOnlineStatus>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.ServerInfo:
                    {
                        SyncParams<CCTVServerInfo> sp = new SyncParams<CCTVServerInfo>(DefaultSections.ServerInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVServerInfo> sync = new StaticInfoSynchronizer<CCTVServerInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVServerInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.StaticInfo:
                    {
                        SyncParams<CCTVStaticInfo> sp = new SyncParams<CCTVStaticInfo>(DefaultSections.StaticInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVStaticInfo> sync = new StaticInfoSynchronizer<CCTVStaticInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVStaticInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.ControlConfig:
                    {
                        SyncParams<CCTVControlConfig> sp = new SyncParams<CCTVControlConfig>(DefaultSections.ControlConfig, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVControlConfig> sync = new StaticInfoSynchronizer<CCTVControlConfig>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVControlConfig>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.DeviceInfo:
                    {
                        SyncParams<CCTVDeviceInfo> sp = new SyncParams<CCTVDeviceInfo>(DefaultSections.DeviceInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVDeviceInfo> sync = new StaticInfoSynchronizer<CCTVDeviceInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVDeviceInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.UserInfo:
                    {
                        SyncParams<CCTVUserInfo> sp = new SyncParams<CCTVUserInfo>(DefaultSections.UserInfo, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVUserInfo> sync = new StaticInfoSynchronizer<CCTVUserInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVUserInfo>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.Privilege:
                    {
                        SyncParams<CCTVPrivilege> sp = new SyncParams<CCTVPrivilege>(DefaultSections.Privilege, interval, updateHandler, savePath);
                        StaticInfoSynchronizer<CCTVPrivilege> sync = new StaticInfoSynchronizer<CCTVPrivilege>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                        _syncParams[type] = new ParamHolder<CCTVPrivilege>()
                        {
                            Sync = sync,
                            Param = sp
                        };
                    }
                    break;
                case CCTVInfoType.UserPrivilege:
                    {
                        {
                            SyncParams<CCTVUserPrivilege> sp = new SyncParams<CCTVUserPrivilege>(DefaultSections.UserPrivilege, interval, updateHandler, savePath);
                            StaticInfoSynchronizer<CCTVUserPrivilege> sync = new StaticInfoSynchronizer<CCTVUserPrivilege>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
                            _syncParams[type] = new ParamHolder<CCTVUserPrivilege>()
                            {
                                Sync = sync,
                                Param = sp
                            };
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void RegisterHierarchy(string logicalTreeName, TimeSpan interval, SyncUpdateHandler updateHandler)
        {
            RegisterHierarchy(logicalTreeName, interval, updateHandler, null);
        }

        public void RegisterHierarchy(string logicalTreeName, TimeSpan interval, SyncUpdateHandler updateHandler, string savePath)
        {
            if (_syncParams.ContainsKey(CCTVInfoType.HierarchyInfo))
            {
                IDisposable dis = _syncParams[CCTVInfoType.HierarchyInfo] as IDisposable;
                dis.Dispose();
            }
            SyncParams<CCTVHierarchyInfo> sp = new SyncParams<CCTVHierarchyInfo>(HierarchyInfoUtil.CreateSection(logicalTreeName), interval, updateHandler, savePath);
            StaticInfoSynchronizer<CCTVHierarchyInfo> sync = new StaticInfoSynchronizer<CCTVHierarchyInfo>(_baseAddress, sp.Section, sp.Interval, sp.SavePath, sp.OnUpdate);
            _syncParams[CCTVInfoType.HierarchyInfo] = new ParamHolder<CCTVHierarchyInfo>()
            {
                Sync = sync,
                Param = sp
            };
        }

        public void UnregisterDefault(CCTVInfoType type)
        {
            if (_syncParams.ContainsKey(type))
            {
                IDisposable dis = _syncParams[type] as IDisposable;
                dis.Dispose();
                _syncParams.Remove(type);
            }
        }
        #endregion 【注册默认信息刷新服务】


        #region 【手动刷新】
        public void UpdateDefault(CCTVInfoType type)
        {
            if (_syncParams.ContainsKey(type))
            {
                IUpdate pd = _syncParams[type] as IUpdate;
                pd.Update();
            }
        }

        /// <summary>
        /// 手动刷新全部预置信息。
        /// </summary>
        public void UpdateAllDefault()
        {
            foreach (CCTVInfoType iType in Enum.GetValues(typeof(CCTVInfoType)))
                UpdateDefault(iType);
        }
        #endregion 【手动刷新】

        #region 【获取单项全部信息】
        private void checkUpdate(IUpdate up)
        {
            if (!up.HasUpdated())
                up.Update();
        }

        private void checkRegister(CCTVInfoType type)
        {
            if (!_syncParams.ContainsKey(type))
            {
                throw new InvalidOperationException("未注册此种类型的信息同步刷新服务:" + type);
            }
        }

        private T[] GetDefaultInfoFull<T>(CCTVInfoType type)
        {
            checkRegister(type);
            ParamHolder<T> ph = _syncParams[type] as ParamHolder<T>;
            checkUpdate(ph);
            return ph.Sync.Values.ToArray();
        }

        public CCTVTargetTrack[] GetAllTargetTrackInfo()
        {
            return GetDefaultInfoFull<CCTVTargetTrack>(CCTVInfoType.TargetTrackInfo);
        }

        public CCTVVideoAnalyze[] GetAllVideoAnalyzeInfo()
        {
            return GetDefaultInfoFull<CCTVVideoAnalyze>(CCTVInfoType.VideoAnalyzeInfo);
        }

        public CCTVVideoTrack[] GetAllVideoTrackInfo()
        {
            return GetDefaultInfoFull<CCTVVideoTrack>(CCTVInfoType.VideoTrackInfo);
        }

        public CCTVCameraLimits[] GetAllCameraLimitsInfo()
        {
            return GetDefaultInfoFull<CCTVCameraLimits>(CCTVInfoType.CameraLimitsInfo);
        }

        public CCTVDynamicInfo[] GetAllDynamicInfo()
        {
            return GetDefaultInfoFull<CCTVDynamicInfo>(CCTVInfoType.DynamicInfo);
        }

        public CCTVHierarchyNode[] GetAllHierarchyRoots()
        {
            var hInfos = GetDefaultInfoFull<CCTVHierarchyInfo>(CCTVInfoType.HierarchyInfo);
            return HierarchyInfoUtil.BuildTree(hInfos).ToArray();
        }

        public CCTVHierarchyInfo[] GetAllHierarchyInfo()
        {
            return GetDefaultInfoFull<CCTVHierarchyInfo>(CCTVInfoType.HierarchyInfo);
        }

        public CCTVLogicalTree[] GetAllLogicalTree()
        {
            return GetDefaultInfoFull<CCTVLogicalTree>(CCTVInfoType.LogicalTree);
        }

        public CCTVOnlineStatus[] GetAllOnlineStatus()
        {
            return GetDefaultInfoFull<CCTVOnlineStatus>(CCTVInfoType.OnlineStatus);
        }

        public CCTVServerInfo[] GetAllServerInfo()
        {
            return GetDefaultInfoFull<CCTVServerInfo>(CCTVInfoType.ServerInfo);
        }

        public CCTVStaticInfo[] GetAllStaticInfo()
        {
            return GetDefaultInfoFull<CCTVStaticInfo>(CCTVInfoType.StaticInfo);
        }

        public CCTVControlConfig[] GetAllControlConfig()
        {
            return GetDefaultInfoFull<CCTVControlConfig>(CCTVInfoType.ControlConfig);
        }

        public CCTVDeviceInfo[] GetAllDeviceInfo()
        {
            return GetDefaultInfoFull<CCTVDeviceInfo>(CCTVInfoType.DeviceInfo);
        }

        public CCTVUserInfo[] GetAllUserInfo()
        {
            return GetDefaultInfoFull<CCTVUserInfo>(CCTVInfoType.UserInfo);
        }

        public CCTVPrivilege[] GetAllPrivilege()
        {
            return GetDefaultInfoFull<CCTVPrivilege>(CCTVInfoType.Privilege);
        }

        public CCTVUserPrivilege[] GetAllUserPrivilege()
        {
            return GetDefaultInfoFull<CCTVUserPrivilege>(CCTVInfoType.UserPrivilege);
        }
        #endregion 【获取单项全部信息】

        #region 【获取单条信息】
        private T GetDefaultInfo<T>(CCTVInfoType type, string key)
        {
            checkRegister(type);
            ParamHolder<T> ph = _syncParams[type] as ParamHolder<T>;
            checkUpdate(ph);
            T value;
            ph.Sync.TryGetValue(key, out value);
            return value;
        }

        public CCTVTargetTrack GetTargetTrackInfo(string key)
        {
            return GetDefaultInfo<CCTVTargetTrack>(CCTVInfoType.TargetTrackInfo, key);
        }

        public CCTVVideoAnalyze GetVideoAnalyzeInfo(string key)
        {
            return GetDefaultInfo<CCTVVideoAnalyze>(CCTVInfoType.VideoAnalyzeInfo, key);
        }

        public CCTVVideoTrack GetAllVideoTrackInfo(string key)
        {
            return GetDefaultInfo<CCTVVideoTrack>(CCTVInfoType.VideoTrackInfo, key);
        }

        public CCTVCameraLimits GetCameraLimitsInfo(string key)
        {
            return GetDefaultInfo<CCTVCameraLimits>(CCTVInfoType.CameraLimitsInfo, key);
        }

        public CCTVDynamicInfo GetDynamicInfo(string key)
        {
            return GetDefaultInfo<CCTVDynamicInfo>(CCTVInfoType.DynamicInfo, key);
        }

        public CCTVGlobalInfo GetGlobalInfo()
        {
            return GetDefaultInfo<CCTVGlobalInfo>(CCTVInfoType.GlobalInfo, "Default");
        }

        public CCTVLogicalTree GetLogicalTree(string key)
        {
            return GetDefaultInfo<CCTVLogicalTree>(CCTVInfoType.LogicalTree, key);
        }

        public CCTVOnlineStatus GetOnlineStatus(string key)
        {
            return GetDefaultInfo<CCTVOnlineStatus>(CCTVInfoType.OnlineStatus, key);
        }

        public CCTVServerInfo GetServerInfo(string key)
        {
            return GetDefaultInfo<CCTVServerInfo>(CCTVInfoType.ServerInfo, key);
        }

        public CCTVStaticInfo GetStaticInfo(string key)
        {
            return GetDefaultInfo<CCTVStaticInfo>(CCTVInfoType.StaticInfo, key);
        }

        public CCTVHierarchyInfo GetHierarchyInfo(string key)
        {
            return GetDefaultInfo<CCTVHierarchyInfo>(CCTVInfoType.HierarchyInfo, key);
        }

        public CCTVControlConfig GetControlConfig(string key)
        {
            return GetDefaultInfo<CCTVControlConfig>(CCTVInfoType.ControlConfig, key);
        }

        public CCTVDeviceInfo GetDeviceInfo(string key)
        {
            return GetDefaultInfo<CCTVDeviceInfo>(CCTVInfoType.DeviceInfo, key);
        }

        public CCTVUserInfo GetUserInfo(string userName)
        {
            return GetDefaultInfo<CCTVUserInfo>(CCTVInfoType.UserInfo, userName);
        }

        public CCTVPrivilege GetPrivilege(string name)
        {
            return GetDefaultInfo<CCTVPrivilege>(CCTVInfoType.Privilege, name);
        }

        public CCTVUserPrivilege GetUserPrivilege(string userName)
        {
            return GetDefaultInfo<CCTVUserPrivilege>(CCTVInfoType.UserPrivilege, userName);
        }
        #endregion 【获取单条信息】

        #region 【更新单条信息】
        private void PutDefaultInfo<T>(CCTVInfoType type, string key, T info, bool isDeleted)
        {
            checkRegister(type);
            ParamHolder<T> ph = _syncParams[type] as ParamHolder<T>;
            ph.Sync.PutUpdate(new List<ObjectItem<T>>()
            {
                new ObjectItem<T>() {
                    Key=key,
                    IsDeleted=isDeleted,
                    Item=info
                }
            });
        }

        public void PutTargetTrackInfo(CCTVTargetTrack info, bool isDeleted)
        {
            PutDefaultInfo<CCTVTargetTrack>(CCTVInfoType.TargetTrackInfo, info.VideoId, info, isDeleted);
        }

        public void PutVideoAnalyzeInfo(CCTVVideoAnalyze info, bool isDeleted)
        {
            PutDefaultInfo<CCTVVideoAnalyze>(CCTVInfoType.VideoAnalyzeInfo, info.VideoId, info, isDeleted);
        }

        public void PutAllVideoTrackInfo(CCTVVideoTrack info, bool isDeleted)
        {
            PutDefaultInfo<CCTVVideoTrack>(CCTVInfoType.VideoTrackInfo, info.VideoId, info, isDeleted);
        }

        public void PutCameraLimitsInfo(CCTVCameraLimits info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.CameraLimitsInfo, info.VideoId, info, isDeleted);
        }

        public void PutDynamicInfo(CCTVDynamicInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.DynamicInfo, info.VideoId, info, isDeleted);
        }

        public void PutGlobalInfo(CCTVGlobalInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.GlobalInfo, "Default", info, isDeleted);
        }

        public void PutLogicalTree(CCTVLogicalTree info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.LogicalTree, info.LogicalName, info, isDeleted);
        }

        public void PutOnlineStatus(CCTVOnlineStatus info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.OnlineStatus, info.VideoId, info, isDeleted);
        }

        public void PutServerInfo(CCTVServerInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.ServerInfo, info.ServerId, info, isDeleted);
        }

        public void PutStaticInfo(CCTVStaticInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.StaticInfo, info.VideoId, info, isDeleted);
        }

        public void PutControlConfig(CCTVControlConfig info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.ControlConfig, info.VideoId, info, isDeleted);
        }

        public void PutDeviceInfo(CCTVDeviceInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.DeviceInfo, info.VideoId, info, isDeleted);
        }

        public void PutUserInfo(CCTVUserInfo info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.UserInfo, info.UserName, info, isDeleted);
        }

        public void PutPrivilege(CCTVPrivilege info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.Privilege, info.Name, info, isDeleted);
        }

        public void PutUserPrivilege(CCTVUserPrivilege info, bool isDeleted)
        {
            PutDefaultInfo(CCTVInfoType.UserPrivilege, info.UserName, info, isDeleted);
        }
        #endregion 【更新单条信息】
        //public string GetVideoReadableName(string videoId)
        //{
        //    HierarchyNode hierarchy = GetHierarchyRoot();
        //    if (hierarchy != null)
        //    {
        //        string name = getVideoReadableName(hierarchy, videoId);
        //        if (name != null)
        //            return name;
        //    }
        //    return null;
        //}

        //private string getVideoReadableName(HierarchyNode node, string videoId)
        //{
        //    HierarchyNode video = node.Children.FirstOrDefault(x => x.Id == videoId);
        //    if (video == null)
        //    {
        //        foreach (HierarchyNode child in node.Children)
        //        {
        //            string name = getVideoReadableName(child, videoId);
        //            if (name != null)
        //                return name;
        //        }
        //    }
        //    else
        //        return $"{node.Name} - {video.Name}";

        //    return null;
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!Disposed)
            {
                if (disposing)
                {
                    foreach (CCTVInfoType key in _syncParams.Keys)
                    {
                        IDisposable dis = _syncParams[key] as IDisposable;
                        if (dis != null)
                            dis.Dispose();
                    }
                }
                _syncParams.Clear();
                _syncParams = null;
            }
        }
    }
}

