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
    public class VideoTrackController : ApiController
    {
        // GET: api/VideoTrack
        public IEnumerable<CCTVVideoTrack> Get()
        {
            return VideoTrackPersistence.Instance.GetAllInfos();
        }

        // GET: api/VideoTrack/5
        public CCTVVideoTrack Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return VideoTrackPersistence.Instance.GetInfo(id);
        }

        // POST: api/VideoTrack
        public IHttpActionResult Post([FromBody]CCTVVideoTrack control)
        {
            if (control == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(control.VideoId))
                return BadRequest("无效的VideoId标识");
            VideoTrackPersistence.Instance.Put(control.VideoId, control);
            return Ok("添加视频跟踪信息成功");
        }

        //Put:api/VideoTrack/5
        public IHttpActionResult Put(string id, [FromBody]CCTVVideoTrack control)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (control == null)
                return BadRequest("提交的数据是空值");
            VideoTrackPersistence.Instance.Put(id, control);
            return Ok("修改视频跟踪信息成功");
        }

        // DELETE: api/VideoTrack/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            VideoTrackPersistence.Instance.Delete(id);
            return Ok("删除视频跟踪信息成功");
        }
    }
}
