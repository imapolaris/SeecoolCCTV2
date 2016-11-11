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
    public class ServerController : ApiController
    {
        // GET: api/Server
        public IEnumerable<CCTVServerInfo> Get()
        {
            return ServerPersistence.Instance.GetAllInfos();
        }

        // GET: api/Server/5
        public CCTVServerInfo Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的ServerId标识");
            return ServerPersistence.Instance.GetInfo(id);
        }

        [Route("api/Server/idnames")]
        public IEnumerable<IdNamePair> GetIdNames()
        {
            IEnumerable<CCTVServerInfo> servers = ServerPersistence.Instance.GetAllInfos();
            if (servers == null)
                return new List<IdNamePair>();
            else
                return servers.Select(ii => new IdNamePair(ii.ServerId, ii.Name));
        }

        // POST: api/Server
        public IHttpActionResult Post([FromBody]CCTVServerInfo server)
        {
            if (server == null)
                return BadRequest("提交的节点信息数据是一个空值");
            if (string.IsNullOrWhiteSpace(server.ServerId))
                server.ServerId = Guid.NewGuid().ToString();
            if (isNameExist(server))
                return BadRequest("已存在同名的节点服务器");
            ServerPersistence.Instance.Put(server.ServerId, server);
            return Ok("添加节点信息成功");
        }

        //Put:api/server/5
        public IHttpActionResult Put(string id, [FromBody]CCTVServerInfo server)
        {
            if (id == null)
                return BadRequest("无效的ServerId标识");
            if (server == null)
                return BadRequest("提交的节点信息数据是一个空值");
            server.ServerId = id;
            if (isNameExist(server))
                return BadRequest("已存在同名的节点服务器");
            ServerPersistence.Instance.Put(id, server);
            return Ok("修改节点信息成功");
        }

        // DELETE: api/Server/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的ServerId标识");
            ServerPersistence.Instance.Delete(id);
            return Ok("成功删除节点信息");
        }

        private bool isNameExist(CCTVServerInfo server)
        {
            var servers = ServerPersistence.Instance.GetAllInfos();
            if (servers != null && servers.Count() > 0)
            {
                foreach (CCTVServerInfo si in servers)
                {
                    if (si.ServerId != server.ServerId && server.Name.Equals(si.Name))
                        return true;
                }
            }
            return false;
        }
    }
}
