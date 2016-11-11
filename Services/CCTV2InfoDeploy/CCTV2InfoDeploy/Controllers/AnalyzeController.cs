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
    public class AnalyzeController : ApiController
    {
        // GET: api/Analyze
        public IEnumerable<CCTVVideoAnalyze> Get()
        {
            return AnalyzePersistence.Instance.GetAllInfos();
        }

        // GET: api/Analyze/5
        public CCTVVideoAnalyze Get(string id)
        {
            if (id == null)
                throw new HttpRequestException("无效的VideoId标识");
            return AnalyzePersistence.Instance.GetInfo(id);
        }

        // POST: api/Analyze
        public IHttpActionResult Post([FromBody]CCTVVideoAnalyze analyze)
        {
            if (analyze == null)
                return BadRequest("提交的数据是空值");
            if (string.IsNullOrWhiteSpace(analyze.VideoId))
                return BadRequest("无效的VideoId标识");
            AnalyzePersistence.Instance.Put(analyze.VideoId, analyze);
            return Ok("添加视频分析信息成功");
        }

        //Put:api/Analyze/5
        public IHttpActionResult Put(string id, [FromBody]CCTVVideoAnalyze analyze)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            if (analyze == null)
                return BadRequest("提交的数据是空值");
            AnalyzePersistence.Instance.Put(id, analyze);
            return Ok("修改视频分析信息成功");
        }

        // DELETE: api/Analyze/5
        public IHttpActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest("无效的VideoId标识");
            AnalyzePersistence.Instance.Delete(id);
            return Ok("删除视频分析信息成功");
        }
    }
}
