using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CenterStorageDeploy.Managers;
using CenterStorageDeploy.Models;

namespace CenterStorageDeploy.Controllers
{
    public class ServerController : ApiController
    {
        // GET: api/Server
        public IHttpActionResult Get()
        {
            StorageSource ss = NodesManager.Instance.GetStorageSource();
            if (ss == null)
                return BadRequest("尚未设置集中存储服务器地址");
            return Ok(ss);
        }


        // POST: api/Server
        public IHttpActionResult Post([FromBody]StorageSource ss)
        {
            if (ss == null)
                return BadRequest("传入的数据不能为空");
            NodesManager.Instance.UpdateStorageSource(ss);
            return Ok("更新集中存储器地址成功。");
        }
    }
}
