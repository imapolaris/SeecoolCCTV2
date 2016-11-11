using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayNet.H264;
using LumiSoft.Net.RTP;

namespace GatewayNet.Lower
{
    public class RTPHeaderTrigger
    {
        private const int Interval = 8000;

        private Nalu _sps;
        private Nalu _pps;
        private int _spsStamp;
        private int _ppsStamp;

        public bool IsSPSTimeout { get { return Environment.TickCount - _spsStamp >= Interval; } }
        public bool IsPPSTimeout { get { return Environment.TickCount - _ppsStamp >= Interval; } }
        public RTPHeaderTrigger()
        {

        }

        public void Init()
        {
            _spsStamp = _ppsStamp = Environment.TickCount;
        }

        public void Update(Nalu nal)
        {
            if (nal.Header.Type == NaluTypes.SPS)
            {
                _spsStamp = Environment.TickCount;
                _sps = nal;
            }
            else if (nal.Header.Type == NaluTypes.PPS)
            {
                _ppsStamp = Environment.TickCount;
                _pps = nal;
            }
        }

        public void SendSPS(RTP_SendStream sender, uint timestamp)
        {
            if (_sps != null)
            {
                RTP_Packet packet = new RTP_Packet();
                packet.Timestamp = timestamp;
                packet.Data = _sps.NaluBytes();
                sender.Send(packet);
                _spsStamp = Environment.TickCount;
            }
        }

        public void SendPPS(RTP_SendStream sender, uint timestamp)
        {
            if (_pps != null)
            {
                RTP_Packet packet = new RTP_Packet();
                packet.Timestamp = timestamp;
                packet.Data = _pps.NaluBytes();
                sender.Send(packet);
                _ppsStamp = Environment.TickCount;
            }
        }
    }
}
