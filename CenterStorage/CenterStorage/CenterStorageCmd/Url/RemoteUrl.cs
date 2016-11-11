using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenterStorageCmd.Url
{
    public class RemoteUrl : UrlBase, IRemoteUrl
    {
        static string TimeHeader = "time=";
        public DateTime BeginTime { get; private set; }

        public DateTime EndTime { get; private set; }
        static string SourceHeader = "source=";
        public string SourceIp { get; private set; }

        public int SourcePort { get; private set; }

        private const string IpRegex = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";
        public RemoteUrl(string ip, int port, DateTime begin, DateTime end, IVideoInfo[] videos, string path)
            :base(path, videos, "REMOTE")
        {
            SourceIp = ip;
            SourcePort = port;
            BeginTime = begin;
            EndTime = end;
        }

        public override void CheckValid()
        {
            base.CheckValid();
            if (!checkIp(SourceIp))
                throw new InvalidOperationException("IP地址格式错误。");
            if (!checkPort(SourcePort))
                throw new InvalidOperationException("端口号错误。");
            if (EndTime <= BeginTime)
                throw new InvalidOperationException("开始时间应小于结束时间。");
        }

        public static IUrl Parse(string url)
        {
            RemoteUrl remoteUrl = new RemoteUrl(null, 0, DateTime.MinValue,DateTime.MinValue,null,null);
            return remoteUrl.parse(url);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UrlHeader).Append(SourceType).Append("/");
            sb.Append(TimeHeader).Append(toString(BeginTime)).Append("-").Append(toString(EndTime)).Append("/");

            sb.Append(SourceHeader).Append(SourceIp).Append(":").Append(SourcePort).Append("/");

            string path = GetPath();
            if (!string.IsNullOrWhiteSpace(path))
                sb.Append(path).Append("/");

            string videos = GetVideoInfosString();
            if (!string.IsNullOrWhiteSpace(videos))
                sb.Append(videos);
            return sb.ToString();
        }
        
        protected override bool Update(string str)
        {
            if (base.Update(str))
                return true;
            if (!setSource(str))
                if (!setTime(str))
                    return false;
            return false;
        }

        bool setTime(string str)
        {
            if (str.StartsWith(TimeHeader))
            {
                str = str.Substring(TimeHeader.Length);
                string[] times = str.Split('-');
                BeginTime = toTime(times[0]);
                EndTime = toTime(times[1]);
                return true;
            }
            return false;
        }

        bool setSource(string str)
        {
            if (str.StartsWith(SourceHeader))
            {
                str = str.Substring(SourceHeader.Length);
                string[] ipport = str.Split(':');
                SourceIp = ipport[0];
                SourcePort = int.Parse(ipport[1]);
                return true;
            }
            return false;
        }

        private bool checkIp(string ip)
        {
            Regex reg = new Regex(IpRegex);
            return reg.IsMatch(ip);
        }

        private bool checkPort(int port)
        {
            Console.WriteLine("Port: {0}", port);
            return port > 100 && port < 65535;
        }

        private static string toString(DateTime time)
        {
            return (time.Ticks / 10000000).ToString();
        }

        private static DateTime toTime(string time)
        {
            return new DateTime(long.Parse(time) * 10000000);
        }
    }
}
