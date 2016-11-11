using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd.H264
{
    public class NaluHeader
    {
        /// <summary>
        /// 如果有语法冲突，则为 True。当网络识别此单元存在比特错误时，可将其设为 True，以便接收方丢掉该单元。 
        /// </summary>
        public bool Forbidden { get; private set; }
        /// <summary>
        /// 取值为0,1,2,3;
        /// 用来指示该NALU 的重要性等级。值越大，表示当前NALU越重要。具体大于0 时取何值，没有具体规定。
        /// </summary>
        public int NRI { get; private set; }
        /// <summary>
        /// 指出NALU 的类型。
        /// 0     没有定义
        /// 1-23 NAL单元 单个 NAL 单元包.
        /// 24    STAP-A 单一时间的组合包
        /// 25    STAP-B 单一时间的组合包
        /// 26    MTAP16 多个时间的组合包
        /// 27    MTAP24 多个时间的组合包
        /// 28    FU-A 分片的单元
        /// 29    FU-B 分片的单元
        /// 30-31 没有定义
        /// </summary>
        public int Type { get; private set; }

        private byte _data;
        private NaluHeader()
        {

        }
        public NaluHeader(bool f, int nri, int type)
        {
            Forbidden = f;
            NRI = nri;
            Type = type;
            buildData();
        }

        public static NaluHeader Parse(byte data)
        {
            NaluHeader a = new NaluHeader();
            a.Forbidden = ((data & 0x80) >> 7) == 1;
            a.NRI = (data & 0x60) >> 5;
            a.Type = (data & 0x1F);
            a._data = data;
            return a;
        }

        private void buildData()
        {
            int t = ((Forbidden ? 1 : 0) & 0x01) << 7;
            t |= (NRI & 0x03) << 5;
            t |= (Type & 0x1F);
            _data = (byte)t;
        }

        public byte ToByte()
        {
            return _data;
        }

        public override string ToString()
        {
            return $"F:{Forbidden},NRI:{NRI},Type:{Type}";
        }
    }
}
