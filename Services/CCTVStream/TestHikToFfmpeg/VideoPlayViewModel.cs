using AopUtil.WpfBinding;
using CCTVClient;
using CCTVStreamCmd;
using CCTVStreamCmd.Hikvision;
using Common.Util;
using FFmpeg;
using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using VideoRender;

namespace TestHikToFfmpeg
{
    public class VideoPlayViewModel: ObservableObject, IDisposable
    {
        HikStream _hik;
        private VideoDecoder _decoder;
        IRenderSource _renderSource;
        StreamHikToStandard _hikToStand;

        public VideoInfo VideoInfo { get; set; }
        public ICommand PlayCmd { get; set; }
        public ICommand StopCmd { get; set; }
        public ICommand TransformCmd { get; set; }
        [AutoNotify]
        public ImageSource ImageSrc { get; set; }
        [AutoNotify]
        public Stretch StretchMode { get; set; } = Stretch.Uniform;
        bool _isTransform;
        [AutoNotify]
        public string MessageInfo { get; set; }
        object _objLock = new object();
        public VideoPlayViewModel()
        {
            VideoInfo = new VideoInfo();
            PlayCmd = new Common.Command.DelegateCommand(_=>play());
            StopCmd = new Common.Command.DelegateCommand(_=>stop());
            TransformCmd = new Common.Command.DelegateCommand(_ => transform());
        }

        private void play()
        {
            _isTransform = false;
            startPlay();
        }

        private void transform()
        {
            _isTransform = true;
            //_hikToStand = new StreamHikToStandard();
            //_hikToStand.HeaderEvent += onStandardHeader;
            //_hikToStand.StreamEvent += onStandardStream;
            startPlay();
        }

        private void startPlay()
        {
            try
            {
                stop();
                MessageInfo = null;
                VideoInfo.IsEnabled = false;
                initRenderSource();
                initHikSource();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                VideoInfo.IsEnabled = true;
            }
        }

        private void stop()
        {
            VideoInfo.IsEnabled = true;
            disposeHikSource();
            disposeHikDecoder();
            if (_decoder != null)
                _decoder.Dispose();
            _decoder = null;
            disposeRenderSource();
            ImageSrc = null;
        }

        private void initHikSource()
        {
            _hik = new HikStream(VideoInfo.Ip, VideoInfo.Port, VideoInfo.UserName, VideoInfo.Password, VideoInfo.Channel, VideoInfo.IsSub, IntPtr.Zero);
            _hik.HeaderEvent += onHeader;
            _hik.StreamEvent += onStream;
        }

        private void disposeHikSource()
        {
            if (_hik != null)
            {
                _hik.HeaderEvent -= onHeader;
                _hik.StreamEvent -= onStream;
                _hik.Dispose();
            }
            _hik = null;
        }

        private void onHeader(IHeaderPacket packet)
        {
            lock (_objLock)
            {
                checkStyleChanged(packet);
                if (_isTransform)
                {
                    _decoder = new VideoDecoder();
                    _decoder.Create(Constants.AVCodecID.AV_CODEC_ID_H264);
                }
                else
                {
                    initHikDecoder((packet as HikHeaderPacket).Buffer);
                }
            }
        }

        private void checkStyleChanged(IHeaderPacket packet)
        {
            var buffer = (packet as HikHeaderPacket)?.Buffer;
            if (buffer != null && buffer.Length > 4)
            {
                if (buffer[0] == 0x34 && buffer[1] == 0x48 && buffer[2] == 0x4B && buffer[3] == 0x48)//ascii: 4HKH(旧设备)
                    MessageInfo = "设备不支持海康码流转为Ffmpeg.VideoDecoder格式";
                else if (buffer[0] == 0x49 && buffer[1] == 0x4D && buffer[2] == 0x4B && buffer[3] == 0x48)
                    MessageInfo = "设备支持海康码流转为Ffmpeg.VideoDecoder格式";
                else
                    MessageInfo = "其它设备类型，请通过验证判断！" + BitConverter.ToString(buffer);
            }
            else
                MessageInfo = "视频包头错误.";
        }

        private void onStream(IStreamPacket packet)
        {
            if (_isTransform)
                updateFromH264(packet);
            else
                _hikDecoder?.InputData(1, packet.Buffer);
        }

