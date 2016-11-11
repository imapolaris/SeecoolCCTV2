using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Util;
using CCTVModels;
using Persistence;

namespace CCTV2InfoDeploy.Controllers
{
    public class StaticController : ApiController
    {
        // GET: api/Static
        public IEnumerable<CCTVStaticInfo> Get()
        {
            return StaticPersistence.Instance.GetAllInfos();
        }

        // GET: api/Static/5
        public CCTVStaticInfo Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return StaticPersistence.Instance.GetInfo(id);
        }

        [Route("api/static/idnames")]
        public IEnumerable<IdNamePair> GetIdNames()
        {
            IEnumerable<CCTVStaticInfo> infos = StaticPersistence.Instance.GetAllInfos();
            if (infos == null)
                return new List<IdNamePair>();
            else
                return infos.Select(ii => new IdNamePair(ii.VideoId, ii.Name));
        }

        // POST: api/Static
        public IHttpActionResult Post([FromBody]CCTVStaticInfo info)
        {
            if (info == null)
                throw new HttpRequestException("视频节点静态信息内容为空。");
            if (string.IsNullOrWhiteSpace(info.VideoId))
                info.VideoId = Guid.NewGuid().ToString();
            StaticPersistence.Instance.Put(info.VideoId, info);
            return Ok("添加静态信息成功");
        }

        //Put: api/static/5
        public IHttpActionResult Put(string id, [FromBody]CCTVStaticInfo info)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (info == null)
                return BadRequest("请求的静态信息对象为空");
            //更新码流Url
            if (info.Platform == CCTVPlatformType.CCTV2)
            {
                CCTVDeviceInfo di = DevicePersistence.Instance.GetInfo(info.VideoId);
                if (di != null)
                    StreamUrlGenner.BuildStreamUrl(info.Streams, di);
            }
            StaticPersistence.Instance.Put(id, info);
            return Ok("修改静态信息成功");
        }

        // DELETE: api/Static/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            StaticPersistence.Instance.Delete(id);
            ControlPersistence.Instance.Delete(id);
            CameraPersistence.Instance.Delete(id);
            TargetTrackPersistence.Instance.Delete(id);
            VideoTrackPersistence.Instance.Delete(id);
            AnalyzePersistence.Instance.Delete(id);
            return Ok("删除静态信息成功");
        }
    }
}
