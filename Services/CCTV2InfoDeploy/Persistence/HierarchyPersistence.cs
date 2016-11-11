using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;
using Persistence.Remote;

namespace Persistence
{
    public class HierarchyPersistence
    {
        public static HierarchyPersistence Instance { get; private set; }
        static HierarchyPersistence()
        {
            Instance = new HierarchyPersistence();
        }

        private ConcurrentDictionary<string, InfoSynchronizer<CCTVHierarchyInfo>> _synchronizers;
        private readonly string _baseUrl;
        private const string _sectionFormatter = "CCTVHierarchy.{0}";
        private HierarchyPersistence()
        {
            _baseUrl = ConfigurationManager.AppSettings["InfoServiceAddress"];
            _synchronizers = new ConcurrentDictionary<string, InfoSynchronizer<CCTVHierarchyInfo>>();
        }

        private InfoSynchronizer<CCTVHierarchyInfo> getSynchronizer(string logicalName)
        {
            string section = string.Format(_sectionFormatter, logicalName);
            InfoSynchronizer<CCTVHierarchyInfo> sync = null;
            if (!_synchronizers.TryGetValue(section.ToUpper(), out sync))
            {
                sync = new InfoSynchronizer<CCTVHierarchyInfo>(_baseUrl, section);
                _synchronizers[section.ToUpper()] = sync;
            }
            return sync;
        }

        private void getUpdate(string logicalName)
        {
            getSynchronizer(logicalName).GetUpdate();
        }

        private void update(string logicalName, CCTVHierarchyInfo info, bool isDeleted)
        {
            getSynchronizer(logicalName).PutUpdate(new List<ObjectItem<CCTVHierarchyInfo>>()
            {
                new ObjectItem<CCTVHierarchyInfo>() {
                    Key=info.Id,
                    IsDeleted=isDeleted,
                    Item=info
                }
            });
        }

        private void update(string logicalName,IEnumerable<CCTVHierarchyInfo> infos,bool isDeleted)
        {
            List<ObjectItem<CCTVHierarchyInfo>> objs = new List<ObjectItem<CCTVHierarchyInfo>>();
            foreach(CCTVHierarchyInfo info in infos)
            {
                objs.Add(new ObjectItem<CCTVHierarchyInfo>()
                {
                    Key = info.Id,
                    IsDeleted = isDeleted,
                    Item = info
                });
            }
            getSynchronizer(logicalName).PutUpdate(objs);
        }

        public void Put(string logicalName, CCTVHierarchyInfo info)
        {
            update(logicalName, info, false);
        }

        public void Put(string logicalName,IEnumerable<CCTVHierarchyInfo> infos)
        {
            update(logicalName, infos, false);
        }

        public CCTVHierarchyInfo GetInfo(string logicalName, string nodeId)
        {
            getUpdate(logicalName);
            CCTVHierarchyInfo info;
            getSynchronizer(logicalName).TryGetValue(nodeId, out info);
            return info;
        }

        public IEnumerable<CCTVHierarchyInfo> GetAllInfos(string logicalName)
        {
            getUpdate(logicalName);
            return getSynchronizer(logicalName).Values;
        }

        public void Update(string logicalName, CCTVHierarchyInfo info)
        {
            update(logicalName, info, false);
        }

        public void Update(string logicalName,IEnumerable<CCTVHierarchyInfo> infos)
        {
            update(logicalName, infos, false);
        }

        public void Delete(string logicalName, string nodeId)
        {
            CCTVHierarchyInfo info = GetInfo(logicalName, nodeId);
            if (info != null)
                update(logicalName, info, true);
        }

        public void Delete(string logicalName, IEnumerable<CCTVHierarchyInfo> delInfos)
        {
            update(logicalName, delInfos, true);
        }
    }
}
