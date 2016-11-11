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
    public class TargetTrackController : ApiController
    {
        // GET: api/TargetTrack
        public IEnumerable<CCTVTargetTrack> Get()
        {
            return TargetTrackPersistence.Instance.GetAllInfos();
        }

        // GET: api/TargetTrack/5
        public CCTVTargetTrack Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return TargetTrackPersistence.Instance.GetInfo(id);
        }

        // POST: api/TargetTrack
        public IHttpActionResult Post([FromBody]CCTVTargetTrack target)
        {
            if (target == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(target.VideoId))
                return BadRequest("无效的VideoId标识");
            TargetTrackPersistence.Instance.Put(target.VideoId, target);
            return Ok("添加目标跟踪信息成功");
        }

        //Put:api/TargetTrack/5
        public IHttpActionResult Put(string id, [FromBody]CCTVTargetTrack target)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (target == null)
                return BadRequest("提交的数据是空值");
            TargetTrackPersistence.Instance.Put(id, target);
            return Ok("修改目标跟踪信息成功");
        }

        // DELETE: api/TargetTrack/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            TargetTrackPersistence.Instance.Delete(id);
            return Ok("删除目标跟踪信息成功");
        }
    }
}
