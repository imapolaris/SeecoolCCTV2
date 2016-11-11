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
    public class CameraController : ApiController
    {
        // GET: api/Camera
        public IEnumerable<CCTVCameraLimits> Get()
        {
            return CameraPersistence.Instance.GetAllInfos();
        }

        // GET: api/Camera/5
        public CCTVCameraLimits Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return CameraPersistence.Instance.GetInfo(id);
        }

        // POST: api/Camera
        public IHttpActionResult Post([FromBody]CCTVCameraLimits camera)
        {
            if (camera == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(camera.VideoId))
                return BadRequest("无效的VideoId标识");
            CameraPersistence.Instance.Put(camera.VideoId, camera);
            return Ok("添加摄像机相位信息成功");
        }

        //Put:api/Camera/5
        public IHttpActionResult Put(string id, [FromBody]CCTVCameraLimits camera)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (camera == null)
                return BadRequest("提交的数据是空值");
            CameraPersistence.Instance.Put(id, camera);
            return Ok("修改摄像机相位信息成功");
        }

        // DELETE: api/Camera/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            CameraPersistence.Instance.Delete(id);
            return Ok("删除摄像机相位信息成功");
        }
    }
}
