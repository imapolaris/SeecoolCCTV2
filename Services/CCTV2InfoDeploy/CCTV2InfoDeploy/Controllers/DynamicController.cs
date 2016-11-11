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
    public class DynamicController : ApiController
    {
        // GET: api/Dynamic
        public IEnumerable<CCTVDynamicInfo> Get()
        {
            return DynamicPersistence.Instance.GetAllInfos();
        }

        // GET: api/Dynamic/5
        [Route("api/Dynamic/{videoId}")]
        public CCTVDynamicInfo Get(string videoId)
        {
            return DynamicPersistence.Instance.GetInfo(videoId);
        }

        // POST: api/Dynamic
        public void Post([FromBody]CCTVDynamicInfo info)
        {
            if (info == null)
                throw new HttpRequestException("动态信息内容不能为空。");
            if (string.IsNullOrWhiteSpace(info.VideoId))
                info.VideoId = Guid.NewGuid().ToString();
            DynamicPersistence.Instance.Put(info.VideoId, info);
        }

        // DELETE: api/Dynamic/5
        [Route("api/Dynamic/{videoId}")]
        public void Delete(string videoId)
        {
            DynamicPersistence.Instance.Delete(videoId);
        }
    }
}
