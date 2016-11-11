using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg;
using GatewayNet.Util;
using Seecool.VideoStreamBase;

namespace GatewayNet.H264
{
    /// <summary>
    /// 海康PS码流解包。
    /// </summary>
    public class UnpackPS
    {
        private Constants.AVCodecID _codecId = Constants.AVCodecID.AV_CODEC_ID_NONE;
        private PSFragment _lastFg;
        public Constants.AVCodecID CodecID
        {
            get { return _codecId; }
            private set
            {
                if (_codecId != value)
                {
                    var old = _codecId;
                    _codecId = value;
                    onCodeIdChanged(old, value);
                }
            }
        }

        public UnpackPS()
        {
        }

        public void UpdateStandardStream(byte[] buffer)
        {
            if (buffer.Length < 10)
                return;
            int index = getStreamStartCode(buffer, 0);
            if (index == 3)//值为0x000001的位串，它和后面的stream id 构成了标识分组开始的分组起始码，用来标志一个包的开始。
            {
                switch (buffer[index])// Stream id：在节目流中，它规定了基本流的号码和类型。0x(C0~DF)指音频，0x(E0~EF)为视频
                {
                    case 0xBA://PS包头起始码,前12位是RTP Header,接下来的9位包括了SCR，SCRE，MUXRate
                        decodeBA(buffer, index + 1);
                        return;
                    case 0xBB://Systemheader当且仅当pack是第一个数据包时才存在，即PS包头之后就是系统标题.
                        decodeBB(buffer, index + 1);
                        return;
                    case 0xBC://Systemheader当且仅当pack是第一个数据包时才存在，即系统标题之后就是节目流映射。
                        decodeBC(buffer, index + 1);
                        return;
                    case 0xBD://这个是私有流的标识,有的hk摄像头回调然后解读出来的原始h.264码流，有的一包里只有分界符数据(nal_unit_type=9)或补充增强信息单元(nal_unit_type=6)，如果直接送入解码器，有可能会出现问题，这里的处理方式要么丢弃这两个部分，要么和之后的数据合起来，再送入解码器里
                        return;
                }
                if (buffer[index] >= 0xE0 && buffer[index] <= 0xEF)//0x(E0~EF)为视频流
                    decodeE0(buffer, index + 1);
                else
                {
                    if (_lastFg != null)
                    {
                        _lastFg.IsFrameEnd = true;
                        toFireUnpacked();
                    }
                }
            }
        }

        /// <returns>-1 is false, other is length</returns>
        int getStreamStartCode(byte[] buffer, int index)
        {
            int length = -1;
            for (int i = index; i < buffer.Length; i++)
            {
                if (buffer[i] == 0x00)
                    continue;
                if (buffer[i] == 0x01)
                    length = i - index + 1;
                break;
            }
            return length;
        }

        private void decodeBA(byte[] buffer, int index)//节目流包（PS header）
        {
            index += 6;//系统时钟参考字段,系统时钟参考(SCR)是一个分两部分编码的42位字段。第一部分system_clock_reference_base是一个长度为33位的字段，其值SCR_base(i)由式2-19给出；第二部分system_clock_reference_extenstion是一个长度为9位的字段，其值SCR_ext(i)由式2-20给出。SCR字段指出了基本流中包含ESCR_base最后一位的字节到达节目目标解码器输入端的期望时间。
            index += 4;//节目复合速率字段,一个22位整数，规定P-STD在包含该字段的包期间接收节目流的速率。其值以50字节/秒为单位。不允许取0值。该字段所表示的值用于在2.5.2中定义P-STD输入端的字节到达时间。该字段值在本标准中的节目多路复合流的不同包中取值可能不同。
            int pack_stuffing_length = buffer[index - 1] & 0x07;//包填充长度字段,3位整数，规定该字段后填充字节的个数。
            index += pack_stuffing_length;//跳过“填充字节”;
            if (index < buffer.Length)
            {
                byte[] sub = BytesHelper.SubBytes(buffer, index);
                UpdateStandardStream(sub);
            }
        }

        private void decodeBB(byte[] buffer, int index)//PS system header（即PS系统头：节目流系统标题）
        {
        }

