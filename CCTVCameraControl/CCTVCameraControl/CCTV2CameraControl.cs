using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CCTVCameraControl.Base;

namespace CCTVCameraControl
{
    public class CCTV2CameraControl : ICameraControl
    {
        string _api;
        string _videoId;
        public CCTV2CameraControl(string ip, int port, string videoId)
        {
            _api = string.Format($"http://{ip}:{port}/api");
            _videoId = videoId;
        }

        public void Control(CameraAction action, int actData)
        {
            string uri = _api + @"/ptzcontrol/control?videoid=" + _videoId + "&action=" + (int)action + "&data=" + actData;
            dooGet(uri);
        }

        public void TrackTarget(string targetUid, double longitude, double latitude, double altitude, double width)
        {
            string uri = _api + @"/ptzcontrol/lonlatcontrol?videoid=" + _videoId
                + "&targetId=" + targetUid + "&lon=" + longitude + "&lat=" + latitude + "&alt=" + altitude + "width=" + width;
            trydoGet(uri);
        }

        public void StopTrack()
        {
        }


        private void trydoGet(string uri)
        {
            try
            {
                dooGet(uri);
            }
            catch
            { }
        }

        private async void dooGet(string uri)
        {
            //创建HttpClient（注意传入HttpClientHandler）
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler))
            {
                //await异步等待回应
                var response = http.GetAsync(uri).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
        }
    }
}
