using Seecool.RemoteCall;
using Seecool.RemoteCall.PubSub;
using Seecool.RemoteCall.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Routing;
using VideoTrackingCmd;

namespace VideoNS.VideoTrack
{
    public class TrackingDataSwapClient : IDisposable
    {
        HttpClient _webApiClient;
        ZmqSubClient<string> _subStatus;
        public Action<TrackingStatusEnum, string> TrackingStatusEvent;
        public TrackingDataSwapClient(string host, string subPort, string rpcPort)
        {
            try
            {
                _webApiClient = new HttpClient();
                _webApiClient.BaseAddress = new Uri(string.Format("http://{0}:{1}", host, int.Parse(rpcPort)));
                _webApiClient.Timeout = TimeSpan.FromSeconds(10);
                string strTrack = string.Format("tcp://{0}:{1}", host, subPort);
                _subStatus = new ZmqSubClient<string>(strTrack, new JsonFormatter(), onStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static string _baseRequestUri = "api/track";

        string formatRequest(string action, RouteValueDictionary paramDict = null)
        {
            string paramString = formatPara(paramDict);
            if (string.IsNullOrEmpty(action))
                return _baseRequestUri + paramString;
            else
                return _baseRequestUri + '/' + action + paramString;
        }

        static string formatPara(RouteValueDictionary dict)
        {
            if (dict == null || dict.Count == 0)
                return string.Empty;
            else
                return "?" + string.Join("&", dict.Select(i => i.Key + "=" + i.Value));
        }

        public bool IsTracking
        {
            get
            {
                var response = _webApiClient.GetAsync(formatRequest("IsRunning")).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<bool>().Result;
            }
        }

        public void ResetPtz()
        {
            _webApiClient.PostAsync(formatRequest("ResetPtz"), null);
        }

        public void StartDrag(double x, double y)
        {
            _webApiClient.PostAsync(formatRequest("StartDrag", new RouteValueDictionary { { "x", x }, { "y", y } }), null);
        }

        public void Draging(double x, double y)
        {
            _webApiClient.PostAsync(formatRequest("Draging", new RouteValueDictionary { { "x", x }, { "y", y } }), null);
        }

        public void StopDrag()
        {
            _webApiClient.PostAsync(formatRequest("StopDrag"), null);
        }

        public void SetZoomScale(double scale)
        {
            _webApiClient.PostAsync(formatRequest("SetZoomScale", new RouteValueDictionary { { "scale", scale } }), null);
        }

        public void StartTrackFromPoint(int frameStamp, PointF pt)
        {
            _webApiClient.PostAsync(formatRequest("StartTrackFromPoint", new RouteValueDictionary { { "frameTime", frameStamp }, { "x", pt.X }, { "y", pt.Y } }), null);
        }

        public void StartTrackFromPoint(PointF pt)
        {
            _webApiClient.PostAsync(formatRequest("StartTrackFromPoint", new RouteValueDictionary { { "x", pt.X }, { "y", pt.Y } }), null);
        }

        public void StartTrackFromRect(int frameStamp, RectangleF rect)
        {
            _webApiClient.PostAsync(formatRequest("StartTrackFromRect", new RouteValueDictionary { { "frameTime", frameStamp }, { "x", rect.X }, { "y", rect.Y }, { "width", rect.Width }, { "height", rect.Height } }), null);
        }

        public void StopTrack()
        {
            _webApiClient.PostAsync(formatRequest("StopTrack"), null);
        }

        public bool IsAutoZoom
        {
            get
            {
                var response = _webApiClient.GetAsync(formatRequest("GetAutoZoom")).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<bool>().Result;
            }
            set
            {
                _webApiClient.PostAsync(formatRequest("SetAutoZoom", new RouteValueDictionary { { "isAutoZoom", value } }), null);
            }
        }


        public double Zoom
        {
            get
            {
                var response = _webApiClient.GetAsync(formatRequest("GetZoom")).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<double>().Result;
            }
            set
            {
                _webApiClient.PostAsync(formatRequest("SetZoom", new RouteValueDictionary { { "zoom", value } }), null);
            }
        }

        public Image GetScreenShot()
        {
            string request = formatRequest("GetScreenShot");
            var response = _webApiClient.GetAsync(request).Result;
            Console.WriteLine(request);
            response.EnsureSuccessStatusCode();
            byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;
            MemoryStream ms = new MemoryStream(bytes);
            return (Bitmap)Image.FromStream(ms);
        }

        private void onStatus(string statusInfo)
        {
            char[] separator = new char[] { '#' };
            string[] strList = statusInfo.Split(separator);
            TrackingStatusEnum statusEnum = TrackingStatusEnum.None;
            string info = statusInfo;
            if (strList.Length >= 2)
            {
                if (strList.Length >= 3)
                    info = strList[2];
                switch (strList[1].ToLower())
                {
                    case "error":
                        statusEnum = TrackingStatusEnum.Error;
                        break;
                    case "warn":
                        statusEnum = TrackingStatusEnum.Warn;
                        break;
                    case "rect":
                        statusEnum = TrackingStatusEnum.Rect;
                        break;
                    case "autozoom":
                        statusEnum = TrackingStatusEnum.AutoZoom;
                        break;
                    case "zoominfo":
                        statusEnum = TrackingStatusEnum.ZoomAndStatus;
                        break;
                    case "info":
                        statusEnum = TrackingStatusEnum.Info;
                        break;
                    case "status":
                        statusEnum = TrackingStatusEnum.Status;
                        info = getStatusInfo(info);
                        break;
                    case "debug":
                        statusEnum = TrackingStatusEnum.Debug;
                        break;
                    case "prompt":
                        statusEnum = TrackingStatusEnum.Prompt;
                        break;
                    default:
                        statusEnum = TrackingStatusEnum.Other;
                        break;
                }
            }
            else
                statusEnum = TrackingStatusEnum.None;
            if (TrackingStatusEvent != null)
                TrackingStatusEvent(statusEnum, info);
        }

        private string getStatusInfo(string statusInfo)
        {
            string info = statusInfo;
            string[] str = statusInfo.Split(',');
            if (str.Length >= 6)
                info = string.Format("特征点数:{0} Scale:{1}({2}) Counts:{3}, SizeFit：{4} 总用时:{5}ms", str[0], str[1], str[2], str[3], str[4], str[str.Length - 1]);
            if (str.Length > 6)
            {
                for (int i = 5; i < str.Length - 1; i++)
                    info += ", " + str[i];
            }
            return info;
        }

        public void Dispose()
        {
            if (_subStatus != null)
                _subStatus.Dispose();
            _subStatus = null;
            if (_webApiClient != null)
                _webApiClient.Dispose();
            _webApiClient = null;
        }
    }
}
