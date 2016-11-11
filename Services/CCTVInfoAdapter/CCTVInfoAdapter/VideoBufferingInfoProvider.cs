using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CCTVInfoAdapter
{
    public class VideoBuffering
    {
        public string Id = string.Empty;
        public int StreamIndex = 0;
    };

    public class VideoBufferingInfoProvider
    {
        public static readonly VideoBufferingInfoProvider Instance = new VideoBufferingInfoProvider();

        private VideoBufferingInfoProvider()
        {
        }

        public VideoBuffering[] GetVideoBufferingInfo()
        {
            using (FileStream fs = new FileStream("VideoBuffering.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<VideoBuffering[]>(json);
            }
        }
    }
}
