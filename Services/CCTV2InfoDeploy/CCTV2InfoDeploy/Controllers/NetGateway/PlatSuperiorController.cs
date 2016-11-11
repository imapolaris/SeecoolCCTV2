using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Linker;
using CCTV2InfoDeploy.Persistence;
using CCTVModels.User;
using GatewayModels;
using GatewayModels.Param;
using Persistence.GBT;

namespace CCTV2InfoDeploy.NetGateway
{
    public class PlatSuperiorController : ApiController
    {
        // GET: api/PlatSuperior
        public IEnumerable<Platform> Get()
        {
            return getAllSuperPlats();
        }

        // GET: api/PlatSuperior/status
        [Route("api/PlatSuperior/Status")]
        public IEnumerable<PlatformStatus> GetPlatformWithStatus()
        {
            IEnumerable<Platform> plats = getAllSuperPlats();
            if (plats != null)
            {
                List<PlatformStatus> pStatus = new List<PlatformStatus>();
                foreach (Platform p in plats)
                {
                    PlatformStatus pfs = new PlatformStatus(p);
                    BoolPacket bp = GatewayLinker.Instance.GetValue<BoolPacket>(new StringPacket(MessageCode.IsSuperiorOnline, p.Id));
                    pfs.Online = bp == null ? false : bp.Value; //获取状态。
                    pStatus.Add(pfs);
                }
                return pStatus;
            }
            return null;
        }

        // GET: api/PlatSuperior/5
        public Platform Get(string id)
        {
            return getSuperPlat(id);
        }


        // POST: api/PlatSuperior
        public IHttpActionResult Post([FromBody]Platform plat)
        {
            if (plat == null)
                return BadRequest("提交的平台信息数据是一个空值");
            plat.Type = PlatformType.Superior;
            plat.Id = Guid.NewGuid().ToString();
            CCTVUserInfo ui = UserInfoPersistence.Instance.GetInfo(plat.UserName);
            if (ui == null)
                return BadRequest($"鉴权用户[{plat.UserName}]不是当前平台的有效用户。");
            PlatformPersistence.Instance.Put(plat.Id, plat);
            GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.StartRegister, plat.Id));//启动注册服务。
            return Ok("添加平台信息成功");
        }

        //Put:api/PlatSuperior/5
        public IHttpActionResult Put(string id, [FromBody]Platform plat)
        {
            if (plat == null)
                return BadRequest("提交的平台信息数据是一个空值");
            plat.Type = PlatformType.Superior;
            plat.Id = id;
            CCTVUserInfo ui = UserInfoPersistence.Instance.GetInfo(plat.UserName);
            if (ui == null)
                return BadRequest($"鉴权用户[{plat.UserName}]不是当前平台的有效用户。");
            PlatformPersistence.Instance.Put(id, plat);
            //重新启动注册服务。
            GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.StartRegister, plat.Id));
            return Ok("添加平台信息成功");
        }

        //Put:api/PlatSuperior/ShareTo/5
        [HttpPost]
        [Route("api/PlatSuperior/ShareTo/{id}")]
        public IHttpActionResult ShareTo(string id)
        {
            GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.ShareDevice, id));
            return Ok("已发送共享设备信息");
        }

        // DELETE: api/PlatSuperior/5
        public IHttpActionResult Delete(string id)
        {
            PlatformPersistence.Instance.Delete(id);
            //关闭注册服务。
            if (GatewayLinker.Instance.IsConnected)
                GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.StopRegister, id));
            return Ok("成功删除平台信息");
        }

        private IEnumerable<Platform> getAllSuperPlats()
        {
            return PlatformPersistence.Instance.GetAllInfos().Where(p => p.Type == PlatformType.Superior);
        }

        private Platform getSuperPlat(string id)
        {
            Platform plat = PlatformPersistence.Instance.GetInfo(id);
            return (plat != null && plat.Type == PlatformType.Superior) ? plat : null;
        }
    }
}