        private byte[] decodeBC(byte[] buffer, int index)//PS流的节目映射流部分(节目流映射)
        {
            int program_stream_map_length = readUshort(buffer, index);//节目流映射长度字段,16位字段。指示紧跟在该字段后的program_stream_map中的字节数。
            index += 2;//program_stream_map_length
            if (program_stream_map_length > 0x3fa || buffer.Length < index + program_stream_map_length)//该字段的最大值为1018(0x3FA)。
                return null;
            index += 2;//当前下一个指示符字段 + 节目流映射版本字段 + 标记位字段
            int program_stream_info_length = readUshort(buffer, index);  //节目流信息长度字段
            index += 2;//ushort(program_stream_info_length)
            index += program_stream_info_length;
            int elementary_stream_map_length = readUshort(buffer, index);//基本流映射长度字段,16位字段，指出在该节目流映射中的所有基本流信息的字节长度。
                                                                         //它只包括stream_type、elementary_stream_id和elementary_stream_info_length字段。(这里注意一下，这里的基本流映射长度，它只包括他后面的指定的那几个定义字段的总和，即从从这个长度，我们可以知道后面它根了几种类型的流定义，因为一种流的这个定义字段：stream_type(1BYTE)、elementary_stream_id(1BYTE)和elementary_stream_info_length(2BYTE)字段总和为4个字节，所以用elementary_stream_map_length /4 可以得到后面定义了几个流类型信息。)
            index += 2;//elementary_stream_map_length
            int streamType = buffer[index];//
            switch (streamType)
            {
                case 0x10://MPEG-4 视频流
                    CodecID = Constants.AVCodecID.AV_CODEC_ID_MPEG4;
                    break;
                case 0x1B://H.264 视频流
                    CodecID = Constants.AVCodecID.AV_CODEC_ID_H264;
                    break;
                case 0x80://SVAC 视频流
                    break;
                case 0x90://G.711 音频流
                case 0x92://G.722.1 音频流
                case 0x93://G.723.1 音频流
                case 0x99://G.729 音频流
                case 0x9B://SVAC音频流
                    break;
            }
            return null;
        }

        private void decodeE0(byte[] buffer, int index)//视频流
        {
            int length = readUshort(buffer, 4);//PES packet length：16 位字段，指出了PES 分组中跟在该字段后的字节数目。值为0 表示PES 分组长度要么没有规定要么没有限制。这种情况只允许出现在有效负载包含来源于传输流分组中某个视频基本流的字节的PES 分组中。
            if (length != buffer.Length - 6)
                return;

            int start = 8 + buffer[8];
            index = start + 1;
            int startcodeLen = getStreamStartCode(buffer, index);
            if (startcodeLen == 4 || startcodeLen == 3)
            {
                Nalu nal = Nalu.Parse(BytesHelper.SubBytes(buffer, index + startcodeLen));
                //标记上一个包是一帧的结束。
                if (_lastFg != null)
                {
                    _lastFg.IsFrameEnd = true;
                    toFireUnpacked();
                }
                //因为有header值，因此此包是一帧开始
                _lastFg = new PSFragment(nal.Header, nal.Payload);
            }
            else
            {
                toFireUnpacked();
                _lastFg = new PSFragment(null, BytesHelper.SubBytes(buffer, index));
            }
        }

        int readUshort(byte[] buffer, int index)
        {
            return buffer[index] * 256 + buffer[index + 1];
        }

        private void toFireUnpacked()
        {
            if (_lastFg != null)
                onUnpacked(_lastFg);
            _lastFg = null;
        }

        #region 【事件定义
        public event Action<object, PSFragment> Unpacked;
        private void onUnpacked(PSFragment psf)
        {
            Unpacked?.Invoke(this, psf);
        }

        public event EventHandler<CodeIdEventArgs> CodeIdChanged;
        private void onCodeIdChanged(Constants.AVCodecID old, Constants.AVCodecID ne)
        {
            CodeIdChanged?.Invoke(this, new CodeIdEventArgs(old, ne));
        }
        #endregion 【事件定义】
    }

    public class CodeIdEventArgs
    {
        public Constants.AVCodecID OldId { get; private set; }
        public Constants.AVCodecID NewId { get; private set; }

        public CodeIdEventArgs(Constants.AVCodecID old, Constants.AVCodecID ne)
        {
            OldId = old;
            NewId = ne;
        }
    }
}
