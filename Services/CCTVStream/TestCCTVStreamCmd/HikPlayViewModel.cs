using AopUtil.WpfBinding;
using CCTVStreamCmd;
using CCTVStreamCmd.Hikvision;
using FFmpeg;
using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoRender;
using Media.Rtp;
using TestCCTVStreamCmd.H264;
using LumiSoft.Net.RTP;

namespace TestCCTVStreamCmd
{
    class HikPlayViewModel : ObservableObject, IDisposable
    {
        HikStream _hik;
        IRenderSource _renderSource;
        int _width;
        int _height;
        int _curWidth = 0;
        int _curHeight = 0;
        private VideoDecoder _decoder;
        //StreamSocket _socket;
        StreamRtspServer _rtsp;
        private UnpackPS _ups;
        private UnpackPSNew _upsnew;


        [AutoNotify]
        public ImageSource ImageSrc { get; set; }
        public HikPlayViewModel()
        {
            _ups = new UnpackPS();
            _ups.Unpacked += _ups_Unpacked;
            _upsnew = new UnpackPSNew();
            _upsnew.Unpacked += _upsnew_Unpacked;

            _rtsp = new StreamRtspServer();
            //_socket = new StreamSocket();
            _renderSource = new D3DImageSource();
            _renderSource.ImageSourceChanged += onImageSource;
            //_hik = new HikStream("192.168.9.98", 8000, "admin", "12345", 1, false, IntPtr.Zero);
            _hik = new HikStream("192.168.9.155", 8000, "admin", "admin12345", 1, false, IntPtr.Zero);
            _hik.StreamEvent += onHikStream;
            //_hik.RtpFrameEvent += onRtpFrame;
        }

        private void _upsnew_Unpacked(object arg1, PSFragment arg2)
        {
            testSend(arg2);
        }

        RTPPackHelper _helper = new RTPPackHelper();
        private void testSend(PSFragment psf)
        {
            List<Nalu> nList = _helper.ToRTP(psf);
            uint timestamp = 1;
            for (int i = 0; i < nList.Count; i++)
            {
                RTP_Packet packet = new RTP_Packet();
                packet.Timestamp = timestamp;
                packet.Data = nList[i].NaluBytes();
                if (psf.IsFrameEnd && i == nList.Count - 1)
                    packet.IsMarker = true;
                else
                    packet.IsMarker = false;
                testReceive(packet);
            }
        }

        private List<Nalu> _nlist = new List<Nalu>();
        private void testReceive(RTP_Packet packet)
        {
            _nlist.Add(Nalu.Parse(packet.Data));
            if (packet.IsMarker)
            {
                buildFrame();
                _nlist = new List<Nalu>();
            }
        }

        private void buildFrame()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bool isFirst = true;
                for (int i = 0; i < _nlist.Count; i++)
                {
                    Nalu nal = _nlist[i];
                    if (nal.Header.Type < 13)
                        _ups_Unpacked(null, nal);
                    else
                    {
                        //由于我们自己的RTP服务只使用FU_A封包方式发送数据，因此，此处示例我们只解析此封包方式。
                        if (nal.Header.Type == NaluTypes.FU_A)
                        {
                            FragUnitA fua = FragUnitA.Parse(nal.NaluBytes());
                            ////是接收序列的首包，但不是分片的首包，说明接收数据有丢失，无法正常解析。
                            //if (isFirst && !fua.Header.IsStart)
                            //    return;
                            ////是接收序列的末包，但不是分片的末包，说明接收数据有丢失，无法正常解析。
                            //if (i == _nlist.Count - 1 && !fua.Header.IsEnd)
                            //    return;
                            if (isFirst)
                            {
                                Nalu temp = new Nalu(new NaluHeader(false, fua.Indicator.NRI, fua.Header.Type), fua.Payload);
                                ms.Write(temp.NaluBytes(), 0, temp.TotalBytes);
                                isFirst = false;
                            }
                            else
                            {
                                ms.Write(fua.Payload, 0, fua.PayloadLen);
                            }
                        }
                    }
                }
                if (ms.Length > 0)
                    _ups_Unpacked(null, Nalu.Parse(ms.ToArray()));
            }
        }

        private void _ups_Unpacked(object arg1, Nalu arg2)
        {
            if (_decoder == null)
            {
                _decoder = new VideoDecoder();
                var codicId = Constants.AVCodecID.AV_CODEC_ID_H264;
                _decoder.Create(codicId);
            }
            if (arg2.Header.Type == 7 || arg2.Header.Type == 8)
            {
                _decoder.Decode(arg2.BytesWithStartCode(), out _curWidth, out _curHeight);
            }
            else {
                byte[] frameData = _decoder.Decode(arg2.BytesWithStartCode(), out _curWidth, out _curHeight);
                if (frameData != null)
                {
                    if (_curWidth != _width || _curHeight != _height)
                    {
                        _width = _curWidth;
                        _height = _curHeight;
                        _renderSource.SetupSurface(_curWidth, _curHeight);
                    }
                    renderFrame(frameData);
                }
            }
        }

        private void onImageSource()
        {
            ImageSrc = _renderSource?.ImageSource;
        }

        private void onHikStream(IStreamPacket packet)
        {
            //_ups.UpdateStandardStream(packet.Buffer);
            _upsnew.UpdateStandardStream(packet.Buffer);
        }

        void renderFrame(byte[] frame)
        {
            _renderSource?.Render(frame);
        }

        public void Dispose()
        {
            if (_hik != null)
            {
                _hik.StreamEvent -= onHikStream;
                _hik.Dispose();
                _hik = null;
            }

            if (_renderSource != null)
            {
                _renderSource.ImageSourceChanged -= onImageSource;
                _renderSource.Dispose();
                _renderSource = null;
            }

            if (_decoder != null)
            {
                _decoder.Dispose();
                _decoder = null;
            }

            if (_rtsp != null)
            {
                _rtsp.Dispose();
                _rtsp = null;
            }
        }
    }
}