using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.H264
{
    public class FuHeader
    {
        /// <summary>
        /// 当设置成True,开始位指示分片NAL单元的开始。当跟随的FU荷载不是分片NAL单元荷载的开始，开始位设为False。
        /// </summary>
        public bool IsStart { get; private set; }
        /// <summary>
        /// 当设置成True, 结束位指示分片NAL单元的结束，即, 荷载的最后字节也是分片NAL单元的最后一个字节。当跟随的
        /// FU荷载不是分片NAL单元的最后分片,结束位设置为False。
        /// </summary>
        public bool IsEnd { get; private set; }
        /// <summary>
        /// 保留位 只能为False，接收者必须忽略该位。
        /// </summary>
        public bool Retain { get; private set; } = false;
        /// <summary>
        /// NAL单元荷载类型定义1~12.
        /// </summary>
        public int Type { get; private set; }

        private byte _data;
        private FuHeader()
        {
        }

        public FuHeader(bool start, bool end, int type)
        {
            IsStart = start;
            IsEnd = end;
            Type = type;
            buildData();
        }

        public static FuHeader Parse(byte data)
        {
            FuHeader fh = new FuHeader();
            fh.IsStart = ((data & 0x80) >> 7) == 1;
            fh.IsEnd = ((data & 0x40) >> 6) == 1;
            fh.Type = (data & 0x1F);
            fh._data = data;
            return fh;
        }

        private void buildData()
        {
            int t = ((IsStart ? 1 : 0) & 0x01) << 7;
            t |= ((IsEnd ? 1 : 0) & 0x01) << 6;
            t |= ((Retain ? 1 : 0) & 0x01) << 5;
            t |= (Type & 0x1F);
            _data = (byte)t;
        }

        public byte ToByte()
        {
            return _data;
        }

        public override string ToString()
        {
            return $"Start:{IsStart},End:{IsEnd},Type:{Type}";
        }
    }
}
