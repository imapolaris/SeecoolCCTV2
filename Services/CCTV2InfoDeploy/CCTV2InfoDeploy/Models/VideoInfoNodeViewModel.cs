using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace CCTV2InfoDeploy.Models
{
    public class VideoInfoNodeViewModel
    {
        public string NodeId { get; set; }
        public string ParentId { get; set; }
        public CCTVVideoInfoWrap VideoInfo{ get; set; }
    }
}
