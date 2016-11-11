using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoStreamServer.Model
{
    public class HikvUrlInfo
    {
        private const string urlReg = "^hikv://.+:.+@((?:(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d)))\\.){3}(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d)))):\\d{1,5}/stream\\?channel=\\d{1,3}&profile=[a-zA-Z]{3,4}$";
        public string Ip { get; set; }
        public int Port { get; set; }
        public string  User { get; set; }
        public string Password { get; set; }
        public int Channel { get; set; }
        public bool IsSubStream { get; set; }

        public HikvUrlInfo(string url)
        {
            if (!checkUrlValid(url))
                throw new ArgumentException("不是一个有效的海康视频流URL");
            parse(url);
        }

        private void parse(string url)
        {
            Uri uri = new Uri(url);
            Ip = uri.Host;
            Port = uri.Port;
            string ui = uri.UserInfo;
            string[] ups = ui.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (ups.Length != 2)
                throw new ArgumentException("URL中未包含用户名和密码的完整信息");
            User = ups[0];
            Password = ups[1];

            string[] param = uri.Query.Split(new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            if (param.Length < 2)
                throw new ArgumentException("缺少查询参数");
            bool hasChannel = false;
            bool hasType = false;
            foreach(string p in param)
            {
                if(p.ToLower().StartsWith("channel="))
                {
                    string cValue = p.Substring("channel=".Length);
                    Channel = int.Parse(cValue);
                    hasChannel = true;
                }
                else if (p.ToLower().StartsWith("profile="))
                {
                    IsSubStream = p.ToLower().Substring("profile=".Length).Equals("sub");
                    hasType = true;
                }
            }

            if (!(hasChannel && hasType))
                throw new ArgumentException("缺少码流通道或码流类型参数");
        }

        private bool checkUrlValid(string url)
        {
            Regex reg = new Regex(urlReg);
            return reg.IsMatch(url);
        }
    }
}
