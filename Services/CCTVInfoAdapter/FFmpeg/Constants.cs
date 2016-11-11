using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFmpeg
{
    public static class Constants
    {
        public struct AVRational
        {
            public int Numerator;
            public int Denominator;

            public static AVRational Invalid = new AVRational();
        }

        public const long AV_NOPTS_VALUE = unchecked((long)0x8000000000000000);

        public static int MakeFourCC(int ch0, int ch1, int ch2, int ch3)
        {
            return ((int)(byte)(ch0) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
        }

        public enum AVCodecID
        {
            AV_CODEC_ID_NONE,

            /* video codecs */
            AV_CODEC_ID_MPEG1VIDEO,
            AV_CODEC_ID_MPEG2VIDEO, ///< preferred ID for MPEG-1/2 video decoding
            AV_CODEC_ID_MPEG2VIDEO_XVMC,
            AV_CODEC_ID_H261,
            AV_CODEC_ID_H263,
            AV_CODEC_ID_RV10,
            AV_CODEC_ID_RV20,
            AV_CODEC_ID_MJPEG,
            AV_CODEC_ID_MJPEGB,
            AV_CODEC_ID_LJPEG,
            AV_CODEC_ID_SP5X,
            AV_CODEC_ID_JPEGLS,
            AV_CODEC_ID_MPEG4,
            AV_CODEC_ID_RAWVIDEO,
            AV_CODEC_ID_MSMPEG4V1,
            AV_CODEC_ID_MSMPEG4V2,
            AV_CODEC_ID_MSMPEG4V3,
            AV_CODEC_ID_WMV1,
            AV_CODEC_ID_WMV2,
            AV_CODEC_ID_H263P,
            AV_CODEC_ID_H263I,
            AV_CODEC_ID_FLV1,
            AV_CODEC_ID_SVQ1,
            AV_CODEC_ID_SVQ3,
            AV_CODEC_ID_DVVIDEO,
            AV_CODEC_ID_HUFFYUV,
            AV_CODEC_ID_CYUV,
            AV_CODEC_ID_H264,
            AV_CODEC_ID_INDEO3,
            AV_CODEC_ID_VP3,
            AV_CODEC_ID_THEORA,
            AV_CODEC_ID_ASV1,
            AV_CODEC_ID_ASV2,
            AV_CODEC_ID_FFV1,
            AV_CODEC_ID_4XM,
            AV_CODEC_ID_VCR1,
            AV_CODEC_ID_CLJR,
            AV_CODEC_ID_MDEC,
            AV_CODEC_ID_ROQ,
            AV_CODEC_ID_INTERPLAY_VIDEO,
            AV_CODEC_ID_XAN_WC3,
            AV_CODEC_ID_XAN_WC4,
            AV_CODEC_ID_RPZA,
            AV_CODEC_ID_CINEPAK,
            AV_CODEC_ID_WS_VQA,
            AV_CODEC_ID_MSRLE,
            AV_CODEC_ID_MSVIDEO1,
            AV_CODEC_ID_IDCIN,
            AV_CODEC_ID_8BPS,
            AV_CODEC_ID_SMC,
            AV_CODEC_ID_FLIC,
            AV_CODEC_ID_TRUEMOTION1,
            AV_CODEC_ID_VMDVIDEO,
            AV_CODEC_ID_MSZH,
            AV_CODEC_ID_ZLIB,
            AV_CODEC_ID_QTRLE,
            AV_CODEC_ID_TSCC,
            AV_CODEC_ID_ULTI,
            AV_CODEC_ID_QDRAW,
            AV_CODEC_ID_VIXL,
            AV_CODEC_ID_QPEG,
            AV_CODEC_ID_PNG,
            AV_CODEC_ID_PPM,
            AV_CODEC_ID_PBM,
            AV_CODEC_ID_PGM,
            AV_CODEC_ID_PGMYUV,
            AV_CODEC_ID_PAM,
            AV_CODEC_ID_FFVHUFF,
            AV_CODEC_ID_RV30,
            AV_CODEC_ID_RV40,
            AV_CODEC_ID_VC1,
            AV_CODEC_ID_WMV3,
            AV_CODEC_ID_LOCO,
            AV_CODEC_ID_WNV1,
            AV_CODEC_ID_AASC,
            AV_CODEC_ID_INDEO2,
            AV_CODEC_ID_FRAPS,
            AV_CODEC_ID_TRUEMOTION2,
            AV_CODEC_ID_BMP,
            AV_CODEC_ID_CSCD,
            AV_CODEC_ID_MMVIDEO,
            AV_CODEC_ID_ZMBV,
            AV_CODEC_ID_AVS,
            AV_CODEC_ID_SMACKVIDEO,
            AV_CODEC_ID_NUV,
            AV_CODEC_ID_KMVC,
            AV_CODEC_ID_FLASHSV,
            AV_CODEC_ID_CAVS,
            AV_CODEC_ID_JPEG2000,
            AV_CODEC_ID_VMNC,
            AV_CODEC_ID_VP5,
            AV_CODEC_ID_VP6,
            AV_CODEC_ID_VP6F,
            AV_CODEC_ID_TARGA,
            AV_CODEC_ID_DSICINVIDEO,
            AV_CODEC_ID_TIERTEXSEQVIDEO,
            AV_CODEC_ID_TIFF,
            AV_CODEC_ID_GIF,
            AV_CODEC_ID_DXA,
            AV_CODEC_ID_DNXHD,
            AV_CODEC_ID_THP,
            AV_CODEC_ID_SGI,
            AV_CODEC_ID_C93,
            AV_CODEC_ID_BETHSOFTVID,
            AV_CODEC_ID_PTX,
            AV_CODEC_ID_TXD,
            AV_CODEC_ID_VP6A,
            AV_CODEC_ID_AMV,
            AV_CODEC_ID_VB,
            AV_CODEC_ID_PCX,
            AV_CODEC_ID_SUNRAST,
            AV_CODEC_ID_INDEO4,
            AV_CODEC_ID_INDEO5,
            AV_CODEC_ID_MIMIC,
            AV_CODEC_ID_RL2,
            AV_CODEC_ID_ESCAPE124,
            AV_CODEC_ID_DIRAC,
            AV_CODEC_ID_BFI,
            AV_CODEC_ID_CMV,
            AV_CODEC_ID_MOTIONPIXELS,
            AV_CODEC_ID_TGV,
            AV_CODEC_ID_TGQ,
            AV_CODEC_ID_TQI,
            AV_CODEC_ID_AURA,
            AV_CODEC_ID_AURA2,
            AV_CODEC_ID_V210X,
            AV_CODEC_ID_TMV,
            AV_CODEC_ID_V210,
            AV_CODEC_ID_DPX,
            AV_CODEC_ID_MAD,
            AV_CODEC_ID_FRWU,
            AV_CODEC_ID_FLASHSV2,
            AV_CODEC_ID_CDGRAPHICS,
            AV_CODEC_ID_R210,
            AV_CODEC_ID_ANM,
            AV_CODEC_ID_BINKVIDEO,
            AV_CODEC_ID_IFF_ILBM,
            AV_CODEC_ID_IFF_BYTERUN1,
            AV_CODEC_ID_KGV1,
            AV_CODEC_ID_YOP,
            AV_CODEC_ID_VP8,
            AV_CODEC_ID_PICTOR,
            AV_CODEC_ID_ANSI,
            AV_CODEC_ID_A64_MULTI,
            AV_CODEC_ID_A64_MULTI5,
            AV_CODEC_ID_R10K,
            AV_CODEC_ID_MXPEG,
            AV_CODEC_ID_LAGARITH,
            AV_CODEC_ID_PRORES,
            AV_CODEC_ID_JV,
            AV_CODEC_ID_DFA,
            AV_CODEC_ID_WMV3IMAGE,
            AV_CODEC_ID_VC1IMAGE,
            AV_CODEC_ID_UTVIDEO,
            AV_CODEC_ID_BMV_VIDEO,
            AV_CODEC_ID_VBLE,
            AV_CODEC_ID_DXTORY,
            AV_CODEC_ID_V410,
            AV_CODEC_ID_XWD,
            AV_CODEC_ID_CDXL,
            AV_CODEC_ID_XBM,
            AV_CODEC_ID_ZEROCODEC,
            AV_CODEC_ID_MSS1,
            AV_CODEC_ID_MSA1,
            AV_CODEC_ID_TSCC2,
            AV_CODEC_ID_MTS2,
            AV_CODEC_ID_CLLC,
            AV_CODEC_ID_MSS2,
            AV_CODEC_ID_VP9,
            AV_CODEC_ID_AIC,
            AV_CODEC_ID_ESCAPE130_DEPRECATED,
            AV_CODEC_ID_G2M_DEPRECATED,
            AV_CODEC_ID_WEBP_DEPRECATED,
            AV_CODEC_ID_HNM4_VIDEO,
            AV_CODEC_ID_HEVC_DEPRECATED,
            AV_CODEC_ID_FIC,

            AV_CODEC_ID_BRENDER_PIX = 'B' | ('P' << 8) | ('I' << 16) | ('X' << 24), //MakeFourCC('B', 'P', 'I', 'X'),
            //AV_CODEC_ID_Y41P = MakeFourCC('Y', '4', '1', 'P'),
            //AV_CODEC_ID_ESCAPE130 = MakeFourCC('E', '1', '3', '0'),
            //AV_CODEC_ID_EXR = MakeFourCC('0', 'E', 'X', 'R'),
            //AV_CODEC_ID_AVRP = MakeFourCC('A', 'V', 'R', 'P'),

            //AV_CODEC_ID_012V = MakeFourCC('0', '1', '2', 'V'),
            //AV_CODEC_ID_G2M = MakeFourCC(0, 'G', '2', 'M'),
            //AV_CODEC_ID_AVUI = MakeFourCC('A', 'V', 'U', 'I'),
            //AV_CODEC_ID_AYUV = MakeFourCC('A', 'Y', 'U', 'V'),
            //AV_CODEC_ID_TARGA_Y216 = MakeFourCC('T', '2', '1', '6'),
            //AV_CODEC_ID_V308 = MakeFourCC('V', '3', '0', '8'),
            //AV_CODEC_ID_V408 = MakeFourCC('V', '4', '0', '8'),
            //AV_CODEC_ID_YUV4 = MakeFourCC('Y', 'U', 'V', '4'),
            //AV_CODEC_ID_SANM = MakeFourCC('S', 'A', 'N', 'M'),
            //AV_CODEC_ID_PAF_VIDEO = MakeFourCC('P', 'A', 'F', 'V'),
            //AV_CODEC_ID_AVRN = MakeFourCC('A', 'V', 'R', 'n'),
            //AV_CODEC_ID_CPIA = MakeFourCC('C', 'P', 'I', 'A'),
            //AV_CODEC_ID_XFACE = MakeFourCC('X', 'F', 'A', 'C'),
            //AV_CODEC_ID_SGIRLE = MakeFourCC('S', 'G', 'I', 'R'),
            //AV_CODEC_ID_MVC1 = MakeFourCC('M', 'V', 'C', '1'),
            //AV_CODEC_ID_MVC2 = MakeFourCC('M', 'V', 'C', '2'),
            //AV_CODEC_ID_SNOW = MakeFourCC('S', 'N', 'O', 'W'),
            //AV_CODEC_ID_WEBP = MakeFourCC('W', 'E', 'B', 'P'),
            //AV_CODEC_ID_SMVJPEG = MakeFourCC('S', 'M', 'V', 'J'),
            AV_CODEC_ID_HEVC = 'H' | ('2' << 8) | ('6' << 16) | ('5' << 24), //MakeFourCC('H', '2', '6', '5'),
            AV_CODEC_ID_H265 = AV_CODEC_ID_HEVC,

            /* various PCM "codecs" */
            AV_CODEC_ID_FIRST_AUDIO = 0x10000,     ///< A dummy id pointing at the start of audio codecs
            AV_CODEC_ID_PCM_S16LE = 0x10000,
            AV_CODEC_ID_PCM_S16BE,
            AV_CODEC_ID_PCM_U16LE,
            AV_CODEC_ID_PCM_U16BE,
            AV_CODEC_ID_PCM_S8,
            AV_CODEC_ID_PCM_U8,
            AV_CODEC_ID_PCM_MULAW,
            AV_CODEC_ID_PCM_ALAW,
            AV_CODEC_ID_PCM_S32LE,
            AV_CODEC_ID_PCM_S32BE,
            AV_CODEC_ID_PCM_U32LE,
            AV_CODEC_ID_PCM_U32BE,
            AV_CODEC_ID_PCM_S24LE,
            AV_CODEC_ID_PCM_S24BE,
            AV_CODEC_ID_PCM_U24LE,
            AV_CODEC_ID_PCM_U24BE,
            AV_CODEC_ID_PCM_S24DAUD,
            AV_CODEC_ID_PCM_ZORK,
            AV_CODEC_ID_PCM_S16LE_PLANAR,
            AV_CODEC_ID_PCM_DVD,
            AV_CODEC_ID_PCM_F32BE,
            AV_CODEC_ID_PCM_F32LE,
            AV_CODEC_ID_PCM_F64BE,
            AV_CODEC_ID_PCM_F64LE,
            AV_CODEC_ID_PCM_BLURAY,
            AV_CODEC_ID_PCM_LXF,
            AV_CODEC_ID_S302M,
            AV_CODEC_ID_PCM_S8_PLANAR,
            AV_CODEC_ID_PCM_S24LE_PLANAR_DEPRECATED,
            AV_CODEC_ID_PCM_S32LE_PLANAR_DEPRECATED,
            //AV_CODEC_ID_PCM_S24LE_PLANAR = MakeFourCC(24, 'P', 'S', 'P'),
            //AV_CODEC_ID_PCM_S32LE_PLANAR = MakeFourCC(32, 'P', 'S', 'P'),
            //AV_CODEC_ID_PCM_S16BE_PLANAR = MakeFourCC('P', 'S', 'P', 16),

            /* various ADPCM codecs */
            AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
            AV_CODEC_ID_ADPCM_IMA_WAV,
            AV_CODEC_ID_ADPCM_IMA_DK3,
            AV_CODEC_ID_ADPCM_IMA_DK4,
            AV_CODEC_ID_ADPCM_IMA_WS,
            AV_CODEC_ID_ADPCM_IMA_SMJPEG,
            AV_CODEC_ID_ADPCM_MS,
            AV_CODEC_ID_ADPCM_4XM,
            AV_CODEC_ID_ADPCM_XA,
            AV_CODEC_ID_ADPCM_ADX,
            AV_CODEC_ID_ADPCM_EA,
            AV_CODEC_ID_ADPCM_G726,
            AV_CODEC_ID_ADPCM_CT,
            AV_CODEC_ID_ADPCM_SWF,
            AV_CODEC_ID_ADPCM_YAMAHA,
            AV_CODEC_ID_ADPCM_SBPRO_4,
            AV_CODEC_ID_ADPCM_SBPRO_3,
            AV_CODEC_ID_ADPCM_SBPRO_2,
            AV_CODEC_ID_ADPCM_THP,
            AV_CODEC_ID_ADPCM_IMA_AMV,
            AV_CODEC_ID_ADPCM_EA_R1,
            AV_CODEC_ID_ADPCM_EA_R3,
            AV_CODEC_ID_ADPCM_EA_R2,
            AV_CODEC_ID_ADPCM_IMA_EA_SEAD,
            AV_CODEC_ID_ADPCM_IMA_EA_EACS,
            AV_CODEC_ID_ADPCM_EA_XAS,
            AV_CODEC_ID_ADPCM_EA_MAXIS_XA,
            AV_CODEC_ID_ADPCM_IMA_ISS,
            AV_CODEC_ID_ADPCM_G722,
            AV_CODEC_ID_ADPCM_IMA_APC,
            //AV_CODEC_ID_VIMA = MakeFourCC('V', 'I', 'M', 'A'),
            //AV_CODEC_ID_ADPCM_AFC = MakeFourCC('A', 'F', 'C', ' '),
            //AV_CODEC_ID_ADPCM_IMA_OKI = MakeFourCC('O', 'K', 'I', ' '),
            //AV_CODEC_ID_ADPCM_DTK = MakeFourCC('D', 'T', 'K', ' '),
            //AV_CODEC_ID_ADPCM_IMA_RAD = MakeFourCC('R', 'A', 'D', ' '),
            //AV_CODEC_ID_ADPCM_G726LE = MakeFourCC('6', '2', '7', 'G'),

            /* AMR */
            AV_CODEC_ID_AMR_NB = 0x12000,
            AV_CODEC_ID_AMR_WB,

            /* RealAudio codecs*/
            AV_CODEC_ID_RA_144 = 0x13000,
            AV_CODEC_ID_RA_288,

            /* various DPCM codecs */
            AV_CODEC_ID_ROQ_DPCM = 0x14000,
            AV_CODEC_ID_INTERPLAY_DPCM,
            AV_CODEC_ID_XAN_DPCM,
            AV_CODEC_ID_SOL_DPCM,

            /* audio codecs */
            AV_CODEC_ID_MP2 = 0x15000,
            AV_CODEC_ID_MP3, ///< preferred ID for decoding MPEG audio layer 1, 2 or 3
            AV_CODEC_ID_AAC,
            AV_CODEC_ID_AC3,
            AV_CODEC_ID_DTS,
            AV_CODEC_ID_VORBIS,
            AV_CODEC_ID_DVAUDIO,
            AV_CODEC_ID_WMAV1,
            AV_CODEC_ID_WMAV2,
            AV_CODEC_ID_MACE3,
            AV_CODEC_ID_MACE6,
            AV_CODEC_ID_VMDAUDIO,
            AV_CODEC_ID_FLAC,
            AV_CODEC_ID_MP3ADU,
            AV_CODEC_ID_MP3ON4,
            AV_CODEC_ID_SHORTEN,
            AV_CODEC_ID_ALAC,
            AV_CODEC_ID_WESTWOOD_SND1,
            AV_CODEC_ID_GSM, ///< as in Berlin toast format
            AV_CODEC_ID_QDM2,
            AV_CODEC_ID_COOK,
            AV_CODEC_ID_TRUESPEECH,
            AV_CODEC_ID_TTA,
            AV_CODEC_ID_SMACKAUDIO,
            AV_CODEC_ID_QCELP,
            AV_CODEC_ID_WAVPACK,
            AV_CODEC_ID_DSICINAUDIO,
            AV_CODEC_ID_IMC,
            AV_CODEC_ID_MUSEPACK7,
            AV_CODEC_ID_MLP,
            AV_CODEC_ID_GSM_MS, /* as found in WAV */
            AV_CODEC_ID_ATRAC3,
            AV_CODEC_ID_VOXWARE,
            AV_CODEC_ID_APE,
            AV_CODEC_ID_NELLYMOSER,
            AV_CODEC_ID_MUSEPACK8,
            AV_CODEC_ID_SPEEX,
            AV_CODEC_ID_WMAVOICE,
            AV_CODEC_ID_WMAPRO,
            AV_CODEC_ID_WMALOSSLESS,
            AV_CODEC_ID_ATRAC3P,
            AV_CODEC_ID_EAC3,
            AV_CODEC_ID_SIPR,
            AV_CODEC_ID_MP1,
            AV_CODEC_ID_TWINVQ,
            AV_CODEC_ID_TRUEHD,
            AV_CODEC_ID_MP4ALS,
            AV_CODEC_ID_ATRAC1,
            AV_CODEC_ID_BINKAUDIO_RDFT,
            AV_CODEC_ID_BINKAUDIO_DCT,
            AV_CODEC_ID_AAC_LATM,
            AV_CODEC_ID_QDMC,
            AV_CODEC_ID_CELT,
            AV_CODEC_ID_G723_1,
            AV_CODEC_ID_G729,
            AV_CODEC_ID_8SVX_EXP,
            AV_CODEC_ID_8SVX_FIB,
            AV_CODEC_ID_BMV_AUDIO,
            AV_CODEC_ID_RALF,
            AV_CODEC_ID_IAC,
            AV_CODEC_ID_ILBC,
            AV_CODEC_ID_OPUS_DEPRECATED,
            AV_CODEC_ID_COMFORT_NOISE,
            AV_CODEC_ID_TAK_DEPRECATED,
            AV_CODEC_ID_METASOUND,
            //AV_CODEC_ID_FFWAVESYNTH = MakeFourCC('F', 'F', 'W', 'S'),
            //AV_CODEC_ID_SONIC = MakeFourCC('S', 'O', 'N', 'C'),
            //AV_CODEC_ID_SONIC_LS = MakeFourCC('S', 'O', 'N', 'L'),
            //AV_CODEC_ID_PAF_AUDIO = MakeFourCC('P', 'A', 'F', 'A'),
            //AV_CODEC_ID_OPUS = MakeFourCC('O', 'P', 'U', 'S'),
            //AV_CODEC_ID_TAK = MakeFourCC('t', 'B', 'a', 'K'),
            //AV_CODEC_ID_EVRC = MakeFourCC('s', 'e', 'v', 'c'),
            //AV_CODEC_ID_SMV = MakeFourCC('s', 's', 'm', 'v'),

            /* subtitle codecs */
            AV_CODEC_ID_FIRST_SUBTITLE = 0x17000,          ///< A dummy ID pointing at the start of subtitle codecs.
            AV_CODEC_ID_DVD_SUBTITLE = 0x17000,
            AV_CODEC_ID_DVB_SUBTITLE,
            AV_CODEC_ID_TEXT,  ///< raw UTF-8 text
            AV_CODEC_ID_XSUB,
            AV_CODEC_ID_SSA,
            AV_CODEC_ID_MOV_TEXT,
            AV_CODEC_ID_HDMV_PGS_SUBTITLE,
            AV_CODEC_ID_DVB_TELETEXT,
            AV_CODEC_ID_SRT,
            //AV_CODEC_ID_MICRODVD = MakeFourCC('m', 'D', 'V', 'D'),
            //AV_CODEC_ID_EIA_608 = MakeFourCC('c', '6', '0', '8'),
            //AV_CODEC_ID_JACOSUB = MakeFourCC('J', 'S', 'U', 'B'),
            //AV_CODEC_ID_SAMI = MakeFourCC('S', 'A', 'M', 'I'),
            //AV_CODEC_ID_REALTEXT = MakeFourCC('R', 'T', 'X', 'T'),
            //AV_CODEC_ID_SUBVIEWER1 = MakeFourCC('S', 'b', 'V', '1'),
            //AV_CODEC_ID_SUBVIEWER = MakeFourCC('S', 'u', 'b', 'V'),
            //AV_CODEC_ID_SUBRIP = MakeFourCC('S', 'R', 'i', 'p'),
            //AV_CODEC_ID_WEBVTT = MakeFourCC('W', 'V', 'T', 'T'),
            //AV_CODEC_ID_MPL2 = MakeFourCC('M', 'P', 'L', '2'),
            //AV_CODEC_ID_VPLAYER = MakeFourCC('V', 'P', 'l', 'r'),
            //AV_CODEC_ID_PJS = MakeFourCC('P', 'h', 'J', 'S'),
            //AV_CODEC_ID_ASS = MakeFourCC('A', 'S', 'S', ' '),  ///< ASS as defined in Matroska

            /* other specific kind of codecs (generally used for attachments) */
            AV_CODEC_ID_FIRST_UNKNOWN = 0x18000,           ///< A dummy ID pointing at the start of various fake codecs.
            AV_CODEC_ID_TTF = 0x18000,
            //AV_CODEC_ID_BINTEXT = MakeFourCC('B', 'T', 'X', 'T'),
            //AV_CODEC_ID_XBIN = MakeFourCC('X', 'B', 'I', 'N'),
            //AV_CODEC_ID_IDF = MakeFourCC(0, 'I', 'D', 'F'),
            //AV_CODEC_ID_OTF = MakeFourCC(0, 'O', 'T', 'F'),
            //AV_CODEC_ID_SMPTE_KLV = MakeFourCC('K', 'L', 'V', 'A'),
            //AV_CODEC_ID_DVD_NAV = MakeFourCC('D', 'N', 'A', 'V'),
            //AV_CODEC_ID_TIMED_ID3 = MakeFourCC('T', 'I', 'D', '3'),


            AV_CODEC_ID_PROBE = 0x19000, ///< codec_id is not known (like AV_CODEC_ID_NONE) but lavf should attempt to identify it

            AV_CODEC_ID_MPEG2TS = 0x20000, /**< _FAKE_ codec to indicate a raw MPEG-2 TS
                                * stream (only used by libavformat) */
            AV_CODEC_ID_MPEG4SYSTEMS = 0x20001, /**< _FAKE_ codec to indicate a MPEG-4 Systems
                                * stream (only used by libavformat) */
            AV_CODEC_ID_FFMETADATA = 0x21000,   ///< Dummy codec for streams containing only metadata information.

        };

        [Flags]
        public enum ConvertionFlags : int
        {
            SWS_FAST_BILINEAR = 1,
            SWS_BILINEAR = 2,
            SWS_BICUBIC = 4,
            SWS_X = 8,
            SWS_POINT = 0x10,
            SWS_AREA = 0x20,
            SWS_BICUBLIN = 0x40,
            SWS_GAUSS = 0x80,
            SWS_SINC = 0x100,
            SWS_LANCZOS = 0x200,
            SWS_SPLINE = 0x400
        }

        public enum SwsPixelFormat
        {
            PIX_FMT_NONE = -1,
            /// <summary>
            /// planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)
            /// </summary>
            PIX_FMT_YUV420P,
            /// <summary>
            /// packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr
            /// </summary>
            PIX_FMT_YUYV422,
            /// <summary>
            /// packed RGB 8:8:8, 24bpp, RGBRGB...
            /// </summary>
            PIX_FMT_RGB24,
            /// <summary>
            /// packed RGB 8:8:8, 24bpp, BGRBGR...
            /// </summary>
            PIX_FMT_BGR24,
            /// <summary>
            /// planar YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
            /// </summary>
            PIX_FMT_YUV422P,
            /// <summary>
            /// planar YUV 4:4:4, 24bpp, (1 Cr & Cb sample per 1x1 Y samples)
            /// </summary>
            PIX_FMT_YUV444P,
            /// <summary>
            /// planar YUV 4:1:0,  9bpp, (1 Cr & Cb sample per 4x4 Y samples)
            /// </summary>
            PIX_FMT_YUV410P,
            /// <summary>
            /// planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples)
            /// </summary>
            PIX_FMT_YUV411P,
            /// <summary>
            /// Y, 8bpp
            /// </summary>
            PIX_FMT_GRAY8,
            /// <summary>
            /// Y, 1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb
            /// </summary>
            PIX_FMT_MONOWHITE,
            /// <summary>
            /// Y, 1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb
            /// </summary>
            PIX_FMT_MONOBLACK,
            PIX_FMT_PAL8,      ///< 8 bit with PIX_FMT_RGB32 palette
            PIX_FMT_YUVJ420P,  ///< planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of PIX_FMT_YUV420P and setting color_range
            PIX_FMT_YUVJ422P,  ///< planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of PIX_FMT_YUV422P and setting color_range
            PIX_FMT_YUVJ444P,  ///< planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of PIX_FMT_YUV444P and setting color_range
            PIX_FMT_XVMC_MPEG2_MC,///< XVideo Motion Acceleration via common packet passing
            PIX_FMT_XVMC_MPEG2_IDCT,
            /// <summary>
            /// packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
            /// </summary>
            PIX_FMT_UYVY422,
            /// <summary>
            /// packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
            /// </summary>
            PIX_FMT_UYYVYY411,
            /// <summary>
            /// packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
            /// </summary>
            PIX_FMT_BGR8,
            /// <summary>
            /// packed RGB 1:2:1 bitstream,  4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
            /// </summary>
            PIX_FMT_BGR4,
            /// <summary>
            /// packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
            /// </summary>
            PIX_FMT_BGR4_BYTE,
            /// <summary>
            /// packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
            /// </summary>
            PIX_FMT_RGB8,
            /// <summary>
            /// packed RGB 1:2:1 bitstream,  4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
            /// </summary>
            PIX_FMT_RGB4,
            /// <summary>
            /// packed RGB 1:2:1,  8bpp, (msb)1R 2G 1B(lsb)
            /// </summary>
            PIX_FMT_RGB4_BYTE,
            /// <summary>
            /// planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
            /// </summary>
            PIX_FMT_NV12,
            /// <summary>
            /// as above, but U and V bytes are swapped
            /// </summary>
            PIX_FMT_NV21,
            /// <summary>
            /// packed ARGB 8:8:8:8, 32bpp, ARGBARGB...
            /// </summary>
            PIX_FMT_ARGB,
            /// <summary>
            /// packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
            /// </summary>
            PIX_FMT_RGBA,
            /// <summary>
            /// packed ABGR 8:8:8:8, 32bpp, ABGRABGR...
            /// </summary>
            PIX_FMT_ABGR,
            /// <summary>
            /// packed BGRA 8:8:8:8, 32bpp, BGRABGRA...
            /// </summary>
            PIX_FMT_BGRA,

            PIX_FMT_GRAY16BE,  ///<        Y        , 16bpp, big-endian
            PIX_FMT_GRAY16LE,  ///<        Y        , 16bpp, little-endian
            PIX_FMT_YUV440P,   ///< planar YUV 4:4:0 (1 Cr & Cb sample per 1x2 Y samples)
            PIX_FMT_YUVJ440P,  ///< planar YUV 4:4:0 full scale (JPEG), deprecated in favor of PIX_FMT_YUV440P and setting color_range
            PIX_FMT_YUVA420P,  ///< planar YUV 4:2:0, 20bpp, (1 Cr & Cb sample per 2x2 Y & A samples)
            PIX_FMT_VDPAU_H264,///< H.264 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_VDPAU_MPEG1,///< MPEG-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_VDPAU_MPEG2,///< MPEG-2 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_VDPAU_WMV3,///< WMV3 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_VDPAU_VC1, ///< VC-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_RGB48BE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian
            PIX_FMT_RGB48LE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian

            PIX_FMT_RGB565BE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), big-endian
            PIX_FMT_RGB565LE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), little-endian
            PIX_FMT_RGB555BE,  ///< packed RGB 5:5:5, 16bpp, (msb)1A 5R 5G 5B(lsb), big-endian, most significant bit to 0
            PIX_FMT_RGB555LE,  ///< packed RGB 5:5:5, 16bpp, (msb)1A 5R 5G 5B(lsb), little-endian, most significant bit to 0

            PIX_FMT_BGR565BE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), big-endian
            PIX_FMT_BGR565LE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), little-endian
            PIX_FMT_BGR555BE,  ///< packed BGR 5:5:5, 16bpp, (msb)1A 5B 5G 5R(lsb), big-endian, most significant bit to 1
            PIX_FMT_BGR555LE,  ///< packed BGR 5:5:5, 16bpp, (msb)1A 5B 5G 5R(lsb), little-endian, most significant bit to 1

            PIX_FMT_VAAPI_MOCO, ///< HW acceleration through VA API at motion compensation entry-point, Picture.data[3] contains a vaapi_render_state struct which contains macroblocks as well as various fields extracted from headers
            PIX_FMT_VAAPI_IDCT, ///< HW acceleration through VA API at IDCT entry-point, Picture.data[3] contains a vaapi_render_state struct which contains fields extracted from headers
            PIX_FMT_VAAPI_VLD,  ///< HW decoding through VA API, Picture.data[3] contains a vaapi_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers

            PIX_FMT_YUV420P16LE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
            PIX_FMT_YUV420P16BE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
            PIX_FMT_YUV422P16LE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
            PIX_FMT_YUV422P16BE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
            PIX_FMT_YUV444P16LE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
            PIX_FMT_YUV444P16BE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
            PIX_FMT_VDPAU_MPEG4,  ///< MPEG4 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
            PIX_FMT_DXVA2_VLD,    ///< HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer

            PIX_FMT_RGB444LE,  ///< packed RGB 4:4:4, 16bpp, (msb)4A 4R 4G 4B(lsb), little-endian, most significant bits to 0
            PIX_FMT_RGB444BE,  ///< packed RGB 4:4:4, 16bpp, (msb)4A 4R 4G 4B(lsb), big-endian, most significant bits to 0
            PIX_FMT_BGR444LE,  ///< packed BGR 4:4:4, 16bpp, (msb)4A 4B 4G 4R(lsb), little-endian, most significant bits to 1
            PIX_FMT_BGR444BE,  ///< packed BGR 4:4:4, 16bpp, (msb)4A 4B 4G 4R(lsb), big-endian, most significant bits to 1
            PIX_FMT_GRAY8A,    ///< 8bit gray, 8bit alpha
            PIX_FMT_BGR48BE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian
            PIX_FMT_BGR48LE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian

            //the following 10 formats have the disadvantage of needing 1 format for each bit depth, thus
            //If you want to support multiple bit depths, then using PIX_FMT_YUV420P16* with the bpp stored seperately
            //is better
            PIX_FMT_YUV420P9BE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
            PIX_FMT_YUV420P9LE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
            PIX_FMT_YUV420P10BE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
            PIX_FMT_YUV420P10LE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
            PIX_FMT_YUV422P10BE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
            PIX_FMT_YUV422P10LE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
            PIX_FMT_YUV444P9BE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
            PIX_FMT_YUV444P9LE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
            PIX_FMT_YUV444P10BE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
            PIX_FMT_YUV444P10LE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
            PIX_FMT_NB,        ///< number of pixel formats, DO NOT USE THIS if you want to link with shared libav* because the number of formats might differ between versions
        }      
    }
}
