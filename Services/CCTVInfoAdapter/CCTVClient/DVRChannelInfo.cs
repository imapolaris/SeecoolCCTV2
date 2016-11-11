using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
   public class DVRChannelInfo
    {
        public enum DVRType { Unknown, HikVision, USNT };

        public DVRType Type = DVRType.Unknown;
        public string Host = "127.0.0.1";
        public int Port = 8000;
        public string User = "admin";
        public string Pass = "12345";
        public int Channel = 1;
    }
}