        private void updateFromH264(IStreamPacket packet)
        {
            byte[] buffer = getVideoDecoderBuffer(packet.Buffer);
            if (buffer != null && buffer.Length > 0)
            {
                int width = 0;
                int height = 0;
                byte[] frameData = _decoder?.Decode(buffer, out width, out height);
                if (frameData != null)
                {
                    onDisplay(frameData, width, height);
                    MessageInfo = "设备支持海康码流转为Ffmpeg.VideoDecoder格式";
                }
            }
        }

        MemoryStream _ms = new MemoryStream();
        private byte[] getVideoDecoderBuffer(byte[] buffer)
        {
            if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 1)
            {
                if (buffer[3] == 0xE0)
                {
                    int length = ((int)buffer[4] * 256) + buffer[5];
                    if (length == buffer.Length - 6)
                    {
                        int start = 8 + buffer[8];
                        _ms.Write(buffer, start + 1, buffer.Length - start - 1);
                        if (buffer[6] == 0x8C && buffer[7] == 0x80)//开始
                        {
                        }
                        else if (buffer[6] == 0x88 && buffer[7] == 00)
                        {
                            if (buffer[start] == 0xFA)//结束标记
                                return getStreamBuffer();
                        }
                        if (buffer.Length != 0x1400)
                            return getStreamBuffer();
                    }
                }
            }
            return null;
        }

        byte[] getStreamBuffer()
        {
            byte[] buf = _ms.ToArray();
            _ms = new MemoryStream();
            return buf;
        }

        #region 海康视频流格式转化

        //private void onStandardHeader(IHeaderPacket obj)
        //{
        //    var header = obj as StandardHeaderPacket;
        //    if (header != null)
        //    {
        //        if (_decoder == null)
        //            _decoder = new VideoDecoder();
        //        var codicId = (Constants.AVCodecID)header.CodecID;
        //        _decoder.Create(codicId);
        //        if (header.Buffer != null)
        //            _decoder.Decode(header.Buffer, out _curWidth, out _curHeight);
        //        Console.WriteLine(codicId + $"  {_curWidth} * {_curHeight}");
        //    }
        //}

        //private void onStandardStream(IStreamPacket obj)
        //{
        //    if (_decoder != null)
        //    {
        //        byte[] frameData = _decoder.Decode(obj.Buffer, out _curWidth, out _curHeight);
        //        if (frameData != null)
        //        {
        //            if (_curWidth != _width || _curHeight != _height)
        //            {
        //                _width = _curWidth;
        //                _height = _curHeight;
        //                _renderSource.SetupSurface(_curWidth, _curHeight);
        //            }
        //            renderFrame(frameData);
        //        }
        //    }
        //}

        #endregion 海康视频流格式转化


        #region 海康自支持的解码方式

        HikM4Decoder _hikDecoder;
        void initHikDecoder(byte[] header)
        {
            if(_hikDecoder == null)
            {
                _hikDecoder = new HikM4Decoder();
                _hikDecoder.VideoFrameEvent += onVideoFrame;
            }
            _hikDecoder.InputData(HikM4Decoder.HeaderType, header);
        }

        void disposeHikDecoder()
        {
            if (_hikDecoder != null)
            {
                _hikDecoder.VideoFrameEvent -= onVideoFrame;
                _hikDecoder.Dispose();
            }
            _hikDecoder = null;
        }

        private void onVideoFrame(HikM4Decoder.VideoFrame obj)
        {
            onDisplay(obj.Data, obj.Width, obj.Height);
        }

        #endregion 海康自支持的解码方式

        #region 解压后的图像数组转为图像
        int _width;
        int _height;
        private void onDisplay(byte[] frameData, int width, int height)
        {
            if (width != _width || height != _height)
            {
                _width = width;
                _height = height;
                _renderSource?.SetupSurface(width, height);
            }
            _renderSource?.Render(frameData);
        }

        private void initRenderSource()
        {
            _renderSource = new D3DImageSource();
            _renderSource.ImageSourceChanged += onImageSource;
        }

        void onImageSource()
        {
            ImageSrc = _renderSource?.ImageSource;
        }

        void disposeRenderSource()
        {
            if (_renderSource != null)
            {
                _renderSource.ImageSourceChanged -= onImageSource;
                _renderSource.Dispose();
            }
            _renderSource = null;
            _width = 0;
            _height = 0;
        }

        #endregion 解压后的图像数组转为图像
        
        public void Dispose()
        {
            stop();
        }
    }
}
