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
    public class FragUnitB
    {
        private const int NALU_TYPE = NaluTypes.FU_B;
        /// <summary>
        /// FU标记指示字节。
        /// </summary>
        public NaluHeader Indicator { get; private set; }
        /// <summary>
        /// FU头信息。
        /// </summary>
        public FuHeader Header { get; private set; }
        /// <summary>
        /// 解码顺序号。
        /// </summary>
        public short Don { get; private set; }
        /// <summary>
        /// FU负载数据。
        /// </summary>
        public byte[] Payload { get; private set; }
        public int PayloadLen { get { return Payload.Length; } }
        /// <summary>
        /// 总字节长度。
        /// </summary>
        public int TotalBytes { get { return Payload.Length + 4; } }

        private FragUnitB()
        {

        }

        public FragUnitB(NaluHeader indicator, FuHeader header, short don, byte[] payload)
        {
            if (indicator.Type != NALU_TYPE)
                throw new ArgumentException($"FU-B封包的指示字节的类型必须是{NALU_TYPE}", nameof(indicator));
            Indicator = indicator;
            Header = header;
            Don = don;
            Payload = payload;
        }

        public static FragUnitB Parse(byte[] data)
        {
            FragUnitB fu = new FragUnitB();
            fu.Indicator = NaluHeader.Parse(data[0]);
            if (fu.Indicator.Type != NALU_TYPE)
                throw new ArgumentException($"FU-B封包的指示字节的类型必须是{NALU_TYPE}，当前类型是:" + fu.Indicator.Type);
            fu.Header = FuHeader.Parse(data[1]);
            //TODO:我们暂时认为所有的字节序均为BigEndian,如果传输失败，再做改正。
            byte[] donBytes = new byte[] { data[3], data[2] };
            fu.Don = BitConverter.ToInt16(donBytes, 0);
            fu.Payload = BytesHelper.SubBytes(data, 4);
            return fu;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Payload.Length + 4];
            bytes[0] = Indicator.ToByte();
            bytes[1] = Header.ToByte();
            //TODO:我们暂时认为所有的字节序均为BigEndian,如果传输失败，再做改正。
            byte[] donBytes = BitConverter.GetBytes(Don);
            bytes[2] = donBytes[1];
            bytes[3] = donBytes[0];
            Array.Copy(Payload, 0, bytes, 4, Payload.Length);
            return bytes;
        }

        public Nalu ToNalu()
        {
            byte[] np = new byte[PayloadLen + 3];
            np[0] = Header.ToByte();
            //TODO:我们暂时认为所有的字节序均为BigEndian,如果传输失败，再做改正。
            byte[] donBytes = BitConverter.GetBytes(Don);
            np[1] = donBytes[1];
            np[2] = donBytes[0];
            Array.Copy(Payload, 0, np, 3, Payload.Length);
            return new Nalu(Indicator, np);
        }
    }
}
