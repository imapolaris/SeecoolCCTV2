using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayNet.Util;

namespace GatewayNet.H264
{
    public class RTPPackHelper
    {
        private const int MTU = 1300;

        //public static List<Nalu> ToRTPPayload(Nalu src)
        //{
        //    List<Nalu> nList = new List<Nalu>();
        //    if (src.TotalBytes < MTU)
        //    {
        //        nList.Add(src);
        //    }
        //    else
        //    {
        //        List<byte[]> slices = SplitPayload(src);
        //        NaluHeader indicator = new NaluHeader(false, src.Header.NRI, NaluTypes.FU_A);
        //        for (int i = 0; i < slices.Count; i++)
        //        {
        //            bool start = i == 0;
        //            bool end = i == slices.Count - 1;
        //            FuHeader fuh = new FuHeader(start, end, src.Header.Type);
        //            nList.Add(new FragUnitA(indicator, fuh, slices[i]).ToNalu());
        //        }
        //    }
        //    return nList;
        //}

        //private static List<byte[]> SplitPayload(Nalu src)
        //{
        //    List<byte[]> bList = new List<byte[]>();
        //    int index = 0;
        //    int len = MTU - 1;
        //    while (index < src.PayloadLen)
        //    {
        //        if (index + len > src.PayloadLen)
        //        {
        //            len = src.PayloadLen - index;
        //        }
        //        bList.Add(BytesHelper.SubBytes(src.Payload, index, len));
        //        index += len;
        //    }
        //    return bList;
        //}

        private NaluHeader _header;
        public List<Nalu> ToRTPPayload(PSFragment ps)
        {
            if (ps.IsFrameStart)
                _header = ps.Header;
            if (_header == null)
                return new List<Nalu>();

            List<Nalu> nList = new List<Nalu>();
            if (ps.Data.Length < MTU && ps.IsFrameStart && ps.IsFrameEnd)
            {
                nList.Add(new Nalu(_header, ps.Data));
            }
            else
            {
                List<byte[]> slices = SplitPayload(ps.Data);
                NaluHeader indicator = new NaluHeader(false, _header.NRI, NaluTypes.FU_A);
                for (int i = 0; i < slices.Count; i++)
                {
                    bool start = (i == 0) && ps.IsFrameStart;
                    bool end = (i == slices.Count - 1 && ps.IsFrameEnd);
                    FuHeader fuh = new FuHeader(start, end, _header.Type);
                    nList.Add(new FragUnitA(indicator, fuh, slices[i]).ToNalu());
                }
            }
            return nList;
        }

        private List<byte[]> SplitPayload(byte[] data)
        {
            List<byte[]> bList = new List<byte[]>();
            int index = 0;
            int len = MTU - 1;
            while (index < data.Length)
            {
                if (index + len > data.Length)
                {
                    len = data.Length - index;
                }
                bList.Add(BytesHelper.SubBytes(data, index, len));
                index += len;
            }
            return bList;
        }
    }
}
