using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Models;
using CCTV2InfoDeploy.Util;
using CCTVModels;
using Persistence;

namespace CCTV2InfoDeploy.Controllers
{
    public class VideoInfoNodeController : ApiController
    {

        // POST: api/VideoInfoNode
        public IHttpActionResult Post([FromBody]VideoInfoNodeViewModel vinvm)
        {
            CCTVVideoInfoWrap viw = vinvm.VideoInfo;
            if (viw == null || viw.StaticInfo == null)
                return BadRequest("视频基本信息不能为空");
            if (string.IsNullOrWhiteSpace(viw.StaticInfo.VideoId))
                viw.StaticInfo.VideoId = Guid.NewGuid().ToString();

            string videoId = viw.StaticInfo.VideoId;
            //生成码流的URL
            if (viw.StaticInfo.Platform == CCTVPlatformType.CCTV2)
                StreamUrlGenner.BuildStreamUrl(viw.StaticInfo.Streams, viw.DeviceInfo);
            StaticPersistence.Instance.Put(videoId, viw.StaticInfo);
            if (viw.Control != null)
            {
                viw.Control.VideoId = videoId;
                ControlPersistence.Instance.Put(videoId, viw.Control);
            }
            if (viw.CameraLimits != null)
            {
                viw.CameraLimits.VideoId = videoId;
                CameraPersistence.Instance.Put(videoId, viw.CameraLimits);
            }
            if (viw.TargetTrack != null)
            {
                viw.TargetTrack.VideoId = videoId;
                TargetTrackPersistence.Instance.Put(videoId, viw.TargetTrack);
            }
            if (viw.VideoTrack != null)
            {
                viw.VideoTrack.VideoId = videoId;
                VideoTrackPersistence.Instance.Put(videoId, viw.VideoTrack);
            }
            if (viw.VideoAnalyze != null)
            {
                viw.VideoAnalyze.VideoId = videoId;
                AnalyzePersistence.Instance.Put(videoId, viw.VideoAnalyze);
            }
            if (viw.DeviceInfo != null)
            {
                viw.DeviceInfo.VideoId = videoId;
                DevicePersistence.Instance.Put(videoId, viw.DeviceInfo);
            }

            CCTVHierarchyInfo hi = new CCTVHierarchyInfo();
            hi.Id = vinvm.NodeId;
            hi.ParentId = vinvm.ParentId;
            hi.Type = NodeType.Video;
            hi.Name = viw.StaticInfo.Name;
            hi.ElementId = viw.StaticInfo.VideoId;
            if (string.IsNullOrWhiteSpace(hi.Id))
                hi.Id = Guid.NewGuid().ToString();
            HierarchyPersistence.Instance.Put(CCTVLogicalTree.DefaultName, hi);
            return Ok("添加视频信息成功");
        }

        // PUT: api/VideoInfoNode/5
        public IHttpActionResult Put(string id, [FromBody]VideoInfoNodeViewModel vinvm)
        {
            CCTVVideoInfoWrap viw = vinvm.VideoInfo;
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("视频ID不能为空");
            if (viw == null)
                return BadRequest("数据不能为空");

            //更新基本信息
            if (viw.StaticInfo != null)
            {
                viw.StaticInfo.VideoId = id;
                //生成码流的URL
                if (viw.StaticInfo.Platform == CCTVPlatformType.CCTV2)
                    StreamUrlGenner.BuildStreamUrl(viw.StaticInfo.Streams, viw.DeviceInfo);
                StaticPersistence.Instance.Put(id, viw.StaticInfo);
            }
            //更新基本信息以外的其他信息。
            if (viw.Control != null)
            {
                viw.Control.VideoId = id;
                ControlPersistence.Instance.Put(id, viw.Control);
            }
            if (viw.CameraLimits != null)
            {
                viw.CameraLimits.VideoId = id;
                CameraPersistence.Instance.Put(id, viw.CameraLimits);
            }
            if (viw.TargetTrack != null)
            {
                viw.TargetTrack.VideoId = id;
                TargetTrackPersistence.Instance.Put(id, viw.TargetTrack);
            }
            if (viw.VideoTrack != null)
            {
                viw.VideoTrack.VideoId = id;
                VideoTrackPersistence.Instance.Put(id, viw.VideoTrack);
            }
            if (viw.VideoAnalyze != null)
            {
                viw.VideoAnalyze.VideoId = id;
                AnalyzePersistence.Instance.Put(id, viw.VideoAnalyze);
            }
            if (viw.DeviceInfo != null)
            {
                viw.DeviceInfo.VideoId = id;
                DevicePersistence.Instance.Put(id, viw.DeviceInfo);
            }

            CCTVHierarchyInfo hi = new CCTVHierarchyInfo();
            hi.Id = vinvm.NodeId;
            hi.ParentId = vinvm.ParentId;
            hi.Type = NodeType.Video;
            hi.Name = viw.StaticInfo.Name;
            hi.ElementId = viw.StaticInfo.VideoId;
            if (string.IsNullOrWhiteSpace(hi.Id))
                hi.Id = Guid.NewGuid().ToString();
            HierarchyPersistence.Instance.Put(CCTVLogicalTree.DefaultName, hi);
            return Ok("修改视频信息成功");
        }

        // POST: api/VideoInfoNode/Delete
        [HttpPost]
        [Route("api/VideoInfoNode/Delete")]
        public IHttpActionResult Delete([FromBody] VideoInfoDeleteViewModel di)
        {
            StaticPersistence.Instance.Delete(di.VideoId);
            ControlPersistence.Instance.Delete(di.VideoId);
            CameraPersistence.Instance.Delete(di.VideoId);
            TargetTrackPersistence.Instance.Delete(di.VideoId);
            VideoTrackPersistence.Instance.Delete(di.VideoId);
            AnalyzePersistence.Instance.Delete(di.VideoId);
            DevicePersistence.Instance.Delete(di.VideoId);

            HierarchyPersistence.Instance.Delete(CCTVLogicalTree.DefaultName, di.NodeId);
            return Ok("删除视频信息成功");
        }
    }
}
