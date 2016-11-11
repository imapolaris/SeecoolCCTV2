using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHikToFfmpeg
{
    public class VideoInfo: ObservableObject
    {
        [AutoNotify]
        public string Ip { get; set; } = "192.168.9.155";
        [AutoNotify]
        public ushort Port { get; set; } = 8000;
        [AutoNotify]
        public string UserName { get; set; } = "admin";
        [AutoNotify]
        public string Password { get; set; } = "admin12345";
        [AutoNotify]
        public int Channel { get; set; } = 1;
        [AutoNotify]
        public bool IsSub { get; set; } = false;
        [AutoNotify]
        public bool IsEnabled { get; set; } = true;
    }
}
