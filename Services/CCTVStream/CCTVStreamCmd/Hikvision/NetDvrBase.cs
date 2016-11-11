using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CCTVStreamCmd.Hikvision
{
    //实时声音模式
    enum REALSOUND_MODE
    {
        MONOPOLIZE_MODE = 1,//独占模式
        SHARE_MODE = 2      //共享模式
    }

    //软解码预览参数
    [StructLayout(LayoutKind.Sequential)]
    struct NET_DVR_CLIENTINFO
    {
        public int lChannel;
        public uint lLinkMode;
        public IntPtr hPlayWnd;
        public string sMultiCastIP;
    }


    //NET_DVR_Login_V30()参数结构
    [StructLayout(LayoutKind.Sequential)]
    struct NET_DVR_DEVICEINFO_V30
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NetDvrDll32.SERIALNO_LEN)]
        public string sSerialNumber;  //序列号
        public byte byAlarmInPortNum;       //报警输入个数
        public byte byAlarmOutPortNum;      //报警输出个数
        public byte byDiskNum;              //硬盘个数
        public byte byDVRType;              //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
        public byte byChanNum;              //模拟通道个数
        public byte byStartChan;            //起始通道号,例如DVS-1,DVR - 1
        public byte byAudioChanNum;         //语音通道数
        public byte byIPChanNum;                    //最大数字通道个数  
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] byRes1;                   //保留
    };

    public delegate void RealDataCallBack_V30(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser);
}
