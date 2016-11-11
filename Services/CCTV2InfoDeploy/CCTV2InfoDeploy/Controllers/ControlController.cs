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
    public class ControlController : ApiController
    {
        // GET: api/Control
        public IEnumerable<CCTVControlConfig> Get()
        {
            return ControlPersistence.Instance.GetAllInfos();
        }

        // GET: api/Control/5
        public CCTVControlConfig Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return ControlPersistence.Instance.GetInfo(id);
        }

        // POST: api/Control
        public IHttpActionResult Post([FromBody]CCTVControlConfig control)
        {
            if (control == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(control.VideoId))
                return BadRequest("无效的VideoId标识");
            ControlPersistence.Instance.Put(control.VideoId, control);
            return Ok("添加视频控制信息成功");
        }

        //Put:api/Control/5
        public IHttpActionResult Put(string id, [FromBody]CCTVControlConfig control)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (control == null)
                return BadRequest("提交的数据是空值");
            ControlPersistence.Instance.Put(id, control);
            return Ok("修改视频控制信息成功");
        }

        // DELETE: api/Control/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            ControlPersistence.Instance.Delete(id);
            return Ok("删除视频控制信息成功");
        }
    }
}
