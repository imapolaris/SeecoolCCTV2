using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd.H264
{
    public class Nalu
    {
        /// <summary>
        /// NALU的起始字节。
        /// </summary>
        public static byte[] StartCode
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x01 }; }
        }

        private Nalu()
        {

        }

        public Nalu(NaluHeader header, byte[] payload)
        {
            Header = header;
            Payload = payload;
            buildData();
        }

        private byte[] buildData()
        {
            byte[] bytes = new byte[Payload.Length + 1];
            bytes[0] = Header.ToByte();
            Array.Copy(Payload, 0, bytes, 1, Payload.Length);
            return bytes;
        }

        public static Nalu Parse(byte[] data)
        {
            int sIndex = getStartCodeLen(data);
            byte[] ndata = BytesHelper.SubBytes(data, sIndex);
            Nalu a = new Nalu();
            a.Header = NaluHeader.Parse(ndata[0]);
            a.Payload = BytesHelper.SubBytes(ndata, 1);
            return a;
        }

        private static int getStartCodeLen(byte[] data)
        {
            if (data.Length < 3)
                return 0;
            if (data[0] == 0 && data[1] == 0)
            {
                if (data[2] == 0 && data[3] == 1)
                    return 4;
                else if (data[2] == 1)
                    return 3;
            }
            return 0;
        }

        /// <summary>
        /// NALU头信息。
        /// </summary>
        public NaluHeader Header { get; private set; }
        /// <summary>
        /// 负载数据。
        /// </summary>
        public byte[] Payload { get; private set; }
        public int PayloadLen { get { return Payload.Length; } }
        /// <summary>
        /// 总字节长度。
        /// </summary>
        public int TotalBytes { get { return Payload.Length + 1; } }

        public byte[] BytesWithStartCode()
        {
            byte[] nData = buildData();
            byte[] bytes = new byte[nData.Length + 4];
            bytes[0] = 0x00;
            bytes[1] = 0x00;
            bytes[2] = 0x00;
            bytes[3] = 0x01;
            Array.Copy(nData, 0, bytes, 4, nData.Length);
            return bytes;
        }

        public byte[] NaluBytes()
        {
            return buildData();

        }
    }
}
