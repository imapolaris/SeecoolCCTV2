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
    public class DeviceInfoController : ApiController
    {
        // GET: api/DeviceInfo
        public IEnumerable<CCTVDeviceInfo> Get()
        {
            return DevicePersistence.Instance.GetAllInfos();
        }

        // GET: api/DeviceInfo/5
        public CCTVDeviceInfo Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return DevicePersistence.Instance.GetInfo(id);
        }

        // POST: api/DeviceInfo
        public IHttpActionResult Post([FromBody]CCTVDeviceInfo device)
        {
            if (device == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(device.VideoId))
                return BadRequest("无效的VideoId标识");
            //更新码流Url
            CCTVStaticInfo si = StaticPersistence.Instance.GetInfo(device.VideoId);
            if (si != null && si.Platform == CCTVPlatformType.CCTV2)
            {
                StreamUrlGenner.BuildStreamUrl(si.Streams, device);
                StaticPersistence.Instance.Put(device.VideoId, si);
            }
            DevicePersistence.Instance.Put(device.VideoId, device);
            return Ok("添加视频设备信息成功");
        }

        //Put:api/DeviceInfo/5
        public IHttpActionResult Put(string id, [FromBody]CCTVDeviceInfo device)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (device == null)
                return BadRequest("提交的数据是空值");
            //更新码流Url
            CCTVStaticInfo si = StaticPersistence.Instance.GetInfo(id);
            if (si != null && si.Platform == CCTVPlatformType.CCTV2)
            {
                StreamUrlGenner.BuildStreamUrl(si.Streams, device);
                StaticPersistence.Instance.Put(id, si);
            }
            DevicePersistence.Instance.Put(id, device);
            return Ok("修改视频设备信息成功");
        }

        // DELETE: api/DeviceInfo/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            DevicePersistence.Instance.Delete(id);
            return Ok("删除视频设备信息成功");
        }
    }
}
