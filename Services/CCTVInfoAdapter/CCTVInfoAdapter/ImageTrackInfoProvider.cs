using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CCTVInfoAdapter
{
    public class ImageTrackInfoProvider
    {
        public static readonly ImageTrackInfoProvider Instance = new ImageTrackInfoProvider();

        class ImageTrackInfo
        {
            public ulong Id = 0;
            public string Host = string.Empty;
        }
        Dictionary<ulong, ImageTrackInfo> _infoDict = new Dictionary<ulong, ImageTrackInfo>();

        private ImageTrackInfoProvider()
        {
            using (FileStream fs = new FileStream("ImageTrack.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                string json = sr.ReadToEnd();
                _infoDict = JsonConvert.DeserializeObject<ImageTrackInfo[]>(json).ToDictionary(x => x.Id);
            }
        }

        public string GetImageTrackHost(ulong id)
        {
            ImageTrackInfo info;
            if (_infoDict.TryGetValue(id, out info))
                return info.Host;
            return null;
        }
    }
}
