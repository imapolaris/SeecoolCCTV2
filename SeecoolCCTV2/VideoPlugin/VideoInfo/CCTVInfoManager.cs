using CCTVClient;
using Common.Configuration;
using Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.VideoInfo;
using CCTVModels;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using UserModule;
using CCTVInfoHub.Util;
using CCTVCameraControl.RemoteControl;

namespace VideoNS
{
    public class CCTVInfoManager
    {
        public readonly static CCTVInfoManager Instance = new CCTVInfoManager();

        public void Init()
        {
        }

        private CCTVInfoManager()
        {
            try
            {
                _webApiBaseUri = ConfigHandler.GetValue<VideoInfoPlugin>("WebApiBaseUri");
                _fromConfig = true;
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error(ex);
            }

            init();

            RemoteCalls.Global.RegisterFunc<CCTVGlobalInfo>("CCTV2_VideoPlugin_GetGlobalInfo", GetGlobalInfo);
            RemoteCalls.Global.RegisterFunc<string, CCTVHierarchyNode>("CCTV2_VideoPlugin_GetHierarchy", GetHierarchy);

            RemoteCalls.Global.RegisterFunc<CCTVOnlineStatus[]>("CCTV2_VideoPlugin_GetAllOnlineStatus", GetAllOnlineStatus);
            RemoteCalls.Global.RegisterFunc<string, CCTVOnlineStatus>("CCTV2_VideoPlugin_GetOnlineStatus", GetOnlineStatus);

            RemoteCalls.Global.RegisterFunc<CCTVStaticInfo[]>("CCTV2_VideoPlugin_GetAllStaticInfo", GetAllStaticInfo);
            RemoteCalls.Global.RegisterFunc<string, CCTVStaticInfo>("CCTV2_VideoPlugin_GetStaticInfo", GetStaticInfo);

            RemoteCalls.Global.RegisterFunc<CCTVDynamicInfo[]>("CCTV2_VideoPlugin_GetAllDynamicInfo", GetAllDynamicInfo);
            RemoteCalls.Global.RegisterFunc<string, CCTVDynamicInfo>("CCTV2_VideoPlugin_GetDynamicInfo", GetDynamicInfo);
        }

        const string _defaultHierarchy = "Default";
        private string _currentTree = "Default";
        private CCTVHierarchyNode _rootNode;
        bool _fromConfig = false;
        public CCTVHierarchyNode GetHierarchy(string hierarchyName = _defaultHierarchy)
        {
            if (!hierarchyName.Equals(_currentTree, StringComparison.OrdinalIgnoreCase))
            {
                _rootNode = null;
                _currentTree = hierarchyName;
                ClientHub.RegisterHierarchy(hierarchyName, TimeSpan.FromSeconds(5), hierUpdated);
            }
            //目前只能获取默认节点树。
            if (_rootNode == null)
            {
                CCTVHierarchyNode[] roots = ClientHub.GetAllHierarchyRoots();
                if (roots != null && roots.Length > 0)
                {
                    if (_fromConfig)
                    {
                        if (UserManager.Instance?.CurrentUser != null
                        && !string.IsNullOrWhiteSpace(UserManager.Instance.CurrentUser.UserName))
                        {
                            ClientHub.UpdateDefault(CCTVInfoType.UserPrivilege);
                            var priv = ClientHub.GetUserPrivilege(UserManager.Instance.CurrentUser.UserName);
                            if (priv != null && priv.AccessibleNodes != null)
                            {
                                roots = HierarchyInfoUtil.FilterNodes(roots, priv.AccessibleNodes).ToArray();
                            }
                        }
                    }
                    if (roots.Length == 1)
                    {
                        _rootNode = roots[0];
                    }
                    else
                    {
                        string id = Guid.NewGuid().ToString();
                        _rootNode = new CCTVHierarchyNode()
                        {
                            Name = "根节点",
                            Id = id,
                            Type = NodeType.Server,
                            ElementId = id,
                            Children = roots
                        };
                    }
                }
            }
            return _rootNode;
            //if (rootNode == null)
            //    ClientHub.UpdateRegistered<HierarchyInfo>();
            //return rootNode;
        }

        public string GetVideoReadableName(string videoId, string hierarchyName = _defaultHierarchy)
        {
            //目前只能获取默认节点树里面的名称。
            return GetVideoReadableName(videoId);
        }

