using Media.Rtp;
using Media.Rtsp;
using Media.Sdp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.Rtsp
{
    internal class RtspSource : IDisposable
    {
        RtspClient _client;

        public delegate void OnVideoData(byte[] data);
        public event OnVideoData VideoDataEvent;
        private void fireVideoDataEvent(byte[] data)
        {
            var callback = VideoDataEvent;
            if (callback != null)
                callback(data);
        }

        public RtspSource(string url)
        {
            _client = new RtspClient(url, RtspClient.ClientProtocolType.Tcp);

            _client.OnConnect += _client_OnConnect;
            _client.OnPlay += _client_OnPlay;

            _client.Client.RtpFrameChanged += Client_RtpFrameChanged; ;
        }

        static byte[] _startCode = new byte[] { 0x00, 0x00, 0x00, 0x01 };

        private void Client_RtpFrameChanged(object sender, RtpFrame frame)
        {
            if (frame.IsComplete && frame.PayloadTypeByte == 96)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(_startCode, 0, _startCode.Length);

                    bool haveData = false;
                    foreach (RtpPacket packet in frame)
                        haveData |= writeH264PacketData(ms, packet);

                    if (haveData)
                        fireVideoDataEvent(ms.ToArray());
                }
            }
        }

        bool writeH264PacketData(Stream stream, RtpPacket packet)
        {
            byte[] data = packet.PayloadData.ToArray();

            int skipBytes = 0;
            int naluType = data[0] & 0x1F;
            int frameType = data[0] & 0x60;
            switch (naluType)
            {
                case 24: // STAP-A
                    skipBytes = 1;
                    break;
                case 25:
                case 26:
                case 27: // STAP-B, MTAP16, or MTAP24
                    skipBytes = 3;
                    break;
                case 28:
                case 29: // FU-A or FU-B
                    {
                        bool startPacket = (data[1] & 0x80) != 0;
                        bool endPacket = (data[1] & 0x40) != 0;
                        if (startPacket)
                        {
                            data[1] = (byte)((data[0] & 0xE0) | (data[1] & 0x1F));
                            skipBytes = 1;
                        }
                        else
                        { // The start bit is not set, so we skip both the FU indicator and header:
                            skipBytes = 2;
                        }
                    }
                    break;
                default:
                    return false;
            }

            stream.Write(data, skipBytes, data.Length - skipBytes);
            return true;
        }

        private void _client_OnPlay(RtspClient sender, object args)
        {
            byte[] data = getH264HeaderData();
            if (data != null && data.Length > 0)
                fireVideoDataEvent(data);
        }

        byte[] getH264HeaderData()
        {
            string header = "sprop-parameter-sets=";
            foreach (MediaDescription md in _client.PlayingMedia)
                foreach (SessionDescriptionLine sdl in md)
                    foreach (string part in sdl.Parts)
                    {
                        int index = part.ToLower().IndexOf(header);
                        if (index >= 0)
                        {
                            string values = part.Substring(index + header.Length).Trim();
                            string[] comps = values.Split(',');
                            using (MemoryStream ms = new MemoryStream())
                            {
                                foreach (string comp in comps)
                                {
                                    byte[] bytes = Convert.FromBase64String(comp);
                                    ms.Write(_startCode, 0, _startCode.Length);
                                    ms.Write(bytes, 0, bytes.Length);
                                }
                                return ms.ToArray();
                            }
                        }
                    }
            return null;
        }

        private void _client_OnConnect(RtspClient sender, object args)
        {
            _client.SocketWriteTimeout = Math.Max(_client.SocketWriteTimeout, (int)Math.Round(_client.RtspSessionTimeout.TotalMilliseconds));
            _client.SocketReadTimeout = Math.Max(_client.SocketReadTimeout, (int)Math.Round(_client.RtspSessionTimeout.TotalMilliseconds));
        }

        public void Start()
        {
            _client.StartPlaying();
        }

        public void Stop()
        {
            _client.StopPlaying();
        }

        public void Dispose()
        {
            Stop();
            _client.Dispose();
        }
    }
}
