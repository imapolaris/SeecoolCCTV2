using FFmpeg;
using Media.Rtp;
using Media.Rtsp;
using Media.Sdp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Seecool.VideoStreamBase;
using System.Threading;

namespace CCTVStreamCmd.Rtsp
{
    public class RtspStream: StreamBase, IStream, IDisposable
    {
        RtspClient _client;
        static byte[] _startCode = new byte[] { 0x00, 0x00, 0x00, 0x01 };
        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        Thread _thread;
        public RtspStream(string url)
        {
            _disposeEvent.Reset();
            _client = new RtspClient(url, RtspClient.ClientProtocolType.Tcp);
            _client.OnConnect += Client_OnConnect;
            _client.OnPlay += Client_OnPlay;
            //_client.Client.RtpPacketReceieved += Client_RtpPacketReceieved;
            _client.Client.RtpFrameChanged += Client_RtpFrameChanged;
            _client.StartPlaying();
            _thread = new Thread(run) { IsBackground = true };
            _thread.Start();
        }

        Queue<List<byte[]>> _buffers = new Queue<List<byte[]>>();
        private void run()
        {
            while (!_disposeEvent.WaitOne(0))
            {
                if (_buffers.Count > 0)
                {
                    if (_buffers.Count > 1)
                        Console.WriteLine("count: " + _buffers.Count);
                    var buffers = _buffers.Dequeue();
                    byte[] packetData = getFrameBuf(buffers);
                    int nalType = packetData[_startCode.Length] & 0x1F;
                    CCTVFrameType streamType = CCTVFrameType.StreamFrame;
                    if (nalType != 0x05) // KEY FRAME
                        streamType = CCTVFrameType.StreamKeyFrame;
                    onStream(packetData, streamType, DateTime.Now);
                    onFrame(packetData, streamType);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void Client_OnConnect(RtspClient sender, object args)
        {
            Console.WriteLine("OnConnect");
            _client.SocketWriteTimeout = Math.Max(_client.SocketWriteTimeout, (int)Math.Round(_client.RtspSessionTimeout.TotalMilliseconds));
            _client.SocketReadTimeout = Math.Max(_client.SocketReadTimeout, (int)Math.Round(_client.RtspSessionTimeout.TotalMilliseconds));
        }

        private void Client_RtpPacketReceieved(object sender, RtpPacket packet)
        {
            //Console.WriteLine("RtpPacketReceieved : " + packet.PayloadType + ", " + packet.Length);
            //Console.WriteLine();
            //string rtspstr = $"rtsp packet {packet.PayloadData.Count()}:";
            //for (int i = 0; i < Math.Min(50, packet.PayloadData.Count()); i++)
            //    rtspstr += string.Format("{0:X2}, ", packet.PayloadData.ElementAt(i));
            //Console.WriteLine(rtspstr);
        }
        private void Client_RtpFrameChanged(object sender, RtpFrame frame)
        //private void Client_RtpFrameChanged(object sender, RtpFrame frame, RtpClient.TransportContext tc, bool final)
        {///基于RTP的H264视频数据打包解包类 http://blog.csdn.net/dengzikun/article/details/5807694
            //Console.WriteLine($"RtpFrameChanged : {frame.IsComplete}, {frame.Count},{frame.Timestamp}, {frame.Created}");
            if (frame.IsComplete && frame.PayloadTypeByte == 96)
            {
                _buffers.Enqueue(frame.Select(_ => _.PayloadData.ToArray()).ToList());
            }
        }
        
        private void Client_OnPlay(RtspClient sender, object args)
        {
            Console.WriteLine("OnPlay");
            byte[] data = getHeaderData();
            if (data != null && data.Length > 0)
                onHeader(new StandardHeaderPacket((int)Constants.AVCodecID.AV_CODEC_ID_H264, data));
        }

        byte[] getHeaderData()
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

        private byte[] getFrameBuf(List<byte[]> buffers)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(_startCode, 0, _startCode.Length);
                for (int i = 0; i < buffers.Count; i++)
                {
                    byte[] data = buffers.ElementAt(i);
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
                            break;
                    }

                    ms.Write(data, skipBytes, data.Length - skipBytes);
                    if (naluType == 7 || naluType == 8)
                        ms.Write(_startCode, 0, _startCode.Length);
                }

                return ms.ToArray();
            }
        }

        public override void Dispose()
        {
            _disposeEvent.Set();
            _thread.Join(500);
            if (_client != null)
                _client.StopPlaying();
            _client = null;
        }
    }
}