        private string GetVideoReadableName(string videoId)
        {
            CCTVHierarchyNode hierarchy = GetHierarchy(_currentTree);
            if (hierarchy != null)
            {
                string name = getVideoReadableName(hierarchy, videoId);
                if (name != null)
                    return name;
            }
            return null;
        }

        private string getVideoReadableName(CCTVHierarchyNode node, string videoId)
        {
            CCTVHierarchyNode video = node.Children.FirstOrDefault(x => x.ElementId == videoId);
            if (video == null)
            {
                foreach (CCTVHierarchyNode child in node.Children)
                {
                    string name = getVideoReadableName(child, videoId);
                    if (name != null)
                        return name;
                }
            }
            else
                return $"{node.Name} - {video.Name}";

            return null;
        }

        public CCTVStaticInfo[] GetAllStaticInfo()
        {
            return ClientHub.GetAllStaticInfo();
        }

        public CCTVOnlineStatus[] GetAllOnlineStatus()
        {
            return ClientHub.GetAllOnlineStatus();
        }

        public CCTVGlobalInfo GetGlobalInfo()
        {
            return ClientHub.GetGlobalInfo();
        }

        public CCTVStaticInfo GetStaticInfo(string videoId)
        {
            return ClientHub.GetStaticInfo(videoId);
        }

        public CCTVControlConfig GetControlConfig(string videoId)
        {
            return ClientHub.GetControlConfig(videoId);
        }

        public CCTVVideoTrack GetVideoTrack(string videoId)
        {
            return ClientHub.GetAllVideoTrackInfo(videoId);
        }

        public CCTVOnlineStatus GetOnlineStatus(string videoId)
        {
            return ClientHub.GetOnlineStatus(videoId);
        }

        public CCTVDynamicInfo[] GetAllDynamicInfo()
        {
            return ClientHub.GetAllDynamicInfo();
        }

        public CCTVDynamicInfo GetDynamicInfo(string videoId)
        {
            return ClientHub.GetDynamicInfo(videoId);
        }

        CCTVGlobalInfo _globalInfo = null;
        string _webApiBaseUri;
        public string WebApiBaseUri
        {
            get { return _webApiBaseUri; }
            set
            {
                _webApiBaseUri = value;
                _fromConfig = false;
                init();
            }
        }
        private CCTVInfo CCTV1Info { get; set; }
        public CCTVDefaultInfoSync ClientHub { get; private set; }
        public PanTiltControlManager CameraControl { get; private set; }

        private void init()
        {
            if (string.IsNullOrEmpty(WebApiBaseUri))
                return;
            Console.WriteLine(WebApiBaseUri);
            ClientHub = new CCTVDefaultInfoSync(WebApiBaseUri);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.GlobalInfo);
            ClientHub.RegisterDefault(CCTVInfoType.HierarchyInfo, TimeSpan.FromSeconds(5));
            ClientHub.RegisterDefault(CCTVInfoType.StaticInfo, TimeSpan.FromSeconds(10));
            ClientHub.RegisterDefault(CCTVInfoType.OnlineStatus, TimeSpan.FromSeconds(5));
            ClientHub.RegisterDefault(CCTVInfoType.DynamicInfo, TimeSpan.FromSeconds(5));
            ClientHub.RegisterDefault(CCTVInfoType.ControlConfig, TimeSpan.FromSeconds(15));
            ClientHub.RegisterDefault(CCTVInfoType.VideoTrackInfo, TimeSpan.FromSeconds(10));
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.UserPrivilege);
            if (_fromConfig)
                UserManager.Instance.SubscribeLongin(userLogin);

            CameraControl = new PanTiltControlManager(ClientHub);
        }

        private void userLogin(object sender, EventArgs e)
        {
            _rootNode = null;
        }

        private void hierUpdated(IEnumerable<string> keys)
        {
            _rootNode = null;
        }

        private void privUpdated(IEnumerable<string> keys)
        {
            _rootNode = null;
        }


        private void onGlobalInfoUpdate(IEnumerable<CCTVGlobalInfo> values, IEnumerable<string> keysUpdated)
        {
            _globalInfo = GetGlobalInfo();
            if (_globalInfo != null)
            {
                if (CCTV1Info == null || CCTV1Info.ServerHost != _globalInfo.CCTV1Host)
                {
                    if (CCTV1Info != null)
                        CCTV1Info.Stop();
                    CCTV1Info = new CCTVInfo(_globalInfo.CCTV1Host);
                    CCTV1Info.Start();
                }
            }
        }
    }
}
