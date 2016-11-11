using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTVModels;
using Persistence;

namespace CCTV2InfoDeploy.Controllers
{
    [RoutePrefix("api/Hierarchy")]
    public class HierarchyController : ApiController
    {
        [Route("LogicalTree/{logicalName?}")]
        public IEnumerable<CCTVHierarchyNode> GetNode(string logicalName = "Default")
        {
            IEnumerable<CCTVHierarchyInfo> infos = HierarchyPersistence.Instance.GetAllInfos(logicalName);
            return buildRoots(infos);
        }

        // GET: api/Hierarchy/logicalname
        [Route("{logicalName?}")]
        public IEnumerable<CCTVHierarchyInfo> Get(string logicalName = "Default")
        {
            return HierarchyPersistence.Instance.GetAllInfos(logicalName);
        }

        // GET: api/Hierarchy/5/logicalname
        [Route("{nodeId}/{logicalName?}")]
        public CCTVHierarchyInfo Get(string nodeId, string logicalName = "Default")
        {
            return HierarchyPersistence.Instance.GetInfo(logicalName, nodeId);
        }

        // POST: api/Hierarchy/logicalname
        [Route("{logicalName?}")]
        public void Post([FromBody]CCTVHierarchyInfo info, string logicalName = "Default")
        {
            if (info == null)
                throw new HttpRequestException("逻辑树节点内容不能为空。");
            if (string.IsNullOrWhiteSpace(info.Id))
                info.Id = Guid.NewGuid().ToString();
            HierarchyPersistence.Instance.Put(logicalName, info);
        }
        // POST: api/Hierarchies/logicalname
        [Route("~/api/Hierarchies/{logicalName?}")]
        public void Post([FromBody]IEnumerable<CCTVHierarchyInfo> infos, string logicalName = "Default")
        {
            if (infos == null)
                throw new HttpRequestException("逻辑树节点内容不能为空。");
            foreach(CCTVHierarchyInfo info in infos)
            {
                if (string.IsNullOrWhiteSpace(info.Id))
                    info.Id = Guid.NewGuid().ToString();
            }
            HierarchyPersistence.Instance.Put(logicalName, infos);
        }


        // DELETE: api/Hierarchy/5/logicalname
        [Route("{nodeId}/{logicalName?}")]
        public void Delete(string nodeId, string logicalName = "Default")
        {
            IEnumerable<CCTVHierarchyInfo> infos = HierarchyPersistence.Instance.GetAllInfos(logicalName);
            deleteRecursively(infos, nodeId, logicalName);
            //HierarchyPersistence.Instance.Delete(logicalName, nodeId);
        }

        private void deleteRecursively(IEnumerable<CCTVHierarchyInfo> infos, string nodeId, string logicalName)
        {
            Dictionary<string, List<CCTVHierarchyNode>> childrenDict = new Dictionary<string, List<CCTVHierarchyNode>>();
            Dictionary<string, CCTVHierarchyNode> models = new Dictionary<string, CCTVHierarchyNode>();
            buildNodeDict(infos, childrenDict, models);
            List<string> delIds = new List<string>();
            if (models.ContainsKey(nodeId))
            {
                findIdsRecursively(models[nodeId], delIds);
            }

            IEnumerable<CCTVHierarchyInfo> delInfos = infos.Where(info => delIds.Contains(info.Id));
            HierarchyPersistence.Instance.Delete(logicalName, delInfos);
        }

        private void findIdsRecursively(CCTVHierarchyNode node, List<string> ids)
        {
            ids.Add(node.Id);
            if (node.Children != null && node.Children.Length > 0)
            {
                foreach (CCTVHierarchyNode child in node.Children)
                {
                    findIdsRecursively(child, ids);
                }
            }
        }

        private IEnumerable<CCTVHierarchyNode> buildRoots(IEnumerable<CCTVHierarchyInfo> infos)
        {
            Dictionary<string, List<CCTVHierarchyNode>> childrenDict = new Dictionary<string, List<CCTVHierarchyNode>>();
            Dictionary<string, CCTVHierarchyNode> models = new Dictionary<string, CCTVHierarchyNode>();
            buildNodeDict(infos, childrenDict, models);
            return findRoots(infos, models);
        }

        private void buildNodeDict(IEnumerable<CCTVHierarchyInfo> infos
            , Dictionary<string, List<CCTVHierarchyNode>> childrenDict
            , Dictionary<string, CCTVHierarchyNode> models)
        {
            foreach (CCTVHierarchyInfo info in infos)
            {
                CCTVHierarchyNode model = new CCTVHierarchyNode()
                {
                    Id = info.Id,
                    Name = info.Name,
                    Type = info.Type,
                    ElementId = info.ElementId
                };
                models[info.Id] = model;

                if (!string.IsNullOrWhiteSpace(info.ParentId))
                {
                    if (!childrenDict.ContainsKey(info.ParentId))
                        childrenDict[info.ParentId] = new List<CCTVHierarchyNode>();
                    childrenDict[info.ParentId].Add(model);
                }
            }

            foreach (CCTVHierarchyNode model in models.Values)
            {
                if (childrenDict.ContainsKey(model.Id))
                    model.Children = childrenDict[model.Id].ToArray();
            }
        }

        private IEnumerable<CCTVHierarchyNode> findRoots(IEnumerable<CCTVHierarchyInfo> infos,
            Dictionary<string, CCTVHierarchyNode> models)
        {
            var topNodeIds = infos.Where(info => string.IsNullOrWhiteSpace(info.ParentId) || !models.ContainsKey(info.ParentId))
                .Select(info => info.Id);
            int count = topNodeIds.Count();
            List<CCTVHierarchyNode> nodes = new List<CCTVHierarchyNode>();
            foreach (string id in topNodeIds)
            {
                nodes.Add(models[id]);
            }
            return nodes;
        }
    }
}
