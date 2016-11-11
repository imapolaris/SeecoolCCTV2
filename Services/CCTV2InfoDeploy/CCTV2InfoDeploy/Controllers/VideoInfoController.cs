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
    public class VideoInfoController : ApiController
    {
        // GET: api/VideoInfo
        public IEnumerable<CCTVVideoInfoWrap> Get()
        {
            IEnumerable<CCTVStaticInfo> statics = StaticPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVControlConfig> controls = ControlPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVCameraLimits> cameras = CameraPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVTargetTrack> tTracks = TargetTrackPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVVideoTrack> vTracks = VideoTrackPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVVideoAnalyze> analyzes = AnalyzePersistence.Instance.GetAllInfos();
            IEnumerable<CCTVDeviceInfo> devices = DevicePersistence.Instance.GetAllInfos();
            Dictionary<string, CCTVVideoInfoWrap> videos = new Dictionary<string, CCTVVideoInfoWrap>();
            foreach (CCTVStaticInfo st in statics)
            {
                CCTVVideoInfoWrap vvm = new CCTVVideoInfoWrap()
                {
                    StaticInfo = st
                };
                videos[st.VideoId] = vvm;
            }
            foreach (CCTVControlConfig cc in controls)
            {
                if (videos.ContainsKey(cc.VideoId))
                    videos[cc.VideoId].Control = cc;
            }
            foreach (CCTVCameraLimits cli in cameras)
            {
                if (videos.ContainsKey(cli.VideoId))
                    videos[cli.VideoId].CameraLimits = cli;
            }
            foreach (CCTVTargetTrack ti in tTracks)
            {
                if (videos.ContainsKey(ti.VideoId))
                    videos[ti.VideoId].TargetTrack = ti;
            }
            foreach (CCTVVideoTrack vt in vTracks)
            {
                if (videos.ContainsKey(vt.VideoId))
                    videos[vt.VideoId].VideoTrack = vt;
            }
            foreach (CCTVVideoAnalyze va in analyzes)
            {
                if (videos.ContainsKey(va.VideoId))
                    videos[va.VideoId].VideoAnalyze = va;
            }
            foreach (CCTVDeviceInfo di in devices)
            {
                if (videos.ContainsKey(di.VideoId))
                    videos[di.VideoId].DeviceInfo = di;
            }
            return videos.Values.ToArray();
        }

        // GET: api/VideoInfo/5
        public IHttpActionResult Get(string id)
        {
            CCTVStaticInfo si = StaticPersistence.Instance.GetInfo(id);
            if (si != null)
            {
                CCTVVideoInfoWrap video = new CCTVVideoInfoWrap();
                video.StaticInfo = si;
                video.Control = ControlPersistence.Instance.GetInfo(id);
                video.CameraLimits = CameraPersistence.Instance.GetInfo(id);
                video.TargetTrack = TargetTrackPersistence.Instance.GetInfo(id);
                video.VideoTrack = VideoTrackPersistence.Instance.GetInfo(id);
                video.VideoAnalyze = AnalyzePersistence.Instance.GetInfo(id);
                video.DeviceInfo = DevicePersistence.Instance.GetInfo(id);
                return Ok(video);
            }
            return BadRequest("无效的VideoId");
        }

        // POST: api/VideoInfo
        public IHttpActionResult Post([FromBody]CCTVVideoInfoWrap viw)
        {
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
            return Ok("添加视频信息成功");
        }

        // PUT: api/VideoInfo/5
        public IHttpActionResult Put(string id, [FromBody]CCTVVideoInfoWrap viw)
        {
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
            return Ok("修改视频信息成功");
        }

        // DELETE: api/VideoInfo/5
        public IHttpActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("视频ID不能为空");
            StaticPersistence.Instance.Delete(id);
            ControlPersistence.Instance.Delete(id);
            CameraPersistence.Instance.Delete(id);
            TargetTrackPersistence.Instance.Delete(id);
            VideoTrackPersistence.Instance.Delete(id);
            AnalyzePersistence.Instance.Delete(id);
            DevicePersistence.Instance.Delete(id);
            return Ok("删除视频信息成功");
        }
    }
}
