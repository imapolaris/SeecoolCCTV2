using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVInfoHub.Entity;
using CCTVModels;

namespace CCTVInfoHub.Util
{
    public static class HierarchyInfoUtil
    {
        public static string CreateSection(string logicalTreeName)
        {
            return $"{DefaultSections.HierarchyInfoPrefix}.{logicalTreeName}";
        }

        public static IEnumerable<CCTVHierarchyNode> BuildTree(IEnumerable<CCTVHierarchyInfo> infos)
        {
            Dictionary<string, List<CCTVHierarchyNode>> childrenDict = new Dictionary<string, List<CCTVHierarchyNode>>();
            Dictionary<string, CCTVHierarchyNode> models = new Dictionary<string, CCTVHierarchyNode>();
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
            return findRoots(infos, models);
        }

        private static IEnumerable<CCTVHierarchyNode> findRoots(IEnumerable<CCTVHierarchyInfo> infos,
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

        public static IEnumerable<CCTVHierarchyNode> FilterNodes(IEnumerable<CCTVHierarchyNode> nodes, string[] filterIds)
        {
            Dictionary<string, string> idid = filterIds.ToDictionary(id => id);
            List<CCTVHierarchyNode> rtnNodes = new List<CCTVHierarchyNode>();
            foreach (CCTVHierarchyNode node in nodes)
            {
                var temp = cloneNode(node);
                if (hasMatchedChild(temp, idid))
                    rtnNodes.Add(temp);
            }
            return rtnNodes;
        }

        private static bool hasMatchedChild(CCTVHierarchyNode node, Dictionary<string, string> idid)
        {
            if (idid.ContainsKey(node.Id))
            {
                return true;
            }
            else
            {
                bool matched = false;
                if (node.Children != null)
                {
                    List<CCTVHierarchyNode> nodeList = new List<CCTVHierarchyNode>();
                    foreach (CCTVHierarchyNode subNode in node.Children)
                    {
                        var temp = cloneNode(subNode);
                        bool subMatched = hasMatchedChild(temp, idid);
                        matched |= subMatched;
                        if (subMatched)
                            nodeList.Add(temp);
                    }
                    node.Children = nodeList.ToArray();
                }
                return matched;
            }
        }

        private static CCTVHierarchyNode cloneNode(CCTVHierarchyNode node)
        {
            return new CCTVHierarchyNode()
            {
                Id = node.Id,
                ElementId = node.ElementId,
                Type = node.Type,
                Name = node.Name,
                Children = node.Children
            };
        }
    }
}
