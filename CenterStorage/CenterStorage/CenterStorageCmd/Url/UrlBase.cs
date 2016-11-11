using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd.Url
{
    public class UrlBase : IUrl
    {
        public static readonly string UrlHeader = "CCTV2:";
        public string SourceType { get; private set; } = "BASE";
        static string LocalPathHeader = "path=";
        public string LocalPath { get; private set; }
        static string VideosHeader = "videos=";
        public IVideoInfo[] VideoInfos { get; private set; }
        public UrlBase(string localPath, IVideoInfo[] videoInfo, string sourceType)
        {
            LocalPath = localPath;
            VideoInfos = videoInfo;
            SourceType = sourceType;
        }

        protected static string urlHeaderAndRemove(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("未配置下载链接。");
            if (!url.StartsWith(UrlHeader))
                throw new InvalidOperationException("获取信息失败:无效的起始标记。");
            return url.Substring(UrlHeader.Length);
        }

        protected IUrl parse(string urlString)
        {
            urlString = urlHeaderAndRemove(urlString);
            if (!urlString.StartsWith(SourceType))
                return null;

            string[] strs = urlString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            LocalUrl localUrl = new LocalUrl(null, null);
            foreach (var str in strs)
                Update(str);
            CheckValid();
            return this;
        }

        public virtual void CheckValid()
        {
        }

        protected string GetVideoInfosString()
        {
            if (VideoInfos == null || VideoInfos.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            sb.Append(VideosHeader);
            foreach (VideoInfo vi in VideoInfos)
            {
                sb.Append(getVideoInfoString(vi));
                sb.Append('|');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        protected string GetPath()
        {
            if(!string.IsNullOrWhiteSpace(LocalPath))
                return string.Format("{0}{1}", LocalPathHeader, LocalPath);
            return null;
        }

        protected virtual bool Update(string str)
        {
            if(!setVideoInfos(str))
                if (!setLocalPath(str))
                return false;
            return true;
        }

        bool setVideoInfos(string str)
        {
            if (str.StartsWith(VideosHeader))
            {
                str = str.Substring(VideosHeader.Length);
                char[] separator = new char[] { '|' };
                string[] vStrs = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                List<VideoInfo> viList = new List<VideoInfo>();
                foreach (string vStr in vStrs)
                {
                    try
                    {
                        string[] temp = vStr.Split(',');
                        string name = temp.Length > 2 ? temp[2] : null;
                        int streamId = int.Parse(temp[1]);
                        if (streamId < 0 || streamId > 100)
                            throw new InvalidOperationException($"无效的码流！ \"{vStr}\"");
                        VideoInfo vi = new VideoInfo(temp[0], streamId, name);
                        viList.Add(vi);
                    }
                    catch (InvalidOperationException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException($"视频部分解析错误！\"{vStr}\"");
                    }
                }
                VideoInfos = viList.ToArray();
                return true;
            }
            return false;
        }

        bool setLocalPath(string str)
        {
            if (str.StartsWith(LocalPathHeader))
            {
                LocalPath = str.Substring(LocalPathHeader.Length);
                return true;
            }
            return false;
        }

        private static string getVideoInfoString(VideoInfo vi)
        {
            if(string.IsNullOrEmpty(vi.VideoName))
                return string.Format("{0},{1}", vi.VideoId, vi.StreamId);
            else
                return string.Format("{0},{1},{2}", vi.VideoId, vi.StreamId, vi.VideoName);
        }
    }
}
