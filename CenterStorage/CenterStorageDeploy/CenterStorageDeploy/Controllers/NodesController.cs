using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTVModels;
using CenterStorageCmd;
using CenterStorageDeploy.Managers;
using CenterStorageDeploy.Models;
using CenterStorageDeploy.Util;

namespace CenterStorageDeploy.Controllers
{
    public class NodesController : ApiController
    {
        public HttpResponseMessage GetStorageInfos()
        {
            Console.WriteLine("Request:GetStorageInfos:" + DateTime.Now);
            try
            {
                CCTVHierarchyNode root = NodesManager.Instance.GetHierarchyRoot();
                IVideoInfo[] videos = StorageManager.Instance.GetAllStorageVideos();

                Dictionary<string, VideoStorageInfo> dictInfos = new Dictionary<string, VideoStorageInfo>();
                VideoStorageInfo sInfo = TransferModel(root, dictInfos);
                UpdateStorageFlag(dictInfos, videos);

                var response = Request.CreateResponse(sInfo);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:GetStorageInfos:" + DateTime.Now + ":" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e);
            }
        }

        private VideoStorageInfo TransferModel(CCTVHierarchyNode node, Dictionary<string, VideoStorageInfo> sInfos)
        {
            if (node != null)
            {
                VideoStorageInfo vsi = new VideoStorageInfo()
                {
                    VideoId = node.Id,
                    VideoName = node.Name,
                    Type = node.Type.ToString(),
                    StorageOn = false
                };
                sInfos[vsi.VideoId] = vsi;
                if (node.Children != null)
                {
                    List<VideoStorageInfo> children = new List<VideoStorageInfo>();
                    for (int i = 0; i < node.Children.Length; i++)
                    {
                        VideoStorageInfo child = TransferModel(node.Children[i], sInfos);
                        if (child != null)
                            children.Add(child);
                    }
                    vsi.Children = children.ToArray();
                }
                return vsi;
            }
            return null;
        }

        private void UpdateStorageFlag(Dictionary<string, VideoStorageInfo> sInfos, IVideoInfo[] videos)
        {
            if (videos != null)
            {
                foreach (IVideoInfo vi in videos)
                    if (sInfos.ContainsKey(vi.VideoId))
                        sInfos[vi.VideoId].StorageOn = true;
            }
        }

        public IHttpActionResult PostStorage(string videoId, bool flag)
        {
            Console.WriteLine("Request:PostStorage:" + DateTime.Now);
            try
            {
                StorageManager.Instance.SetStorageFlag(videoId, ConstSettings.DefaultStreamId, flag);
                return Ok("修改成功!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:PostStorage:" + DateTime.Now + ":" + e.Message);
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                return InternalServerError(e);
            }
        }
    }
}
