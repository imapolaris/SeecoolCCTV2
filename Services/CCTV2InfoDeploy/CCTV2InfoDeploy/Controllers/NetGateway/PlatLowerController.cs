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

namespace CCTV2InfoDeploy.Controllers.NetGateway
{
    public class PlatLowerController : ApiController
    {
        // GET: api/PlatLower
        public IEnumerable<Platform> Get()
        {
            return getAllLowerPlats();
        }

        // GET: api/PlatLower/status
        [Route("api/PlatLower/Status")]
        public IEnumerable<PlatformStatus> GetPlatformWithStatus()
        {
            IEnumerable<Platform> plats = getAllLowerPlats();
            if (plats != null)
            {
                List<PlatformStatus> pStatus = new List<PlatformStatus>();
                foreach (Platform p in plats)
                {
                    PlatformStatus pfs = new PlatformStatus(p);
                    BoolPacket bp = GatewayLinker.Instance.GetValue<BoolPacket>(new StringPacket(MessageCode.IsLowerOnline, p.Id));
                    pfs.Online = bp == null ? false : bp.Value; //获取状态。
                    pStatus.Add(pfs);
                }
                return pStatus;
            }
            return null;
        }

        //GET:api/PlatLower/Devices/id;
        [Route("api/PlatLower/Devices/{id}")]
        public PlatformDeviceSet GetPlatformDevices(string id)
        {
            return PlatformDevicePersistence.Instance.GetInfo(id);
        }

        // GET: api/PlatLower/5
        public Platform Get(string id)
        {
            return getLowerPlat(id);
        }


        // POST: api/PlatLower
        public IHttpActionResult Post([FromBody]Platform plat)
        {
            if (plat == null)
                return BadRequest("提交的平台信息数据是一个空值");
            plat.Type = PlatformType.Lower;
            plat.Id = Guid.NewGuid().ToString();
            CCTVUserInfo ui = UserInfoPersistence.Instance.GetInfo(plat.UserName);
            if (ui == null)
                return BadRequest($"鉴权用户[{plat.UserName}]不是当前平台的有效用户。");
            PlatformPersistence.Instance.Put(plat.Id, plat);
            return Ok("添加下级平台信息成功");
        }

        //Put:api/PlatLower/5
        public IHttpActionResult Put(string id, [FromBody]Platform plat)
        {
            if (plat == null)
                return BadRequest("提交的平台信息数据是一个空值");
            plat.Type = PlatformType.Lower;
            plat.Id = id;
            CCTVUserInfo ui = UserInfoPersistence.Instance.GetInfo(plat.UserName);
            if (ui == null)
                return BadRequest($"鉴权用户[{plat.UserName}]不是当前平台的有效用户。");
            PlatformPersistence.Instance.Put(id, plat);
            return Ok("修改下级平台信息成功");
        }

        //Put:api/PlatLower/Query/5
        [HttpPost]
        [Route("api/PlatLower/Query/{id}")]
        public IHttpActionResult Query(string id)
        {
            GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.QueryDevice, id));
            return Ok("已发送查询设备信息请求");
        }

        // DELETE: api/PlatLower/5
        public IHttpActionResult Delete(string id)
        {
            Platform pf = PlatformPersistence.Instance.GetInfo(id);
            PlatformPersistence.Instance.Delete(id);
            PlatformDevicePersistence.Instance.Delete(id);
            if (pf != null && GatewayLinker.Instance.IsConnected)
                GatewayLinker.Instance.SendCommand(new StringPacket(MessageCode.RemoveLowerPlatform, $"{pf.SipNumber}@{pf.Ip}"));
            return Ok("成功删除平台信息");
        }

        private IEnumerable<Platform> getAllLowerPlats()
        {
            return PlatformPersistence.Instance.GetAllInfos().Where(p => p.Type == PlatformType.Lower);
        }

        private Platform getLowerPlat(string id)
        {
            Platform plat = PlatformPersistence.Instance.GetInfo(id);
            return (plat != null && plat.Type == PlatformType.Lower) ? plat : null;
        }
    }
}
