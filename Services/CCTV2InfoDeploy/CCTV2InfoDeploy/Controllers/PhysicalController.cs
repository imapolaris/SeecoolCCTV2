using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Models;
using CCTVModels;
using Persistence;

namespace CCTV2InfoDeploy.Controllers
{
    public class PhysicalController : ApiController
    {
        // GET: api/Physical
        public IEnumerable<PhysicalNode> GetNode()
        {
            IEnumerable<CCTVServerInfo> servers = ServerPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVStaticInfo> videos = StaticPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVDeviceInfo> devices = DevicePersistence.Instance.GetAllInfos();
            List<PhysicalNode> pServers = new List<PhysicalNode>();
            List<PhysicalNode> aloneNodes = new List<PhysicalNode>();
            Dictionary<string, List<PhysicalNode>> dServerChildren = new Dictionary<string, List<PhysicalNode>>();
            Dictionary<string, CCTVDeviceInfo> dDevices = new Dictionary<string, CCTVDeviceInfo>();
            foreach (CCTVServerInfo si in servers)
            {
                dServerChildren[si.ServerId] = new List<PhysicalNode>();
                pServers.Add(new PhysicalNode()
                {
                    Id = si.ServerId,
                    Name = si.Name,
                    Type = NodeType.Server
                });
            }
            foreach(CCTVDeviceInfo di in devices)
            {
                dDevices[di.VideoId] = di;
            }

            foreach (CCTVStaticInfo si in videos)
            {
                var pn = new PhysicalNode()
                {
                    Id = si.VideoId,
                    Name = si.Name,
                    Type = NodeType.Video
                };
                if (dDevices.ContainsKey(si.VideoId)&&
                    dServerChildren.ContainsKey(dDevices[si.VideoId].PreferredServerId))
                    dServerChildren[dDevices[si.VideoId].PreferredServerId].Add(pn);
                else
                    aloneNodes.Add(pn);
            }

            foreach (PhysicalNode pn in pServers)
            {
                pn.Children = dServerChildren[pn.Id].ToArray();
            }

            List<PhysicalNode> allNodes = new List<PhysicalNode>();
            if (aloneNodes.Count > 0)
            {
                allNodes.Add(new PhysicalNode()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "未分配",
                    Type = NodeType.Server,
                    Children = aloneNodes.ToArray()
                });
            }

            allNodes.AddRange(pServers);
            return allNodes;
        }
    }
}
