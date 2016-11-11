using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Rtsp.Server.MediaTypes
{
    //public class RFC6184FrameExpand: RFC6184Media.RFC6184Frame
    //{
    //    public RFC6184FrameExpand(byte payloadType, byte[] pps, byte[] sps)
    //        :base(payloadType)
    //    {
    //        if (pps != null && sps != null)
    //        {

    //        }
    //    }

    //    //public override void Packetize(byte[] nal, int mtu = 1500, int? DON = null) //sequenceNumber
    //    //{
    //    //    if (nal == null) return;

    //    //    int nalLength = nal.Length;

    //    //    int offset = 0;

    //    //    if (nalLength >= mtu)
    //    //    {
    //    //        //Consume the original header and move the offset into the data
    //    //        byte nalHeader = nal[offset++],
    //    //            nalFNRI = (byte)(nalHeader & 0xE0), //Extract the F and NRI bit fields
    //    //            nalType = (byte)(nalHeader & Common.Binary.FiveBitMaxValue), //Extract the Type
    //    //            fragmentType = (byte)(DON.HasValue ? Media.Codecs.Video.H264.NalUnitType.FragmentationUnitB : Media.Codecs.Video.H264.NalUnitType.FragmentationUnitA),
    //    //            fragmentIndicator = (byte)(nalFNRI | fragmentType);//Create the Fragment Indicator Octet

    //    //        //Store the nalType contained
    //    //        m_ContainedNalTypes.Add(nalType);

    //    //        //Determine if the marker bit should be set.
    //    //        bool marker = Media.Codecs.Video.H264.NalUnitType.IsAccessUnit(ref nalType);//false; //(nalType == Media.Codecs.Video.H264.NalUnitType.AccessUnitDelimiter);

    //    //        //Get the highest sequence number
    //    //        int highestSequenceNumber = HighestSequenceNumber;

    //    //        //Consume the bytes left in the nal
    //    //        while (offset < nalLength)
    //    //        {
    //    //            //Get the data required which consists of the fragmentIndicator, Constructed Header and the data.
    //    //            IEnumerable<byte> data;

    //    //            //Build the Fragmentation Header

    //    //            //First Packet
    //    //            if (offset == 1)
    //    //            {
    //    //                //FU (A/B) Indicator with F and NRI
    //    //                //Start Bit Set with Original NalType

    //    //                data = Enumerable.Concat(Media.Common.Extensions.Linq.LinqExtensions.Yield(fragmentIndicator), Media.Common.Extensions.Linq.LinqExtensions.Yield(((byte)(0x80 | nalType))));
    //    //            }
    //    //            else if (offset + mtu > nalLength)
    //    //            {
    //    //                //End Bit Set with Original NalType
    //    //                data = Enumerable.Concat(Media.Common.Extensions.Linq.LinqExtensions.Yield(fragmentIndicator), Media.Common.Extensions.Linq.LinqExtensions.Yield(((byte)(0x40 | nalType))));

    //    //                //This should not be set at the nal level for end of nal units.
    //    //                //marker = true;

    //    //            }
    //    //            else//For packets other than the start or end
    //    //            {
    //    //                //No Start, No End
    //    //                data = Enumerable.Concat(Media.Common.Extensions.Linq.LinqExtensions.Yield(fragmentIndicator), Media.Common.Extensions.Linq.LinqExtensions.Yield(nalType));
    //    //            }

    //    //            //FU - B has DON at the very beginning of each 
    //    //            if (fragmentType == Media.Codecs.Video.H264.NalUnitType.FragmentationUnitB)// && Count == 0)// highestSequenceNumber == 0)
    //    //            {
    //    //                //byte[] DONBytes = new byte[2];
    //    //                //Common.Binary.Write16(DONBytes, 0, Common.Binary.IsLittleEndian, (short)DON);

    //    //                data = Enumerable.Concat(Common.Binary.GetBytes((short)DON, Common.Binary.IsLittleEndian), data);
    //    //            }

    //    //            //Add the data the fragment data from the original nal
    //    //            data = Enumerable.Concat(data, nal.Skip(offset).Take(mtu));

    //    //            //Add the packet using the next highest sequence number
    //    //            Add(new Rtp.RtpPacket(2, false, false, marker, PayloadTypeByte, 0, SynchronizationSourceIdentifier, ++highestSequenceNumber, 0, data.ToArray()));

    //    //            //Move the offset
    //    //            offset += mtu;
    //    //        }
    //    //    } //Should check for first byte to be 1 - 23?
    //    //    else Add(new Rtp.RtpPacket(2, false, false, false, PayloadTypeByte, 0, SynchronizationSourceIdentifier, HighestSequenceNumber + 1, 0, nal));
    //    //}
    //}
}
