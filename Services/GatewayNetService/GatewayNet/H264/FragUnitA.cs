using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayNet.Util;

namespace GatewayNet.H264
{
    /// <summary>
    /// 分片封包模式FU-A；
    /// </summary>
    public class FragUnitA
    {
        private const int NALU_TYPE = NaluTypes.FU_A;

        /// <summary>
        /// FU标记指示字节。
        /// </summary>
        public NaluHeader Indicator { get; private set; }
        /// <summary>
        /// FU头信息。
        /// </summary>
        public FuHeader Header { get; private set; }
        /// <summary>
        /// FU负载数据。
        /// </summary>
        public byte[] Payload { get; private set; }
        public int PayloadLen { get { return Payload.Length; } }
        /// <summary>
        /// 总字节长度。
        /// </summary>
        public int TotalBytes { get { return Payload.Length + 2; } }

        private FragUnitA()
        {

        }

        public FragUnitA(NaluHeader indicator, FuHeader header, byte[] payload)
        {
            Indicator = indicator;
            if (indicator.Type != NALU_TYPE)
                throw new ArgumentException($"FU-B封包的指示字节的类型必须是{NALU_TYPE}", nameof(indicator));
            Header = header;
            Payload = payload;
        }

        public static FragUnitA Parse(byte[] data)
        {
            FragUnitA fu = new FragUnitA();
            fu.Indicator = NaluHeader.Parse(data[0]);
            if (fu.Indicator.Type != NALU_TYPE)
                throw new ArgumentException($"FU-B封包的指示字节的类型必须是{NALU_TYPE}，当前类型是:" + fu.Indicator.Type);
            fu.Header = FuHeader.Parse(data[1]);
            fu.Payload = BytesHelper.SubBytes(data, 2);
            return fu;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Payload.Length + 2];
            bytes[0] = Indicator.ToByte();
            bytes[1] = Header.ToByte();
            Array.Copy(Payload, 0, bytes, 2, Payload.Length);
            return bytes;
        }

        public Nalu ToNalu()
        {
            byte[] np = new byte[PayloadLen + 1];
            np[0] = Header.ToByte();
            Array.Copy(Payload, 0, np, 1, Payload.Length);
            return new Nalu(Indicator, np);
        }
    }
}
