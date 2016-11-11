using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Linker;
using CCTV2InfoDeploy.Util;
using GatewayModels;
using GatewayModels.Param;
using Persistence.GBT;

namespace CCTV2InfoDeploy.NetGateway
{
    public class GatewayController : ApiController
    {
        // GET: api/Gateway
        public GatewayStatus Get()
        {
            Gateway gw = GatewayPersistence.Instance.Current;
            GatewayStatus gws = new GatewayStatus(gw);
            BoolPacket bp = GatewayLinker.Instance.GetValue<BoolPacket>(new CodePacket(MessageCode.IsServerStarted));
            gws.IsStarted = bp == null ? false : bp.Value;
            return gws;
        }


        // POST: api/Gateway
        public IHttpActionResult Post([FromBody]Gateway plat)
        {
            if (plat == null)
                return BadRequest("提交的网关数据是一个空值");
            GatewayPersistence.Instance.Put(Gateway.Key, plat);
            BoolPacket bp = GatewayLinker.Instance.GetValue<BoolPacket>(new CodePacket(MessageCode.IsServerStarted));
            if (bp != null && bp.Value)
            {
                GatewayLinker.Instance.SendCommand(new CodePacket(MessageCode.StartServer));
            }
            return Ok("配置网关信息成功");
        }

        // GET:api/Gateway/Stop
        [Route("api/Gateway/Stop")]
        public IHttpActionResult GetStop()
        {
            GatewayLinker.Instance.SendCommand(new CodePacket(MessageCode.StopServer));
            return Ok("互联服务已关闭");
        }

        // GET:api/Gateway/Start
        [Route("api/Gateway/Start")]
        public IHttpActionResult GetStart()
        {
            GatewayLinker.Instance.SendCommand(new CodePacket(MessageCode.StartServer));
            return Ok("互联服务已启动");
        }
    }
}
