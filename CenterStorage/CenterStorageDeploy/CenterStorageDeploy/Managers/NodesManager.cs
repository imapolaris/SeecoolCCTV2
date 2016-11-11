using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels;
using CenterStorageDeploy.Models;

namespace CenterStorageDeploy.Managers
{
    public class NodesManager
    {
        static NodesManager()
        {
            Instance = new NodesManager();
        }

        public static NodesManager Instance { get; private set; }

        private CCTVDefaultInfoSync _hub;
        private NodesManager()
        {
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings["StaticInfoAddress"];
            _hub = new CCTVDefaultInfoSync(baseAddress);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.HierarchyInfo);

            SyncParams<StorageSource> param = new SyncParams<StorageSource>("CenterStorage", TimeSpan.FromSeconds(10));
            _hub.RegisterSynchronizer(param);
        }

        public CCTVHierarchyNode GetHierarchyRoot()
        {
            _hub.UpdateDefault(CCTVInfoType.HierarchyInfo);
            CCTVHierarchyNode[] nodes = _hub.GetAllHierarchyRoots();
            if (nodes != null&&nodes.Length>0)
            {
                if (nodes.Length == 1)
                    return nodes[0];
                else
                {
                    return new CCTVHierarchyNode()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = NodeType.Server,
                        Name = "虚拟节点",
                        Children = nodes
                    };
                }
            }
            else
            {
                return null;
            }
        }

        public StorageSource GetStorageSource()
        {
            _hub.UpdateRegistered<StorageSource>();
            StorageSource ss = _hub.GetRegisteredInfo<StorageSource>("Default");
            return ss;
        }

        public void UpdateStorageSource(StorageSource ss)
        {
            _hub.PutRegisteredInfo<StorageSource>("Default", ss, false);
        }
    }
}
