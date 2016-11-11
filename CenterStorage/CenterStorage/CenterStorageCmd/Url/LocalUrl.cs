using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd.Url
{
    public class LocalUrl : UrlBase, ILocalUrl
    {
        public LocalUrl(string localPath, VideoInfo[] videoInfo)
            :base(localPath, videoInfo, "LOCAL")
        {
        }

        public static IUrl Parse(string urlString)
        {
            LocalUrl localUrl = new LocalUrl(null, null);
            return localUrl.parse(urlString);
        }

        public override void CheckValid()
        {
            base.CheckValid();
            if (string.IsNullOrWhiteSpace(LocalPath))
                throw new InvalidOperationException("未正确配置视频路径。");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UrlHeader).Append(SourceType).Append("/");
            string path = GetPath();
            if(!string.IsNullOrWhiteSpace(path))
                sb.Append(path).Append("/");

            string videos = GetVideoInfosString();
            if (!string.IsNullOrWhiteSpace(videos))
                sb.Append(videos);
            return sb.ToString();
        }
    }
}
