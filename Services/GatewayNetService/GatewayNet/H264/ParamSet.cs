using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.H264
{
    public class ParamSet
    {
        public Nalu SPS { get; set; }
        public Nalu PPS { get; set; }

        public bool IsValid
        {
            get
            {
                return SPS != null && PPS != null;
            }
        }

        public byte[] AllBytes()
        {
            if (IsValid)
                return SPS.BytesWithStartCode().Concat(PPS.BytesWithStartCode()).ToArray();
            return null;
        }
    }
}
