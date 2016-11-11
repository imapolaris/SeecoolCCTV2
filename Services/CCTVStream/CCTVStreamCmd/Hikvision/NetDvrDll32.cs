using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CCTVStreamCmd.Hikvision
{
    static class NetDvrDll32
    {
        const string _dllPath = @"x86\HCNetSDK.dll";

        #region 全局错误码

        public const int NET_DVR_FAIL = -1;
        public const int NET_DVR_NOERROR = 0;//没有错误
        public const int NET_DVR_PASSWORD_ERROR = 1;//用户名密码错误
        public const int NET_DVR_NOENOUGHPRI = 2;//权限不足
        public const int NET_DVR_NOINIT = 3;//没有初始化
        public const int NET_DVR_CHANNEL_ERROR = 4;//通道号错误
        public const int NET_DVR_OVER_MAXLINK = 5;//连接到DVR的客户端个数超过最大
        public const int NET_DVR_VERSIONNOMATCH = 6;//版本不匹配
        public const int NET_DVR_NETWORK_FAIL_CONNECT = 7;//连接服务器失败
        public const int NET_DVR_NETWORK_SEND_ERROR = 8;//向服务器发送失败
        public const int NET_DVR_NETWORK_RECV_ERROR = 9;//从服务器接收数据失败
        public const int NET_DVR_NETWORK_RECV_TIMEOUT = 10;//从服务器接收数据超时
        public const int NET_DVR_NETWORK_ERRORDATA = 11;//传送的数据有误
        public const int NET_DVR_ORDER_ERROR = 12;//调用次序错误
        public const int NET_DVR_OPERNOPERMIT = 13;//无此权限
        public const int NET_DVR_COMMANDTIMEOUT = 14;//DVR命令执行超时
        public const int NET_DVR_ERRORSERIALPORT = 15;//串口号错误
        public const int NET_DVR_ERRORALARMPORT = 16;//报警端口错误
        public const int NET_DVR_PARAMETER_ERROR = 17;//参数错误
        public const int NET_DVR_CHAN_EXCEPTION = 18;//服务器通道处于错误状态
        public const int NET_DVR_NODISK = 19;//没有硬盘
        public const int NET_DVR_ERRORDISKNUM = 20;//硬盘号错误
        public const int NET_DVR_DISK_FULL = 21;//服务器硬盘满
        public const int NET_DVR_DISK_ERROR = 22;//服务器硬盘出错
        public const int NET_DVR_NOSUPPORT = 23;//服务器不支持
        public const int NET_DVR_BUSY = 24;//服务器忙
        public const int NET_DVR_MODIFY_FAIL = 25;//服务器修改不成功
        public const int NET_DVR_PASSWORD_FORMAT_ERROR = 26;//密码输入格式不正确
        public const int NET_DVR_DISK_FORMATING = 27;//硬盘正在格式化，不能启动操作
        public const int NET_DVR_DVRNORESOURCE = 28;//DVR资源不足
        public const int NET_DVR_DVROPRATEFAILED = 29;//DVR操作失败
        public const int NET_DVR_OPENHOSTSOUND_FAIL = 30;//打开PC声音失败
        public const int NET_DVR_DVRVOICEOPENED = 31;//服务器语音对讲被占用
        public const int NET_DVR_TIMEINPUTERROR = 32;//时间输入不正确
        public const int NET_DVR_NOSPECFILE = 33;//回放时服务器没有指定的文件
        public const int NET_DVR_CREATEFILE_ERROR = 34;//创建文件出错
        public const int NET_DVR_FILEOPENFAIL = 35;//打开文件出错
        public const int NET_DVR_OPERNOTFINISH = 36;//上次的操作还没有完成
        public const int NET_DVR_GETPLAYTIMEFAIL = 37;//获取当前播放的时间出错
        public const int NET_DVR_PLAYFAIL = 38;//播放出错
        public const int NET_DVR_FILEFORMAT_ERROR = 39;//文件格式不正确
        public const int NET_DVR_DIR_ERROR = 40;//路径错误
        public const int NET_DVR_ALLOC_RESOURCE_ERROR = 41;//资源分配错误
        public const int NET_DVR_AUDIO_MODE_ERROR = 42;//声卡模式错误
        public const int NET_DVR_NOENOUGH_BUF = 43;//缓冲区太小
        public const int NET_DVR_CREATESOCKET_ERROR = 44;//创建SOCKET出错
        public const int NET_DVR_SETSOCKET_ERROR = 45;//设置SOCKET出错
        public const int NET_DVR_MAX_NUM = 46;//个数达到最大
        public const int NET_DVR_USERNOTEXIST = 47;//用户不存在
        public const int NET_DVR_WRITEFLASHERROR = 48;//写FLASH出错
        public const int NET_DVR_UPGRADEFAIL = 49;//DVR升级失败
        public const int NET_DVR_CARDHAVEINIT = 50;//解码卡已经初始化过
        public const int NET_DVR_PLAYERFAILED = 51;//调用播放库中某个函数失败
        public const int NET_DVR_MAX_USERNUM = 52;//设备端用户数达到最大
        public const int NET_DVR_GETLOCALIPANDMACFAIL = 53;//获得客户端的IP地址或物理地址失败
        public const int NET_DVR_NOENCODEING = 54;//该通道没有编码
        public const int NET_DVR_IPMISMATCH = 55;//IP地址不匹配
        public const int NET_DVR_MACMISMATCH = 56;//MAC地址不匹配
        public const int NET_DVR_UPGRADELANGMISMATCH = 57;//升级文件语言不匹配
        public const int NET_DVR_MAX_PLAYERPORT = 58;//播放器路数达到最大
        public const int NET_DVR_NOSPACEBACKUP = 59;//备份设备中没有足够空间进行备份
        public const int NET_DVR_NODEVICEBACKUP = 60;//没有找到指定的备份设备
        public const int NET_DVR_PICTURE_BITS_ERROR = 61;//图像素位数不符，限24色
        public const int NET_DVR_PICTURE_DIMENSION_ERROR = 62;//图片高*宽超限， 限128*256
        public const int NET_DVR_PICTURE_SIZ_ERROR = 63;//图片大小超限，限100K
        public const int NET_DVR_LOADPLAYERSDKFAILED = 64;//载入当前目录下Player Sdk出错
        public const int NET_DVR_LOADPLAYERSDKPROC_ERROR = 65;//找不到Player Sdk中某个函数入口
        public const int NET_DVR_LOADDSSDKFAILED = 66;//载入当前目录下DSsdk出错
        public const int NET_DVR_LOADDSSDKPROC_ERROR = 67;//找不到DsSdk中某个函数入口
        public const int NET_DVR_DSSDK_ERROR = 68;//调用硬解码库DsSdk中某个函数失败
        public const int NET_DVR_VOICEMONOPOLIZE = 69;//声卡被独占
        public const int NET_DVR_JOINMULTICASTFAILED = 70;//加入多播组失败
        public const int NET_DVR_CREATEDIR_ERROR = 71;//建立日志文件目录失败
        public const int NET_DVR_BINDSOCKET_ERROR = 72;//绑定套接字失败
        public const int NET_DVR_SOCKETCLOSE_ERROR = 73;//socket连接中断，此错误通常是由于连接中断或目的地不可达
        public const int NET_DVR_USERID_ISUSING = 74;//注销时用户ID正在进行某操作
        public const int NET_DVR_SOCKETLISTEN_ERROR = 75;//监听失败
        public const int NET_DVR_PROGRAM_EXCEPTION = 76;//程序异常
        public const int NET_DVR_WRITEFILE_FAILED = 77;//写文件失败
        public const int NET_DVR_FORMAT_READONLY = 78;//禁止格式化只读硬盘
        public const int NET_DVR_WITHSAMEUSERNAME = 79;//用户配置结构中存在相同的用户名
        public const int NET_DVR_DEVICETYPE_ERROR = 80;//导入参数时设备型号不匹配
        public const int NET_DVR_LANGUAGE_ERROR = 81;//导入参数时语言不匹配
        public const int NET_DVR_PARAVERSION_ERROR = 82;//导入参数时软件版本不匹配
        public const int NET_DVR_IPCHAN_NOTALIVE = 83;//预览时外接IP通道不在线
        public const int NET_DVR_RTSP_SDK_ERROR = 84;//加载高清IPC通讯库StreamTransClient失败
        public const int NET_DVR_CONVERT_SDK_ERROR = 85;//加载转码库CVT_StdToHik失败
        public const int NET_DVR_IPC_COUNT_OVERFLOW = 86;//超出最大的ip接入通道数

        public const int NET_PLAYM4_NOERROR = 500;//no error
        public const int NET_PLAYM4_PARA_OVER = 501;//input parameter is invalid;
        public const int NET_PLAYM4_ORDER_ERROR = 502;//The order of the function to be called is error.
        public const int NET_PLAYM4_TIMER_ERROR = 503;//Create multimedia clock failed;
        public const int NET_PLAYM4_DEC_VIDEO_ERROR = 504;//Decode video data failed.
        public const int NET_PLAYM4_DEC_AUDIO_ERROR = 505;//Decode audio data failed.
        public const int NET_PLAYM4_ALLOC_MEMORY_ERROR = 506;//Allocate memory failed.
        public const int NET_PLAYM4_OPEN_FILE_ERROR = 507;//Open the file failed.
        public const int NET_PLAYM4_CREATE_OBJ_ERROR = 508;//Create thread or event failed
        public const int NET_PLAYM4_CREATE_DDRAW_ERROR = 509;//Create DirectDraw object failed.
        public const int NET_PLAYM4_CREATE_OFFSCREEN_ERROR = 510;//failed when creating off-screen surface.
        public const int NET_PLAYM4_BUF_OVER = 511;//buffer is overflow
        public const int NET_PLAYM4_CREATE_SOUND_ERROR = 512;//failed when creating audio device.	
        public const int NET_PLAYM4_SET_VOLUME_ERROR = 513;//Set volume failed
        public const int NET_PLAYM4_SUPPORT_FILE_ONLY = 514;//The function only support play file.
        public const int NET_PLAYM4_SUPPORT_STREAM_ONLY = 515;//The function only support play stream.
        public const int NET_PLAYM4_SYS_NOT_SUPPORT = 516;//System not support.
        public const int NET_PLAYM4_FILEHEADER_UNKNOWN = 517;//No file header.
        public const int NET_PLAYM4_VERSION_INCORRECT = 518;//The version of decoder and encoder is not adapted.  
        public const int NET_PALYM4_INIT_DECODER_ERROR = 519;//Initialize decoder failed.
        public const int NET_PLAYM4_CHECK_FILE_ERROR = 520;//The file data is unknown.
        public const int NET_PLAYM4_INIT_TIMER_ERROR = 521;//Initialize multimedia clock failed.
        public const int NET_PLAYM4_BLT_ERROR = 522;//Blt failed.
        public const int NET_PLAYM4_UPDATE_ERROR = 523;//Update failed.
        public const int NET_PLAYM4_OPEN_FILE_ERROR_MULTI = 524;//openfile error, streamtype is multi
        public const int NET_PLAYM4_OPEN_FILE_ERROR_VIDEO = 525;//openfile error, streamtype is video
        public const int NET_PLAYM4_JPEG_COMPRESS_ERROR = 526;//JPEG compress error
        public const int NET_PLAYM4_EXTRACT_NOT_SUPPORT = 527;//Don't support the version of this file.
        public const int NET_PLAYM4_EXTRACT_DATA_ERROR = 528;//extract video data failed.

        #endregion

        #region 常量

        public const int MAX_NAMELEN = 16;     //DVR本地登陆名
        public const int MAX_RIGHT = 32;       //设备支持的权限（1-12表示本地权限，13-32表示远程权限）
        public const int NAME_LEN = 32;        //用户名长度
        public const int PASSWD_LEN = 16;      //密码长度
        public const int SERIALNO_LEN = 48;    //序列号长度
        public const int MACADDR_LEN = 6;      //mac地址长度
        public const int MAX_ETHERNET = 2;     //设备可配以太网络
        public const int PATHNAME_LEN = 128;   //路径长度

        public const int MAX_TIMESEGMENT_V30 = 8;//9000设备最大时间段数
        public const int MAX_TIMESEGMENT = 4;//8000设备最大时间段数

        public const int MAX_SHELTERNUM = 4;//8000设备最大遮挡区域数
        public const int MAX_DAYS = 7;//每周天数
        public const int PHONENUMBER_LEN = 32;//pppoe拨号号码最大长度

        public const int MAX_DISKNUM_V30 = 33;//9000设备最大硬盘数 最多33个硬盘(包括16个内置SATA硬盘、1个eSATA硬盘和16个NFS盘) 
        public const int MAX_DISKNUM = 16;//8000设备最大硬盘数
        public const int MAX_DISKNUM_V10 = 8;//1.2版本之前版本

        public const int MAX_WINDOW_V30 = 32;//9000设备本地显示最大播放窗口数
        public const int MAX_WINDOW = 16;//8000设备最大硬盘数
        public const int MAX_VGA_V30 = 4;//9000设备最大可接VGA数
        public const int MAX_VGA = 1;//8000设备最大可接VGA数

        public const int MAX_USERNUM_V30 = 32;//9000设备最大用户数
        public const int MAX_USERNUM = 16;//8000设备最大用户数
        public const int MAX_EXCEPTIONNUM_V30 = 32;//9000设备最大异常处理数
        public const int MAX_EXCEPTIONNUM = 16;//8000设备最大异常处理数
        public const int MAX_LINK = 6;//8000设备单通道最大视频流连接数

        public const int MAX_DECPOOLNUM = 4;//单路解码器每个解码通道最大可循环解码数
        public const int MAX_DECNUM = 4;//单路解码器的最大解码通道数（实际只有一个，其他三个保留）
        public const int MAX_TRANSPARENTNUM = 2;//单路解码器可配置最大透明通道数
        public const int MAX_CYCLE_CHAN = 16;//单路解码器最大轮循通道数
        public const int MAX_DIRNAME_LENGTH = 80;//最大目录长度

        public const int MAX_STRINGNUM_V30 = 8;//9000设备最大OSD字符行数数
        public const int MAX_STRINGNUM = 4;//8000设备最大OSD字符行数数
        public const int MAX_STRINGNUM_EX = 8;//8000定制扩展
        public const int MAX_AUXOUT_V30 = 16;//9000设备最大辅助输出数
        public const int MAX_AUXOUT = 4;//8000设备最大辅助输出数
        public const int MAX_HD_GROUP = 16;//9000设备最大硬盘组数
        public const int MAX_NFS_DISK = 8;//8000设备最大NFS硬盘数

        public const int IW_ESSID_MAX_SIZE = 32;//WIFI的SSID号长度
        public const int IW_ENCODING_TOKEN_MAX = 32;//WIFI密锁最大字节数
        public const int MAX_SERIAL_NUM = 64;//最多支持的透明通道路数
        public const int MAX_DDNS_NUMS = 10;//9000设备最大可配ddns数
        public const int MAX_DOMAIN_NAME = 64;// 最大域名长度
        public const int MAX_EMAIL_ADDR_LEN = 48;//最大email地址长度
        public const int MAX_EMAIL_PWD_LEN = 32;//最大email密码长度

        public const int MAXPROGRESS = 100;//回放时的最大百分率
        public const int MAX_SERIALNUM = 2;//8000设备支持的串口数 1-232， 2-485
        public const int CARDNUM_LEN = 20;//卡号长度
        public const int MAX_VIDEOOUT_V30 = 4;//9000设备的视频输出数
        public const int MAX_VIDEOOUT = 2;//8000设备的视频输出数

        public const int MAX_PRESET_V30 = 256;// 9000设备支持的云台预置点数 
        public const int MAX_TRACK_V30 = 256;// 9000设备支持的云台轨迹数 
        public const int MAX_CRUISE_V30 = 256;// 9000设备支持的云台巡航数 
        public const int MAX_PRESET = 128;// 8000设备支持的云台预置点数 
        public const int MAX_TRACK = 128;// 8000设备支持的云台轨迹数 
        public const int MAX_CRUISE = 128;// 8000设备支持的云台巡航数 

        public const int CRUISE_MAX_PRESET_NUMS = 32;// 一条巡航最多的巡航点 

        public const int MAX_SERIAL_PORT = 8;//9000设备支持232串口数
        public const int MAX_PREVIEW_MODE = 8;// 设备支持最大预览模式数目 1画面,4画面,9画面,16画面.... 
        public const int MAX_MATRIXOUT = 16;// 最大模拟矩阵输出个数 
        public const int LOG_INFO_LEN = 11840;// 日志附加信息 
        public const int DESC_LEN = 16;// 云台描述字符串长度 
        public const int PTZ_PROTOCOL_NUM = 200;// 9000最大支持的云台协议数 

        public const int MAX_AUDIO = 1;//8000语音对讲通道数
        public const int MAX_AUDIO_V30 = 2;//9000语音对讲通道数
        public const int MAX_CHANNUM = 16;//8000设备最大通道数
        public const int MAX_ALARMIN = 16;//8000设备最大报警输入数
        public const int MAX_ALARMOUT = 4;//8000设备最大报警输出数
        //9000 IPC接入
        public const int MAX_ANALOG_CHANNUM = 32;//最大32个模拟通道
        public const int MAX_ANALOG_ALARMOUT = 32;//最大32路模拟报警输出 
        public const int MAX_ANALOG_ALARMIN = 32;//最大32路模拟报警输入

        public const int MAX_IP_DEVICE = 32;//允许接入的最大IP设备数
        public const int MAX_IP_CHANNEL = 32;//允许加入的最多IP通道数
        public const int MAX_IP_ALARMIN = 128;//允许加入的最多报警输入数
        public const int MAX_IP_ALARMOUT = 64;//允许加入的最多报警输出数

        // 最大支持的通道数 最大模拟加上最大IP支持 
        public const int MAX_CHANNUM_V30 = (MAX_ANALOG_CHANNUM + MAX_IP_CHANNEL);//64
        public const int MAX_ALARMOUT_V30 = (MAX_ANALOG_ALARMOUT + MAX_IP_ALARMOUT);//96
        public const int MAX_ALARMIN_V30 = (MAX_ANALOG_ALARMIN + MAX_IP_ALARMIN);//160

        #endregion

        #region NET_DVR_IsSupport()返回值
        //1－9位分别表示以下信息（位与是TRUE)表示支持；

        public const int NET_DVR_SUPPORT_DDRAW = 0x01;//支持DIRECTDRAW，如果不支持，则播放器不能工作；
        public const int NET_DVR_SUPPORT_BLT = 0x02;//显卡支持BLT操作，如果不支持，则播放器不能工作；
        public const int NET_DVR_SUPPORT_BLTFOURCC = 0x04;//显卡BLT支持颜色转换，如果不支持，播放器会用软件方法作RGB转换；
        public const int NET_DVR_SUPPORT_BLTSHRINKX = 0x08;//显卡BLT支持X轴缩小；如果不支持，系统会用软件方法转换；
        public const int NET_DVR_SUPPORT_BLTSHRINKY = 0x10;//显卡BLT支持Y轴缩小；如果不支持，系统会用软件方法转换；
        public const int NET_DVR_SUPPORT_BLTSTRETCHX = 0x20;//显卡BLT支持X轴放大；如果不支持，系统会用软件方法转换；
        public const int NET_DVR_SUPPORT_BLTSTRETCHY = 0x40;//显卡BLT支持Y轴放大；如果不支持，系统会用软件方法转换；
        public const int NET_DVR_SUPPORT_SSE = 0x80;//CPU支持SSE指令，Intel Pentium3以上支持SSE指令；
        public const int NET_DVR_SUPPORT_MMX = 0x100;//CPU支持MMX指令集，Intel Pentium3以上支持SSE指令；

        #endregion

        #region 云台控制命令

        public const int LIGHT_PWRON = 2;// 接通灯光电源 
        public const int WIPER_PWRON = 3;// 接通雨刷开关 
        public const int FAN_PWRON = 4;// 接通风扇开关 
        public const int HEATER_PWRON = 5;// 接通加热器开关 
        public const int AUX_PWRON1 = 6;// 接通辅助设备开关 
        public const int AUX_PWRON2 = 7;// 接通辅助设备开关 
        public const int SET_PRESET = 8;// 设置预置点 
        public const int CLE_PRESET = 9;// 清除预置点 

        public const int ZOOM_IN = 11;// 焦距以速度SS变大(倍率变大) 
        public const int ZOOM_OUT = 12;// 焦距以速度SS变小(倍率变小) 
        public const int FOCUS_NEAR = 13;// 焦点以速度SS前调 
        public const int FOCUS_FAR = 14;// 焦点以速度SS后调 
        public const int IRIS_OPEN = 15;// 光圈以速度SS扩大 
        public const int IRIS_CLOSE = 16;// 光圈以速度SS缩小 

        public const int TILT_UP = 21;// 云台以SS的速度上仰 
        public const int TILT_DOWN = 22;// 云台以SS的速度下俯 
        public const int PAN_LEFT = 23;// 云台以SS的速度左转 
        public const int PAN_RIGHT = 24;// 云台以SS的速度右转 
        public const int UP_LEFT = 25;// 云台以SS的速度上仰和左转 
        public const int UP_RIGHT = 26;// 云台以SS的速度上仰和右转 
        public const int DOWN_LEFT = 27;// 云台以SS的速度下俯和左转 
        public const int DOWN_RIGHT = 28;// 云台以SS的速度下俯和右转 
        public const int PAN_AUTO = 29;// 云台以SS的速度左右自动扫描 

        public const int FILL_PRE_SEQ = 30;// 将预置点加入巡航序列 
        public const int SET_SEQ_DWELL = 31;// 设置巡航点停顿时间 
        public const int SET_SEQ_SPEED = 32;// 设置巡航速度 
        public const int CLE_PRE_SEQ = 33;// 将预置点从巡航序列中删除 
        public const int STA_MEM_CRUISE = 34;// 开始记录轨迹 
        public const int STO_MEM_CRUISE = 35;// 停止记录轨迹 
        public const int RUN_CRUISE = 36;// 开始轨迹 
        public const int RUN_SEQ = 37;// 开始巡航 
        public const int STOP_SEQ = 38;// 停止巡航 
        public const int GOTO_PRESET = 39;// 快球转到预置点 

        #endregion

        #region 回放时播放控制命令
        //NET_DVR_PlayBackControl
        //NET_DVR_PlayControlLocDisplay
        //NET_DVR_DecPlayBackCtrl的宏定义
        //具体支持查看函数说明和代码

        public const int NET_DVR_PLAYSTART = 1;//开始播放
        public const int NET_DVR_PLAYSTOP = 2;//停止播放
        public const int NET_DVR_PLAYPAUSE = 3;//暂停播放
        public const int NET_DVR_PLAYRESTART = 4;//恢复播放
        public const int NET_DVR_PLAYFAST = 5;//快放
        public const int NET_DVR_PLAYSLOW = 6;//慢放
        public const int NET_DVR_PLAYNORMAL = 7;//正常速度
        public const int NET_DVR_PLAYFRAME = 8;//单帧放
        public const int NET_DVR_PLAYSTARTAUDIO = 9;//打开声音
        public const int NET_DVR_PLAYSTOPAUDIO = 10;//关闭声音
        public const int NET_DVR_PLAYAUDIOVOLUME = 11;//调节音量
        public const int NET_DVR_PLAYSETPOS = 12;//改变文件回放的进度
        public const int NET_DVR_PLAYGETPOS = 13;//获取文件回放的进度
        public const int NET_DVR_PLAYGETTIME = 14;//获取当前已经播放的时间(按文件回放的时候有效)
        public const int NET_DVR_PLAYGETFRAME = 15;//获取当前已经播放的帧数(按文件回放的时候有效)
        public const int NET_DVR_GETTOTALFRAMES = 16;//获取当前播放文件总的帧数(按文件回放的时候有效)
        public const int NET_DVR_GETTOTALTIME = 17;//获取当前播放文件总的时间(按文件回放的时候有效)
        public const int NET_DVR_THROWBFRAME = 20;//丢B帧
        public const int NET_DVR_SETSPEED = 24;//设置码流速度
        public const int NET_DVR_KEEPALIVE = 25;//保持与设备的心跳(如果回调阻塞，建议2秒发送一次)

        #endregion

        #region 远程按键定义
        // key value send to CONFIG program

        public const int KEY_CODE_1 = 1;
        public const int KEY_CODE_2 = 2;
        public const int KEY_CODE_3 = 3;
        public const int KEY_CODE_4 = 4;
        public const int KEY_CODE_5 = 5;
        public const int KEY_CODE_6 = 6;
        public const int KEY_CODE_7 = 7;
        public const int KEY_CODE_8 = 8;
        public const int KEY_CODE_9 = 9;
        public const int KEY_CODE_0 = 10;
        public const int KEY_CODE_POWER = 11;
        public const int KEY_CODE_MENU = 12;
        public const int KEY_CODE_ENTER = 13;
        public const int KEY_CODE_CANCEL = 14;
        public const int KEY_CODE_UP = 15;
        public const int KEY_CODE_DOWN = 16;
        public const int KEY_CODE_LEFT = 17;
        public const int KEY_CODE_RIGHT = 18;
        public const int KEY_CODE_EDIT = 19;
        public const int KEY_CODE_ADD = 20;
        public const int KEY_CODE_MINUS = 21;
        public const int KEY_CODE_PLAY = 22;
        public const int KEY_CODE_REC = 23;
        public const int KEY_CODE_PAN = 24;
        public const int KEY_CODE_M = 25;
        public const int KEY_CODE_A = 26;
        public const int KEY_CODE_F1 = 27;
        public const int KEY_CODE_F2 = 28;

        // for PTZ control 
        public const int KEY_PTZ_UP_START = KEY_CODE_UP;
        public const int KEY_PTZ_UP_STOP = 32;

        public const int KEY_PTZ_DOWN_START = KEY_CODE_DOWN;
        public const int KEY_PTZ_DOWN_STOP = 33;

        public const int KEY_PTZ_LEFT_START = KEY_CODE_LEFT;
        public const int KEY_PTZ_LEFT_STOP = 34;

        public const int KEY_PTZ_RIGHT_START = KEY_CODE_RIGHT;
        public const int KEY_PTZ_RIGHT_STOP = 35;

        public const int KEY_PTZ_AP1_START = KEY_CODE_EDIT;// 光圈+ 
        public const int KEY_PTZ_AP1_STOP = 36;

        public const int KEY_PTZ_AP2_START = KEY_CODE_PAN; // 光圈- 
        public const int KEY_PTZ_AP2_STOP = 37;

        public const int KEY_PTZ_FOCUS1_START = KEY_CODE_A;// 聚焦+ 
        public const int KEY_PTZ_FOCUS1_STOP = 38;

        public const int KEY_PTZ_FOCUS2_START = KEY_CODE_M;// 聚焦- 
        public const int KEY_PTZ_FOCUS2_STOP = 39;

        public const int KEY_PTZ_B1_START = 40;// 变倍+ 
        public const int KEY_PTZ_B1_STOP = 41;

        public const int KEY_PTZ_B2_START = 42;// 变倍- 
        public const int KEY_PTZ_B2_STOP = 43;

        //9000新增
        public const int KEY_CODE_11 = 44;
        public const int KEY_CODE_12 = 45;
        public const int KEY_CODE_13 = 46;
        public const int KEY_CODE_14 = 47;
        public const int KEY_CODE_15 = 48;
        public const int KEY_CODE_16 = 49;

        #endregion

        #region 参数配置命令
        //用于NET_DVR_SetDVRConfig和NET_DVR_GetDVRConfig,注意其对应的配置结构

        public const uint NET_DVR_GET_DEVICECFG = 100;//获取设备参数
        public const uint NET_DVR_SET_DEVICECFG = 101;//设置设备参数
        public const uint NET_DVR_GET_NETCFG = 102;//获取网络参数
        public const uint NET_DVR_SET_NETCFG = 103;//设置网络参数
        public const uint NET_DVR_GET_PICCFG = 104;//获取图象参数
        public const uint NET_DVR_SET_PICCFG = 105;//设置图象参数
        public const uint NET_DVR_GET_COMPRESSCFG = 106;//获取压缩参数
        public const uint NET_DVR_SET_COMPRESSCFG = 107;//设置压缩参数
        public const uint NET_DVR_GET_RECORDCFG = 108;//获取录像时间参数
        public const uint NET_DVR_SET_RECORDCFG = 109;//设置录像时间参数
        public const uint NET_DVR_GET_DECODERCFG = 110;//获取解码器参数
        public const uint NET_DVR_SET_DECODERCFG = 111;//设置解码器参数
        public const uint NET_DVR_GET_RS232CFG = 112;//获取232串口参数
        public const uint NET_DVR_SET_RS232CFG = 113;//设置232串口参数
        public const uint NET_DVR_GET_ALARMINCFG = 114;//获取报警输入参数
        public const uint NET_DVR_SET_ALARMINCFG = 115;//设置报警输入参数
        public const uint NET_DVR_GET_ALARMOUTCFG = 116;//获取报警输出参数
        public const uint NET_DVR_SET_ALARMOUTCFG = 117;//设置报警输出参数
        public const uint NET_DVR_GET_TIMECFG = 118;//获取DVR时间
        public const uint NET_DVR_SET_TIMECFG = 119;//设置DVR时间
        public const uint NET_DVR_GET_PREVIEWCFG = 120;//获取预览参数
        public const uint NET_DVR_SET_PREVIEWCFG = 121;//设置预览参数
        public const uint NET_DVR_GET_VIDEOOUTCFG = 122;//获取视频输出参数
        public const uint NET_DVR_SET_VIDEOOUTCFG = 123;//设置视频输出参数
        public const uint NET_DVR_GET_USERCFG = 124;//获取用户参数
        public const uint NET_DVR_SET_USERCFG = 125;//设置用户参数
        public const uint NET_DVR_GET_EXCEPTIONCFG = 126;//获取异常参数
        public const uint NET_DVR_SET_EXCEPTIONCFG = 127;//设置异常参数
        public const uint NET_DVR_GET_ZONEANDDST = 128;//获取时区和夏时制参数
        public const uint NET_DVR_SET_ZONEANDDST = 129;//设置时区和夏时制参数
        public const uint NET_DVR_GET_SHOWSTRING = 130;//获取叠加字符参数
        public const uint NET_DVR_SET_SHOWSTRING = 131;//设置叠加字符参数
        public const uint NET_DVR_GET_EVENTCOMPCFG = 132;//获取事件触发录像参数
        public const uint NET_DVR_SET_EVENTCOMPCFG = 133;//设置事件触发录像参数

        public const uint NET_DVR_GET_AUXOUTCFG = 140;//获取报警触发辅助输出设置(HS设备辅助输出2006-02-28)
        public const uint NET_DVR_SET_AUXOUTCFG = 141;//设置报警触发辅助输出设置(HS设备辅助输出2006-02-28)
        public const uint NET_DVR_GET_PREVIEWCFG_AUX = 142;//获取-s系列双输出预览参数(-s系列双输出2006-04-13)
        public const uint NET_DVR_SET_PREVIEWCFG_AUX = 143;//设置-s系列双输出预览参数(-s系列双输出2006-04-13)

        public const uint NET_DVR_GET_PICCFG_EX = 200;//获取图象参数(SDK_V14扩展命令)
        public const uint NET_DVR_SET_PICCFG_EX = 201;//设置图象参数(SDK_V14扩展命令)
        public const uint NET_DVR_GET_USERCFG_EX = 202;//获取用户参数(SDK_V15扩展命令)
        public const uint NET_DVR_SET_USERCFG_EX = 203;//设置用户参数(SDK_V15扩展命令)
        public const uint NET_DVR_GET_COMPRESSCFG_EX = 204;//获取压缩参数(SDK_V15扩展命令2006-05-15)
        public const uint NET_DVR_SET_COMPRESSCFG_EX = 205;//设置压缩参数(SDK_V15扩展命令2006-05-15)


        public const uint NET_DVR_GET_NETAPPCFG = 222;//获取网络应用参数 NTP/DDNS/EMAIL
        public const uint NET_DVR_SET_NETAPPCFG = 223;//设置网络应用参数 NTP/DDNS/EMAIL
        public const uint NET_DVR_GET_NTPCFG = 224;//获取网络应用参数 NTP
        public const uint NET_DVR_SET_NTPCFG = 225;//设置网络应用参数 NTP
        public const uint NET_DVR_GET_DDNSCFG = 226;//获取网络应用参数 DDNS
        public const uint NET_DVR_SET_DDNSCFG = 227;//设置网络应用参数 DDNS
        //对应NET_DVR_EMAILPARA                        
        public const uint NET_DVR_GET_EMAILCFG = 228;//获取网络应用参数 EMAIL
        public const uint NET_DVR_SET_EMAILCFG = 229;//设置网络应用参数 EMAIL

        public const uint NET_DVR_GET_NFSCFG = 230;// NFS disk config 
        public const uint NET_DVR_SET_NFSCFG = 231;// NFS disk config 

        public const uint NET_DVR_GET_SHOWSTRING_EX = 238;//获取叠加字符参数扩展(支持8条字符)
        public const uint NET_DVR_SET_SHOWSTRING_EX = 239;//设置叠加字符参数扩展(支持8条字符)
        public const uint NET_DVR_GET_NETCFG_OTHER = 244;//获取网络参数
        public const uint NET_DVR_SET_NETCFG_OTHER = 245;//设置网络参数

        //对应NET_DVR_EMAILCFG结构
        public const uint NET_DVR_GET_EMAILPARACFG = 250;//Get EMAIL parameters
        public const uint NET_DVR_SET_EMAILPARACFG = 251;//Setup EMAIL parameters


        public const uint NET_DVR_GET_DDNSCFG_EX = 274;//获取扩展DDNS参数
        public const uint NET_DVR_SET_DDNSCFG_EX = 275;//设置扩展DDNS参数

        public const uint NET_DVR_SET_PTZPOS = 292;//云台设置PTZ位置
        public const uint NET_DVR_GET_PTZPOS = 293;//云台获取PTZ位置
        public const uint NET_DVR_GET_PTZSCOPE = 294;//云台获取PTZ范围

        public const uint NET_DVR_GET_ALLHDCFG = 300;//

        // DS9000新增命令(_V30)
        //网络(NET_DVR_NETCFG_V30结构)
        public const uint NET_DVR_GET_NETCFG_V30 = 1000;//获取网络参数
        public const uint NET_DVR_SET_NETCFG_V30 = 1001;//设置网络参数

        //图象(NET_DVR_PICCFG_V30结构)
        public const uint NET_DVR_GET_PICCFG_V30 = 1002;//获取图象参数
        public const uint NET_DVR_SET_PICCFG_V30 = 1003;//设置图象参数

        //录像时间(NET_DVR_RECORD_V30结构)
        public const uint NET_DVR_GET_RECORDCFG_V30 = 1004;//获取录像参数
        public const uint NET_DVR_SET_RECORDCFG_V30 = 1005;//设置录像参数

        //用户(NET_DVR_USER_V30结构)
        public const uint NET_DVR_GET_USERCFG_V30 = 1006;//获取用户参数
        public const uint NET_DVR_SET_USERCFG_V30 = 1007;//设置用户参数

        //9000DDNS参数配置(NET_DVR_DDNSPARA_V30结构)
        public const uint NET_DVR_GET_DDNSCFG_V30 = 1010;//获取DDNS(9000扩展)
        public const uint NET_DVR_SET_DDNSCFG_V30 = 1011;//设置DDNS(9000扩展)

        //EMAIL功能(NET_DVR_EMAILCFG_V30结构)
        public const uint NET_DVR_GET_EMAILCFG_V30 = 1012;//获取EMAIL参数 
        public const uint NET_DVR_SET_EMAILCFG_V30 = 1013;//设置EMAIL参数 

        //巡航参数 (NET_DVR_CRUISE_PARA结构)
        public const uint NET_DVR_GET_CRUISE = 1020;
        public const uint NET_DVR_SET_CRUISE = 1021;


        //报警输入结构参数 (NET_DVR_ALARMINCFG_V30结构)
        public const uint NET_DVR_GET_ALARMINCFG_V30 = 1024;
        public const uint NET_DVR_SET_ALARMINCFG_V30 = 1025;

        //报警输出结构参数 (NET_DVR_ALARMOUTCFG_V30结构)
        public const uint NET_DVR_GET_ALARMOUTCFG_V30 = 1026;
        public const uint NET_DVR_SET_ALARMOUTCFG_V30 = 1027;

        //视频输出结构参数 (NET_DVR_VIDEOOUT_V30结构)
        public const uint NET_DVR_GET_VIDEOOUTCFG_V30 = 1028;
        public const uint NET_DVR_SET_VIDEOOUTCFG_V30 = 1029;

        //叠加字符结构参数 (NET_DVR_SHOWSTRING_V30结构)
        public const uint NET_DVR_GET_SHOWSTRING_V30 = 1030;
        public const uint NET_DVR_SET_SHOWSTRING_V30 = 1031;

        //异常结构参数 (NET_DVR_EXCEPTION_V30结构)
        public const uint NET_DVR_GET_EXCEPTIONCFG_V30 = 1034;
        public const uint NET_DVR_SET_EXCEPTIONCFG_V30 = 1035;

        //串口232结构参数 (NET_DVR_RS232CFG_V30结构)
        public const uint NET_DVR_GET_RS232CFG_V30 = 1036;
        public const uint NET_DVR_SET_RS232CFG_V30 = 1037;

        //压缩参数 (NET_DVR_COMPRESSIONCFG_V30结构)
        public const uint NET_DVR_GET_COMPRESSCFG_V30 = 1040;
        public const uint NET_DVR_SET_COMPRESSCFG_V30 = 1041;

        //获取485解码器参数 (NET_DVR_DECODERCFG_V30结构)
        public const uint NET_DVR_GET_DECODERCFG_V30 = 1042;//获取解码器参数
        public const uint NET_DVR_SET_DECODERCFG_V30 = 1043;//设置解码器参数

        //获取预览参数 (NET_DVR_PREVIEWCFG_V30结构)
        public const uint NET_DVR_GET_PREVIEWCFG_V30 = 1044;//获取预览参数
        public const uint NET_DVR_SET_PREVIEWCFG_V30 = 1045;//设置预览参数

        //辅助预览参数 (NET_DVR_PREVIEWCFG_AUX_V30结构)
        public const uint NET_DVR_GET_PREVIEWCFG_AUX_V30 = 1046;//获取辅助预览参数
        public const uint NET_DVR_SET_PREVIEWCFG_AUX_V30 = 1047;//设置辅助预览参数

        //IP接入配置参数 （NET_DVR_IPPARACFG结构）
        public const uint NET_DVR_GET_IPPARACFG = 1048;//获取IP接入配置信息 
        public const uint NET_DVR_SET_IPPARACFG = 1049;//设置IP接入配置信息

        //IP报警输入接入配置参数 （NET_DVR_IPALARMINCFG结构）
        public const uint NET_DVR_GET_IPALARMINCFG = 1050;//获取IP报警输入接入配置信息 
        public const uint NET_DVR_SET_IPALARMINCFG = 1051;//设置IP报警输入接入配置信息

        //IP报警输出接入配置参数 （NET_DVR_IPALARMOUTCFG结构）
        public const uint NET_DVR_GET_IPALARMOUTCFG = 1052;//获取IP报警输出接入配置信息 
        public const uint NET_DVR_SET_IPALARMOUTCFG = 1053;//设置IP报警输出接入配置信息

        //硬盘管理的参数获取 (NET_DVR_HDCFG结构)
        public const uint NET_DVR_GET_HDCFG = 1054;//获取硬盘管理配置参数
        public const uint NET_DVR_SET_HDCFG = 1055;//设置硬盘管理配置参数
        //盘组管理的参数获取 (NET_DVR_HDGROUP_CFG结构)
        public const uint NET_DVR_GET_HDGROUP_CFG = 1056;//获取盘组管理配置参数
        public const uint NET_DVR_SET_HDGROUP_CFG = 1057;//设置盘组管理配置参数

        //设备编码类型配置(NET_DVR_COMPRESSION_AUDIO结构)
        public const uint NET_DVR_GET_COMPRESSCFG_AUD = 1058;//获取设备语音对讲编码参数
        public const uint NET_DVR_SET_COMPRESSCFG_AUD = 1059;//设置设备语音对讲编码参数

        #endregion

        #region 查找文件和日志函数返回值
        public const uint NET_DVR_FILE_SUCCESS = 1000;//获得文件信息
        public const uint NET_DVR_FILE_NOFIND = 1001;//没有文件
        public const uint NET_DVR_ISFINDING = 1002;//正在查找文件
        public const uint NET_DVR_NOMOREFILE = 1003;//查找文件时没有更多的文件
        public const uint NET_DVR_FILE_EXCEPTION = 1004;//查找文件时异常
        #endregion

        #region 回调函数类型
        public const uint COMM_ALARM = 0x1100;//8000报警信息主动上传
        public const uint COMM_TRADEINFO = 0x1500;//ATMDVR主动上传交易信息

        public const uint COMM_ALARM_V30 = 0x4000;//9000报警信息主动上传
        public const uint COMM_IPCCFG = 0x4001;//9000设备IPC接入配置改变报警信息主动上传


        //操作异常类型(消息方式, 回调方式(保留))
        public const uint EXCEPTION_EXCHANGE = 0x8000;//用户交互时异常
        public const uint EXCEPTION_AUDIOEXCHANGE = 0x8001;//语音对讲异常
        public const uint EXCEPTION_ALARM = 0x8002;//报警异常
        public const uint EXCEPTION_PREVIEW = 0x8003;//网络预览异常
        public const uint EXCEPTION_SERIAL = 0x8004;//透明通道异常
        public const uint EXCEPTION_RECONNECT = 0x8005;//预览时重连
        public const uint EXCEPTION_ALARMRECONNECT = 0x8006;//报警时重连
        public const uint EXCEPTION_SERIALRECONNECT = 0x8007;//透明通道重连
        public const uint EXCEPTION_PLAYBACK = 0x8010;//回放异常
        public const uint EXCEPTION_DISKFMT = 0x8011;//硬盘格式化

        //预览回调函数
        public const uint NET_DVR_SYSHEAD = 1;//系统头数据
        public const uint NET_DVR_STREAMDATA = 2;//视频流数据（包括复合流和音视频分开的视频流数据）
        public const uint NET_DVR_AUDIOSTREAMDATA = 3;//音频流数据
        public const uint NET_DVR_STD_VIDEODATA = 4;//标准视频流数据
        public const uint NET_DVR_STD_AUDIODATA = 5;//标准音频流数据

        //回调预览中的状态和消息
        public const uint NET_DVR_REALPLAYEXCEPTION = 111;//预览异常
        public const uint NET_DVR_REALPLAYNETCLOSE = 112;//预览时连接断开
        public const uint NET_DVR_REALPLAY5SNODATA = 113;//预览5s没有收到数据
        public const uint NET_DVR_REALPLAYRECONNECT = 114;//预览重连

        //回放回调函数
        public const uint NET_DVR_PLAYBACKOVER = 101;//回放数据播放完毕
        public const uint NET_DVR_PLAYBACKEXCEPTION = 102;//回放异常
        public const uint NET_DVR_PLAYBACKNETCLOSE = 103;//回放时候连接断开
        public const uint NET_DVR_PLAYBACK5SNODATA = 104;//回放5s没有收到数据

        #endregion

        #region 设备型号(DVR类型)
        // 设备类型

        public const uint DVR = 1;//对尚未定义的dvr类型返回NETRET_DVR
        public const uint ATMDVR = 2;//atm dvr
        public const uint DVS = 3;//DVS
        public const uint DEC = 4;// 6001D 
        public const uint ENC_DEC = 5;// 6001F 
        public const uint DVR_HC = 6;//8000HC
        public const uint DVR_HT = 7;//8000HT
        public const uint DVR_HF = 8;//8000HF
        public const uint DVR_HS = 9;// 8000HS DVR(no audio) 
        public const uint DVR_HTS = 10;// 8016HTS DVR(no audio) 
        public const uint DVR_HB = 11;// HB DVR(SATA HD) 
        public const uint DVR_HCS = 12;// 8000HCS DVR 
        public const uint DVS_A = 13;// 带ATA硬盘的DVS 
        public const uint DVR_HC_S = 14;// 8000HC-S 
        public const uint DVR_HT_S = 15;// 8000HT-S 
        public const uint DVR_HF_S = 16;// 8000HF-S 
        public const uint DVR_HS_S = 17;// 8000HS-S 
        public const uint ATMDVR_S = 18;// ATM-S 
        public const uint LOWCOST_DVR = 19;//7000H系列
        public const uint DEC_MAT = 20;//多路解码器
        public const uint DVR_MOBILE = 21;// mobile DVR                  
        public const uint DVR_HD_S = 22;// 8000HD-S 
        public const uint DVR_HD_SL = 23;// 8000HD-SL 
        public const uint DVR_HC_SL = 24;// 8000HC-SL 
        public const uint DVR_HS_ST = 25;// 8000HS_ST 
        public const uint DVS_HW = 26;// 6000HW 
        public const uint IPCAM = 30;//IP 摄像机
        public const uint MEGA_IPCAM = 31;//X52MF系列,752MF,852MF
        public const uint IPCAM_X62MF = 32;//X62MF系列可接入9000设备,762MF,862MF
        public const uint IPDOME = 40;//IP标清快球
        public const uint MEGA_IPDOME = 41;//IP高清快球
        public const uint IPMOD = 50;//IP 模块
        public const uint DS71XX_H = 71;// DS71XXH_S 
        public const uint DS72XX_H_S = 72;// DS72XXH_S 
        public const uint DS73XX_H_S = 73;// DS73XXH_S 
        public const uint DS81XX_HS_S = 81;// DS81XX_HS_S 
        public const uint DS81XX_HL_S = 82;// DS81XX_HL_S 
        public const uint DS81XX_HC_S = 83;// DS81XX_HC_S 
        public const uint DS81XX_HD_S = 84;// DS81XX_HD_S 
        public const uint DS81XX_HE_S = 85;// DS81XX_HE_S 
        public const uint DS81XX_HF_S = 86;// DS81XX_HF_S 
        public const uint DS81XX_AH_S = 87;// DS81XX_AH_S 
        public const uint DS81XX_AHF_S = 88;// DS81XX_AHF_S 
        public const uint DS90XX_HF_S = 90;//DS90XX_HF_S
        public const uint DS91XX_HF_S = 91;//DS91XX_HF_S
        public const uint DS91XX_HD_S = 92;//91XXHD-S(MD)

        #endregion

        #region 参数配置结构、参数(其中_V30为9000新增)

        // 校时结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_TIME
        {
            public uint dwYear;		//年
            public uint dwMonth;	//月
            public uint dwDay;		//日
            public uint dwHour;		//时
            public uint dwMinute;	//分
            public uint dwSecond;	//秒
        }

        // 时间段(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SCHEDTIME
        {
            //开始时间
            public byte byStartHour;
            public byte byStartMin;
            //结束时间
            public byte byStopHour;
            public byte byStopMin;
        }

        #region 设备报警和异常处理方式
        public const uint NOACTION = 0x0;//无响应
        public const uint WARNONMONITOR = 0x1;//监视器上警告
        public const uint WARNONAUDIOOUT = 0x2;//声音警告
        public const uint UPTOCENTER = 0x4;//上传中心
        public const uint TRIGGERALARMOUT = 0x8;//触发报警输出
        #endregion

        // 报警和异常处理结构(子结构)(多处使用)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HANDLEEXCEPTION_V30
        {
            public uint dwHandleType;	//处理方式,处理方式的"或"结果
            //0x00: 无响应
            //0x01: 监视器上警告
            //0x02: 声音警告
            //0x04: 上传中心
            //0x08: 触发报警输出
            //0x10: Jpeg抓图并上传EMail
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30)]
            public byte[] byRelAlarmOut;
            //报警触发的输出通道,报警触发的输出,为1表示触发该输出
        }

        // 报警和异常处理结构(子结构)(多处使用)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HANDLEEXCEPTION
        {
            public uint dwHandleType;			//处理方式,处理方式的"或"结果
            //0x00: 无响应
            //0x01: 监视器上警告
            //0x02: 声音警告
            //0x04: 上传中心
            //0x08: 触发报警输出
            //0x10: Jpeg抓图并上传EMail
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT)]
            public byte[] byRelAlarmOut;		//报警触发的输出通道,报警触发的输出,为1表示触发该输出
        }

        // DVR设备参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICECFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sDVRName;     //DVR名称
            public uint dwDVRID;				//DVR ID,用于遥控器 //V1.4(0-99), V1.5(0-255)
            public uint dwRecycleRecord;		//是否循环录像,0:不是; 1:是
            //以下不可更改
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SERIALNO_LEN)]
            public string sSerialNumber;  //序列号
            public uint dwSoftwareVersion;			//软件版本号,高16位是主版本,低16位是次版本
            public uint dwSoftwareBuildDate;			//软件生成日期,0xYYYYMMDD
            public uint dwDSPSoftwareVersion;		//DSP软件版本,高16位是主版本,低16位是次版本
            public uint dwDSPSoftwareBuildDate;		// DSP软件生成日期,0xYYYYMMDD
            public uint dwPanelVersion;				// 前面板版本,高16位是主版本,低16位是次版本
            public uint dwHardwareVersion;	// 硬件版本,高16位是主版本,低16位是次版本
            public byte byAlarmInPortNum;		//DVR报警输入个数
            public byte byAlarmOutPortNum;		//DVR报警输出个数
            public byte byRS232Num;			//DVR 232串口个数
            public byte byRS485Num;			//DVR 485串口个数
            public byte byNetworkPortNum;		//网络口个数
            public byte byDiskCtrlNum;			//DVR 硬盘控制器个数
            public byte byDiskNum;				//DVR 硬盘个数
            public byte byDVRType;				//DVR类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				//DVR 通道个数
            public byte byStartChan;			//起始通道号,例如DVS-1,DVR - 1
            public byte byDecordChans;			//DVR 解码路数
            public byte byVGANum;				//VGA口的个数
            public byte byUSBNum;				//USB口的个数
            public byte byAuxoutNum;	//辅口的个数
            public byte byAudioNum;		//语音口的个数
            public byte byIPChanNum;	//最大数字通道数
        }

        // IP地址
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPADDR
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sIpV4;     //IPv4地址
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] byRes;     //保留
        }

        // 网络数据结构(子结构)(9000扩展) NET_DVR_ETHERNET_V30
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ETHERNET_V30
        {
            public NET_DVR_IPADDR struDVRIP;      //DVR IP地址
            public NET_DVR_IPADDR struDVRIPMask;  //DVR IP地址掩码
            public uint dwNetInterface;   		  //网络接口1-10MBase-T 2-10MBase-T全双工 3-100MBase-TX 4-100M全双工 5-10M/100M自适应
            public ushort wDVRPort;				  //端口号
            public ushort wMTU;					  //增加MTU设置，默认1500。
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMACAddr;			  // 物理地址
        }

        // 网络数据结构(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ETHERNET
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;          //DVR IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIPMask;      // DVR IP地址掩码
            public uint dwNetInterface;   //网络接口 1-10MBase-T 2-10MBase-T全双工 3-100MBase-TX 4-100M全双工 5-10M/100M自适应
            public ushort wDVRPort;		//端口号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMACAddr;		//服务器的物理地址
        }

        // pppoe结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PPPOECFG
        {
            public uint dwPPPOE;								//0-不启用,1-启用
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sPPPoEUser;							//PPPoE用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPPPoEPassword;						// PPPoE密码
            public NET_DVR_IPADDR struPPPoEIP;					//PPPoE IP地址
        }

        // 网络配置结构(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NETCFG_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ETHERNET)]
            public NET_DVR_ETHERNET_V30[] struEtherNet;		//以太网口
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public NET_DVR_IPADDR[] struRes1;					//保留
            public NET_DVR_IPADDR struAlarmHostIpAddr;					// 报警主机IP地址 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public ushort[] wRes2;								// 保留 
            public ushort wAlarmHostIpPort;								// 报警主机端口号 
            public byte byUseDhcp;                                      // 是否启用DHCP 0xff-无效 0-不启用 1-启用
            public byte byRes3;
            public NET_DVR_IPADDR struDnsServer1IpAddr;					// 域名服务器1的IP地址 
            public NET_DVR_IPADDR struDnsServer2IpAddr;					// 域名服务器2的IP地址 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
            public string byIpResolver;					// IP解析服务器域名或IP地址 
            public ushort wIpResolverPort;								// IP解析服务器端口号 
            public ushort wHttpPortNo;									// HTTP端口号 
            public NET_DVR_IPADDR struMulticastIpAddr;					// 多播组地址 
            public NET_DVR_IPADDR struGatewayIpAddr;						// 网关地址 
            public NET_DVR_PPPOECFG struPPPoE;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;
        }

        // 网络配置结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NETCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ETHERNET)]
            public NET_DVR_ETHERNET[] struEtherNet;		// 以太网口 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sManageHostIP;		//远程管理主机地址
            public ushort wManageHostPort; //远程管理主机端口号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDNSIP;            //DNS服务器地址  
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sMultiCastIP;     //多播组地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sGatewayIP;       	//网关地址 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sNFSIP;			//NAS主机IP地址	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PATHNAME_LEN)]
            public string sNFSDirectory;//NAS目录
            public uint dwPPPOE;				//0-不启用,1-启用
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sPPPoEUser;	//PPPoE用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPPPoEPassword;// PPPoE密码
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sPPPoEIP;			//PPPoE IP地址(只读)
            public ushort wHttpPort;				//HTTP端口号
        }

        #region 通道图象结构

        //移动侦测(子结构)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MOTION_V30
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 96)]
            public byte[] byMotionScope;	//侦测区域,0-96位,表示64行,共有96*64个小宏块,为1表示是移动侦测区域,0-表示不是
            public byte byMotionSensitive;		//移动侦测灵敏度, 0 - 5,越高越灵敏,oxff关闭
            public byte byEnableHandleMotion;	// 是否处理移动侦测 0－否 1－是
            public byte byPrecision;			// 移动侦测算法的进度: 0--16*16, 1--32*32, 2--64*64 ... 
            public byte reservedData;
            public NET_DVR_HANDLEEXCEPTION_V30 struMotionHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byRelRecordChan; //报警触发的录象通道,为1表示触发该通道	
        }

        //移动侦测(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MOTION
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18 * 22)]
            public byte[] byMotionScope;	//侦测区域,共有22*18个小宏块,为1表示该宏块是移动侦测区域,0-表示不是
            public byte byMotionSensitive;		//移动侦测灵敏度, 0 - 5,越高越灵敏,0xff关闭
            public byte byEnableHandleMotion;	// 是否处理移动侦测 
            public NET_DVR_HANDLEEXCEPTION strMotionHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byRelRecordChan; //报警触发的录象通道,为1表示触发该通道	
        }

        //遮挡报警(子结构)(9000扩展)  区域大小704*576
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HIDEALARM_V30
        {
            public uint dwEnableHideAlarm;			// 是否启动遮挡报警 ,0-否,1-低灵敏度 2-中灵敏度 3-高灵敏度 
            public ushort wHideAlarmAreaTopLeftX;			// 遮挡区域的x坐标 
            public ushort wHideAlarmAreaTopLeftY;			// 遮挡区域的y坐标 
            public ushort wHideAlarmAreaWidth;			// 遮挡区域的宽 
            public ushort wHideAlarmAreaHeight;			//遮挡区域的高
            public NET_DVR_HANDLEEXCEPTION_V30 strHideAlarmHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
        }

        //遮挡报警(子结构)  区域大小704*576
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HIDEALARM
        {
            public uint dwEnableHideAlarm;			// 是否启动遮挡报警 ,0-否,1-低灵敏度 2-中灵敏度 3-高灵敏度 
            public ushort wHideAlarmAreaTopLeftX;			// 遮挡区域的x坐标 
            public ushort wHideAlarmAreaTopLeftY;			// 遮挡区域的y坐标 
            public ushort wHideAlarmAreaWidth;			// 遮挡区域的宽 
            public ushort wHideAlarmAreaHeight;			//遮挡区域的高
            public NET_DVR_HANDLEEXCEPTION strHideAlarmHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
        }

        //信号丢失报警(子结构)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VILOST_V30
        {
            public byte byEnableHandleVILost;	// 是否处理信号丢失报警 
            public NET_DVR_HANDLEEXCEPTION_V30 strVILostHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
        }

        //信号丢失报警(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VILOST
        {
            public byte byEnableHandleVILost;	// 是否处理信号丢失报警 
            public NET_DVR_HANDLEEXCEPTION strVILostHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
        }

        //遮挡区域(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SHELTER
        {
            public ushort wHideAreaTopLeftX;				// 遮挡区域的x坐标 
            public ushort wHideAreaTopLeftY;				// 遮挡区域的y坐标 
            public ushort wHideAreaWidth;				// 遮挡区域的宽 
            public ushort wHideAreaHeight;				//遮挡区域的高
        }

        //color
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COLOR
        {
            public byte byBrightness;  	//亮度,0-255
            public byte byContrast;    	//对比度,0-255
            public byte bySaturation;  	//饱和度,0-255
            public byte byHue;    			//色调,0-255
        }

        //通道图象结构(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PICCFG_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sChanName;
            public uint dwVideoFormat;	// 只读 视频制式 1-NTSC 2-PAL
            public NET_DVR_COLOR struColor;//	图像参数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
            public byte[] reservedData; //保留
            //显示通道名
            public uint dwShowChanName; // 预览的图象上是否显示通道名称,0-不显示,1-显示 区域大小704*576
            public ushort wShowNameTopLeftX;				// 通道名称显示位置的x坐标 
            public ushort wShowNameTopLeftY;				// 通道名称显示位置的y坐标 
            //视频信号丢失报警
            public NET_DVR_VILOST_V30 struVILost;
            public NET_DVR_VILOST_V30 struRes;		//保留
            //移动侦测
            public NET_DVR_MOTION_V30 struMotion;
            //遮挡报警
            public NET_DVR_HIDEALARM_V30 struHideAlarm;
            //遮挡  区域大小704*576
            public uint dwEnableHide;		// 是否启动遮挡 ,0-否,1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SHELTERNUM)]
            public NET_DVR_SHELTER[] struShelter;
            //OSD
            public uint dwShowOsd;// 预览的图象上是否显示OSD,0-不显示,1-显示 区域大小704*576
            public ushort wOSDTopLeftX;				// OSD的x坐标 
            public ushort wOSDTopLeftY;				// OSD的y坐标 
            public byte byOSDType;					// OSD类型(主要是年月日格式) 
            // 0: XXXX-XX-XX 年月日 
            // 1: XX-XX-XXXX 月日年 
            // 2: XXXX年XX月XX日 
            // 3: XX月XX日XXXX年 
            // 4: XX-XX-XXXX 日月年
            // 5: XX日XX月XXXX年
            public byte byDispWeek;				// 是否显示星期 
            public byte byOSDAttrib;				// OSD属性:透明，闪烁 
            // 0: 不显示OSD
            // 1: 透明,闪烁 
            // 2: 透明,不闪烁 
            // 3: 闪烁,不透明 
            // 4: 不透明,不闪烁 
            public byte byHourOsdType;          // OSD小时制:0-24小时制,1-12小时制
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;
        }

        //通道图象结构SDK_V14扩展
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PICCFG_EX
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sChanName;
            public uint dwVideoFormat;	// 只读 视频制式 1-NTSC 2-PAL
            public byte byBrightness;  	//亮度,0-255
            public byte byContrast;    	//对比度,0-255
            public byte bySaturation;  	//饱和度,0-255 
            public byte byHue;    			//色调,0-255
            //显示通道名
            public uint dwShowChanName; // 预览的图象上是否显示通道名称,0-不显示,1-显示 区域大小704*576
            public ushort wShowNameTopLeftX;				// 通道名称显示位置的x坐标 
            public ushort wShowNameTopLeftY;				// 通道名称显示位置的y坐标 
            //信号丢失报警
            public NET_DVR_VILOST struVILost;
            //移动侦测
            public NET_DVR_MOTION struMotion;
            //遮挡报警
            public NET_DVR_HIDEALARM struHideAlarm;
            //遮挡  区域大小704*576
            public uint dwEnableHide;		// 是否启动遮挡 ,0-否,1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SHELTERNUM)]
            public NET_DVR_SHELTER[] struShelter;
            //OSD
            public uint dwShowOsd;// 预览的图象上是否显示OSD,0-不显示,1-显示 区域大小704*576
            public ushort wOSDTopLeftX;				// OSD的x坐标 
            public ushort wOSDTopLeftY;				// OSD的y坐标 
            public byte byOSDType;					// OSD类型(主要是年月日格式) 
            // 0: XXXX-XX-XX 年月日 
            // 1: XX-XX-XXXX 月日年 
            // 2: XXXX年XX月XX日 
            // 3: XX月XX日XXXX年 
            // 4: XX-XX-XXXX 日月年
            // 5: XX日XX月XXXX年
            public byte byDispWeek;				// 是否显示星期 
            public byte byOSDAttrib;				// OSD属性:透明，闪烁 
            // 0: 不显示OSD
            // 1: 透明,闪烁 
            // 2: 透明,不闪烁 
            // 3: 闪烁,不透明 
            // 4: 不透明,不闪烁 
            public byte byHourOsdType;          // OSD小时制:0-24小时制,1-12小时制
        }

        //通道图象结构(SDK_V13及之前版本)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PICCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sChanName;
            public uint dwVideoFormat;	// 只读：视频制式 1-NTSC 2-PAL
            public byte byBrightness;  	//亮度,0-255
            public byte byContrast;    	//对比度,0-255
            public byte bySaturation;  	//饱和度,0-255 
            public byte byHue;    			//色调,0-255
            //显示通道名
            public uint dwShowChanName; // 预览的图象上是否显示通道名称,0-不显示,1-显示 区域为704*576 
            public ushort wShowNameTopLeftX;				// 通道名称显示位置的x坐标 
            public ushort wShowNameTopLeftY;				// 通道名称显示位置的y坐标 
            //信号丢失报警
            public NET_DVR_VILOST struVILost;
            //移动侦测
            public NET_DVR_MOTION struMotion;
            //遮挡报警
            public NET_DVR_HIDEALARM struHideAlarm;
            //遮挡
            public uint dwEnableHide;		// 是否启动遮挡 ,0-否,1-是 区域为704*576
            public ushort wHideAreaTopLeftX;				// 遮挡区域的x坐标 
            public ushort wHideAreaTopLeftY;				// 遮挡区域的y坐标 
            public ushort wHideAreaWidth;				// 遮挡区域的宽 
            public ushort wHideAreaHeight;				//遮挡区域的高
            //OSD
            public uint dwShowOsd;	// 预览的图象上是否显示OSD,0-不显示,1-显示
            public ushort wOSDTopLeftX;				// OSD的x坐标 
            public ushort wOSDTopLeftY;				// OSD的y坐标 
            public byte byOSDType;					// OSD类型(主要是年月日格式) 
            // 0: XXXX-XX-XX 年月日 
            // 1: XX-XX-XXXX 月日年 
            // 2: XXXX年XX月XX日 
            // 3: XX月XX日XXXX年 
            // 4: XX-XX-XXXX 日月年
            // 5: XX日XX月XXXX年
            public byte byDispWeek;				// 是否显示星期 
            public byte byOSDAttrib;				// OSD属性:透明，闪烁 
            // 0: 不显示OSD
            // 1: 透明,闪烁 
            // 2: 透明,不闪烁 
            // 3: 闪烁,不透明 
            // 4: 不透明,不闪烁 
            public byte reservedData2;
        }

        #endregion

        #region 录像相关结构

        //码流压缩参数(子结构)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO_V30
        {
            public byte byStreamType;		//码流类型 0-视频流, 1-复合流, 表示事件压缩参数时最高位表示是否启用压缩参数
            public byte byResolution;  	//分辨率0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF 5（保留）16-VGA（640*480） 17-UXGA（1600*1200） 18-SVGA （800*600）19-HD720p（1280*720）20-XVGA  21-HD900p
            public byte byBitrateType;		//码率类型0:变码率，1:定码率
            public byte byPicQuality;		//图象质量 0-最好 1-次好 2-较好 3-一般 4-较差 5-差	
            public uint dwVideoBitrate; 	//视频码率 0-保留 1-16K 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
            // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
            //最高位(31位)置成1表示是自定义码流, 0-30位表示码流值。
            public uint dwVideoFrameRate;   //帧率 0-全部; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20; V2.0版本中新加14-15; 15-18; 16-22;
            public ushort wIntervalFrameI;  //I帧间隔
                                            //2006-08-11 增加单P帧的配置接口，可以改善实时流延时问题
            public byte byIntervalBPFrame;//0-BBP帧; 1-BP帧; 2-单P帧
            public byte byres1;        //保留
            public byte byVideoEncType;   //视频编码类型 0 hik264;1标准h264; 2标准mpeg4;
            public byte byAudioEncType;   //音频编码类型 0－OggVorbis
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] byres;//这里保留音频的压缩参数
        }

        //通道压缩参数(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_V30
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_V30 struNormHighRecordPara; //录像 对应8000的普通
            public NET_DVR_COMPRESSION_INFO_V30 struRes;//保留
            public NET_DVR_COMPRESSION_INFO_V30 struEventRecordPara;       //事件触发压缩参数
            public NET_DVR_COMPRESSION_INFO_V30 struNetPara;	//网传(子码流)
        }

        //码流压缩参数(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO
        {
            public byte byStreamType;		//码流类型0-视频流,1-复合流
            public byte byResolution;  	//分辨率0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF
            public byte byBitrateType;		//码率类型0:变码率，1:定码率
            public byte byPicQuality;		//图象质量 0-最好 1-次好 2-较好 3-一般 4-较差 5-差	
            public uint dwVideoBitrate; 	//视频码率 0-保留 1-保留 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
            // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
            //最高位(31位)置成1表示是自定义码流, 0-30位表示码流值(MIN-16K MAX-8192K)。
            public uint dwVideoFrameRate;	//帧率 0-全部; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20;
        }

        //通道压缩参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO struRecordPara; //录像
            public NET_DVR_COMPRESSION_INFO struNetPara;	//网传
        }

        //码流压缩参数(子结构)(扩展) 增加I帧间隔
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO_EX
        {
            public byte byStreamType;		//码流类型0-视频流, 1-复合流
            public byte byResolution;  	//分辨率0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF, 5-2QCIF(352X144)(车载专用)
            public byte byBitrateType;		//码率类型0:变码率，1:定码率
            public byte byPicQuality;		//图象质量 0-最好 1-次好 2-较好 3-一般 4-较差 5-差
            public uint dwVideoBitrate; 	//视频码率 0-保留 1-16K(保留) 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
            // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
            //最高位(31位)置成1表示是自定义码流, 0-30位表示码流值(MIN-32K MAX-8192K)。
            public uint dwVideoFrameRate;	//帧率 0-全部; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20, //V2.0增加14-15, 15-18, 16-22;
            public ushort wIntervalFrameI;  //I帧间隔
            //2006-08-11 增加单P帧的配置接口，可以改善实时流延时问题
            public byte byIntervalBPFrame;//0-BBP帧; 1-BP帧; 2-单P帧
            public byte byRes;
        }

        //通道压缩参数(扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_EX
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_EX struRecordPara; //录像
            public NET_DVR_COMPRESSION_INFO_EX struNetPara;	//网传
        }

        //时间段录像参数配置(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RECORDSCHED
        {
            public NET_DVR_SCHEDTIME struRecordTime;
            public byte byRecordType;	//0:定时录像，1:移动侦测，2:报警录像，3:动测|报警，4:动测&报警, 5:命令触发, 6: 手动录像
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] reservedData;
        }

        //全天录像参数配置(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RECORDDAY
        {
            public ushort wAllDayRecord;				// 是否全天录像 
            public byte byRecordType;				// 录象类型 0:定时录像，1:移动侦测，2:报警录像，3:动测|报警，4:动测&报警 5:命令触发, 6: 手动录像
            public byte reservedData;
        }

        //通道录像参数配置(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RECORD_V30
        {
            public uint dwSize;
            public uint dwRecord;  //是否录像 0-否 1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS)]
            public NET_DVR_RECORDDAY[] struRecAllDay;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_RECORDSCHED[] struRecordSched;
            public uint dwRecordTime;	// 录象延时长度 0-5秒， 1-20秒， 2-30秒， 3-1分钟， 4-2分钟， 5-5分钟， 6-10分钟
            public uint dwPreRecordTime;	// 预录时间 0-不预录 1-5秒 2-10秒 3-15秒 4-20秒 5-25秒 6-30秒 7-0xffffffff(尽可能预录)
            public uint dwRecorderDuration;	// 录像保存的最长时间
            public byte byRedundancyRec;	//是否冗余录像,重要数据双备份：0/1
            public byte byAudioRec;		//录像时复合流编码时是否记录音频数据：国外有此法规
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] byReserve;
        }

        //通道录像参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RECORD
        {
            public uint dwSize;
            public uint dwRecord;  //是否录像 0-否 1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS)]
            public NET_DVR_RECORDDAY[] struRecAllDay;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_RECORDSCHED[] struRecordSched;
            public uint dwRecordTime;	// 录象时间长度 
            public uint dwPreRecordTime;	// 预录时间 0-不预录 1-5秒 2-10秒 3-15秒 4-20秒 5-25秒 6-30秒 7-0xffffffff(尽可能预录) 
        }

        #endregion

        #region 云台解码器相关结构

        //云台协议表结构配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PTZ_PROTOCOL
        {
            public uint dwType;             //解码器类型值，从1开始连续递增
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DESC_LEN)]
            public string byDescribe;       //解码器的描述符，和8000中的一致
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PTZCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PTZ_PROTOCOL_NUM)]
            public NET_DVR_PTZ_PROTOCOL[] struPtz;//最大200中PTZ协议
            public uint dwPtzNum;           //有效的ptz协议数目，从0开始(即计算时加1)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byRes;
        }

        //通道解码器(云台)参数配置(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERCFG_V30
        {
            public uint dwSize;
            public uint dwBaudRate;//波特率(bps)，0－50，1－75，2－110，3－150，4－300，5－600，6－1200，7－2400，8－4800，9－9600，10－19200， 11－38400，12－57600，13－76800，14－115.2k;
            public byte byDataBit;// 数据有几位 0－5位，1－6位，2－7位，3－8位;
            public byte byStopBit;// 停止位 0－1位，1－2位;
            public byte byParity;// 校验 0－无校验，1－奇校验，2－偶校验;
            public byte byFlowcontrol;// 0－无，1－软流控,2-硬流控
            public ushort wDecoderType;//解码器类型, 从0开始，对应ptz协议列表
            public ushort wDecoderAddress;	//解码器地址:0 - 255
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PRESET_V30)]
            public byte[] bySetPreset;		// 预置点是否设置,0-没有设置,1-设置
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CRUISE_V30)]
            public byte[] bySetCruise;		// 巡航是否设置: 0-没有设置,1-设置 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_TRACK_V30)]
            public byte[] bySetTrack;		// 轨迹是否设置,0-没有设置,1-设置
        }

        //通道解码器(云台)参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERCFG
        {
            public uint dwSize;
            public uint dwBaudRate;//波特率(bps)，0－50，1－75，2－110，3－150，4－300，5－600，6－1200，7－2400，8－4800，9－9600，10－19200， 11－38400，12－57600，13－76800，14－115.2k;
            public byte byDataBit;// 数据有几位 0－5位，1－6位，2－7位，3－8位;
            public byte byStopBit;// 停止位 0－1位，1－2位;
            public byte byParity;// 校验 0－无校验，1－奇校验，2－偶校验;
            public byte byFlowcontrol;// 0－无，1－软流控,2-硬流控
            public ushort wDecoderType;//解码器类型, 0－YouLi，1－LiLin-1016，2－LiLin-820，3－Pelco-p，4－DM DynaColor，5－HD600，6－JC-4116，7－Pelco-d WX，8－Pelco-d PICO
            public ushort wDecoderAddress;	//解码器地址:0 - 255
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PRESET)]
            public byte[] bySetPreset;		// 预置点是否设置,0-没有设置,1-设置
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CRUISE)]
            public byte[] bySetCruise;		// 巡航是否设置: 0-没有设置,1-设置 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_TRACK)]
            public byte[] bySetTrack;		// 轨迹是否设置,0-没有设置,1-设置
        }

        #endregion

        #region 串口相关结构

        //ppp参数配置(子结构)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PPPCFG_V30
        {
            public NET_DVR_IPADDR struRemoteIP;	//远端IP地址
            public NET_DVR_IPADDR struLocalIP;		//本地IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIPMask;         //本地IP地址掩码
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUsername;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            public byte byPPPMode;            //PPP模式, 0－主动，1－被动	
            public byte byRedial;            //是否回拨 ：0-否,1-是
            public byte byRedialMode;        //回拨模式,0-由拨入者指定,1-预置回拨号码
            public byte byDataEncrypt;	     //数据加密,0-否,1-是
            public uint dwMTU;              //MTU
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PHONENUMBER_LEN)]
            public string sTelephoneNumber;   //电话号码
        }

        //ppp参数配置(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PPPCFG
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteIP;            //远端IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIP;             //本地IP地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIPMask;         //本地IP地址掩码
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUsername;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            public byte byPPPMode;            //PPP模式, 0－主动，1－被动	
            public byte byRedial;            //是否回拨 ：0-否,1-是
            public byte byRedialMode;        //回拨模式,0-由拨入者指定,1-预置回拨号码
            public byte byDataEncrypt;	     //数据加密,0-否,1-是
            public uint dwMTU;              //MTU
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PHONENUMBER_LEN)]
            public string sTelephoneNumber;   //电话号码
        }

        //RS232串口参数配置(子结构)(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_RS232
        {
            public uint dwBaudRate;//波特率(bps)，0－50，1－75，2－110，3－150，4－300，5－600，6－1200，7－2400，8－4800，9－9600，10－19200， 11－38400，12－57600，13－76800，14－115.2k;
            public byte byDataBit;// 数据有几位 0－5位，1－6位，2－7位，3－8位;
            public byte byStopBit;// 停止位 0－1位，1－2位;
            public byte byParity;// 校验 0－无校验，1－奇校验，2－偶校验;
            public byte byFlowcontrol;// 0－无，1－软流控,2-硬流控
            public uint dwWorkMode;// 工作模式，0－232串口用于PPP拨号，1－232串口用于参数控制，2－透明通道
        }

        //RS232串口参数配置(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RS232CFG_V30
        {
            public uint dwSize;
            public NET_DVR_SINGLE_RS232 struRs232;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 84)]
            public byte[] byRes;
            public NET_DVR_PPPCFG_V30 struPPPConfig;
        }

        //RS232串口参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RS232CFG
        {
            public uint dwSize;
            public uint dwBaudRate;//波特率(bps)，0－50，1－75，2－110，3－150，4－300，5－600，6－1200，7－2400，8－4800，9－9600，10－19200， 11－38400，12－57600，13－76800，14－115.2k;
            public byte byDataBit;// 数据有几位 0－5位，1－6位，2－7位，3－8位;
            public byte byStopBit;// 停止位 0－1位，1－2位;
            public byte byParity;// 校验 0－无校验，1－奇校验，2－偶校验;
            public byte byFlowcontrol;// 0－无，1－软流控,2-硬流控
            public uint dwWorkMode;// 工作模式，0－窄带传输(232串口用于PPP拨号)，1－控制台(232串口用于参数控制)，2－透明通道
            public NET_DVR_PPPCFG struPPPConfig;
        }

        #endregion

        #region 报警输入输出相关结构

        //报警输入参数配置(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINCFG_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sAlarmInName;	// 名称 
            public byte byAlarmType;	//报警器类型,0：常开,1：常闭
            public byte byAlarmInHandle;	// 是否处理 0-不处理 1-处理
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes1;
            public NET_DVR_HANDLEEXCEPTION_V30 struAlarmHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byRelRecordChan; //报警触发的录象通道,为1表示触发该通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byEnablePreset;		// 是否调用预置点 0-否,1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byPresetNo;// 调用的云台预置点序号,一个报警输入可以调用多个通道的云台预置点, 0xff表示不调用预置点。
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 192)]
            public byte[] byRes2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byEnableCruise;		// 是否调用巡航 0-否,1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byCruiseNo;			// 巡航 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byEnablePtzTrack;		// 是否调用轨迹 0-否,1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byPTZTrack;			// 调用的云台的轨迹序号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes3;
        }

        //报警输入参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sAlarmInName;	// 名称 
            public byte byAlarmType;	//报警器类型,0：常开,1：常闭
            public byte byAlarmInHandle;	// 是否处理 
            public NET_DVR_HANDLEEXCEPTION struAlarmHandleType;	// 处理方式 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//布防时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byRelRecordChan; //报警触发的录象通道,为1表示触发该通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byEnablePreset;		// 是否调用预置点 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byPresetNo;// 调用的云台预置点序号,一个报警输入可以调用多个通道的云台预置点, 0xff表示不调用预置点。
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byEnableCruise;		// 是否调用巡航 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byCruiseNo;			// 巡航 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byEnablePtzTrack;		// 是否调用轨迹 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public byte[] byPTZTrack;			// 调用的云台的轨迹序号 
        }

        //上传报警信息(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V30
        {
            public uint dwAlarmType;//0-信号量报警,1-硬盘满,2-信号丢失,3－移动侦测,4－硬盘未格式化,5-读写硬盘出错,6-遮挡报警,7-制式不匹配, 8-非法访问, 0xa-GPS定位信息(车载定制)
            public uint dwAlarmInputNumber;//报警输入端口
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30)]
            public uint[] dwAlarmOutputNumber;//触发的输出端口，为1表示对应输出
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public uint[] dwAlarmRelateChannel;//触发的录像通道，为1表示对应录像, dwAlarmRelateChannel[0]对应第1个通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public uint[] dwChannel;//dwAlarmType为2或3,6时，表示哪个通道，dwChannel[0]对应第1个通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30)]
            public uint[] dwDiskNumber;//dwAlarmType为1,4,5时,表示哪个硬盘, dwDiskNumber[0]对应第1个硬盘
        }

        //上传报警信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO
        {
            public uint dwAlarmType;//0-信号量报警,1-硬盘满,2-信号丢失,3－移动侦测,4－硬盘未格式化,5-读写硬盘出错,6-遮挡报警,7-制式不匹配, 8-非法访问, 9-串口状态, 0xa-GPS定位信息(车载定制)
            public uint dwAlarmInputNumber;//报警输入端口, 当报警类型为9时该变量表示串口状态0表示正常， -1表示错误
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT)]
            public uint[] dwAlarmOutputNumber;//触发的输出端口，哪一位为1表示对应哪一个输出
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public uint[] dwAlarmRelateChannel;//触发的录像通道，哪一位为1表示对应哪一路录像, dwAlarmRelateChannel[0]对应第1个通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public uint[] dwChannel;//dwAlarmType为2或3,6时，表示哪个通道，dwChannel[0]位对应第1个通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM)]
            public uint[] dwDiskNumber;//dwAlarmType为1,4,5时,表示哪个硬盘, dwDiskNumber[0]位对应第1个硬盘
        }

        #endregion

        #region IPC接入参数配置
        // IP设备结构 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPDEVINFO
        {
            public uint dwEnable;				    // 该IP设备是否启用 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;	    // 密码  
            public NET_DVR_IPADDR struIP;			// IP地址 
            public ushort wDVRPort;			 	    // 端口号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 34)]
            public byte[] byRes;				// 保留 
        }


        // IP通道匹配参数 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPCHANINFO
        {
            public byte byEnable;					// 0表示9000设备的数字通道连接对应的IPC或DVS失败，该通道不在线；1表示连接成功，该通道在线；
            public byte byIPID;					// IP设备ID 取值1- MAX_IP_DEVICE 
            public byte byChannel;					// 通道号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
            public byte[] byRes;					// 保留 
        }

        // IP接入配置结构 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPPARACFG
        {
            public uint dwSize;			                            // 结构大小 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE)]
            public NET_DVR_IPDEVINFO[] struIPDevInfo;    // IP设备 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM)]
            public byte[] byAnalogChanEnable;        // 模拟通道是否启用，从低到高表示1-32通道，0表示无效 1有效 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;	// IP通道     
        }

        // IP报警输出参数 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTINFO
        {
            public byte byIPID;					// IP设备ID取值1- MAX_IP_DEVICE 
            public byte byAlarmOut;				// 报警输出号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] byRes;					// 保留 
        }

        // IP报警输出配置结构 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTCFG
        {
            public uint dwSize;			                        // 结构大小    
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT)]
            public NET_DVR_IPALARMOUTINFO[] struIPAlarmOutInfo;// IP报警输出 
        }

        // IP报警输入参数 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMININFO
        {
            public byte byIPID;					// IP设备ID取值1- MAX_IP_DEVICE 
            public byte byAlarmIn;					// 报警输入号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] byRes;					// 保留 
        }

        // IP报警输入配置结构 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMINCFG
        {
            public uint dwSize;			                        // 结构大小     
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN)]
            public NET_DVR_IPALARMININFO[] struIPAlarmInInfo;// IP报警输入 
        }

        //ipc alarm info
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMINFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE)]
            public NET_DVR_IPDEVINFO[] struIPDevInfo;            // IP设备 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM)]
            public byte[] byAnalogChanEnable;                // 模拟通道是否启用，0-未启用 1-启用 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;	        // IP通道 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN)]
            public NET_DVR_IPALARMININFO[] struIPAlarmInInfo;    // IP报警输入 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT)]
            public NET_DVR_IPALARMOUTINFO[] struIPAlarmOutInfo; // IP报警输出 
        }

        #endregion

        #region 本地硬盘相关结构

        //本地硬盘信息配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_HD
        {
            public uint dwHDNo;         //硬盘号, 取值0~MAX_DISKNUM_V30-1
            public uint dwCapacity;     //硬盘容量(不可设置)
            public uint dwFreeSpace;    //硬盘剩余空间(不可设置)
            public uint dwHdStatus;     //硬盘状态(不可设置) 0-正常, 1-未格式化, 2-错误, 3-SMART状态, 4-不匹配, 5-休眠
            public byte byHDAttr;       //0-默认, 1-冗余; 2-只读
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            public uint dwHdGroup;      //属于哪个盘组 1-MAX_HD_GROUP
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HDCFG
        {
            public uint dwSize;
            public uint dwHDCount;          //硬盘数(不可设置)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30)]
            public NET_DVR_SINGLE_HD[] struHDInfo;//硬盘相关操作都需要重启才能生效；
        }

        //本地硬盘组信息配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_HDGROUP
        {
            public uint dwHDGroupNo;       //盘组号(不可设置) 1-MAX_HD_GROUP  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byHDGroupChans; //盘组对应的录像通道, 0-表示该通道不录象到该盘组，1-表示录象到该盘组
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HDGROUP_CFG
        {
            public uint dwSize;
            public uint dwHDGroupCount;        //盘组总数(不可设置)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_HD_GROUP)]
            public NET_DVR_SINGLE_HDGROUP[] struHDGroupAttr;//硬盘相关操作都需要重启才能生效；
        }

        #endregion

        //配置缩放参数的结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SCALECFG
        {
            public uint dwSize;
            public uint dwMajorScale;    // 主显示 0-不缩放，1-缩放
            public uint dwMinorScale;    // 辅显示 0-不缩放，1-缩放
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwRes;
        }

        //DVR报警输出(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTCFG_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sAlarmOutName;	// 名称 
            public uint dwAlarmOutDelay;	// 输出保持时间(-1为无限，手动关闭)
            //0-5秒,1-10秒,2-30秒,3-1分钟,4-2分钟,5-5分钟,6-10分钟,7-手动
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30)]
            public NET_DVR_SCHEDTIME[] struAlarmOutTime;// 报警输出激活时间段 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes;
        }

        //DVR报警输出
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sAlarmOutName;	// 名称 
            public uint dwAlarmOutDelay;	// 输出保持时间(-1为无限，手动关闭)
            //0-5秒,1-10秒,2-30秒,3-1分钟,4-2分钟,5-5分钟,6-10分钟,7-手动
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT)]
            public NET_DVR_SCHEDTIME[] struAlarmOutTime;// 报警输出激活时间段 
        }

        //DVR本地预览参数(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWCFG_V30
        {
            public uint dwSize;
            public byte byPreviewNumber;//预览数目,0-1画面,1-4画面,2-9画面,3-16画面,0xff:最大画面
            public byte byEnableAudio;//是否声音预览,0-不预览,1-预览
            public ushort wSwitchTime;//切换时间,0-不切换,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PREVIEW_MODE * MAX_WINDOW_V30)]
            public byte[] bySwitchSeq;//切换顺序,如果lSwitchSeq[i]为 0xff表示不用
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] byRes;
        }

        //DVR本地预览参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWCFG
        {
            public uint dwSize;
            public byte byPreviewNumber;//预览数目,0-1画面,1-4画面,2-9画面,3-16画面,0xff:最大画面
            public byte byEnableAudio;//是否声音预览,0-不预览,1-预览
            public ushort wSwitchTime;//切换时间,0-不切换,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_WINDOW)]
            public byte[] bySwitchSeq;//切换顺序,如果lSwitchSeq[i]为 0xff表示不用
        }

        //DVR视频输出
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VGAPARA
        {
            public ushort wResolution;							// 分辨率 
            public ushort wFreq;									// 刷新频率 
            public uint dwBrightness;							// 亮度 
        }

        #region MATRIX输出参数结构

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIXPARA_V30
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM)]
            public ushort[] wOrder;		// 预览顺序, 0xff表示相应的窗口不预览 
            public ushort wSwitchTime;				// 预览切换时间 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] res;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIXPARA
        {
            public ushort wDisplayLogo;						// 显示视频通道号 
            public ushort wDisplayOsd;						// 显示时间 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VOOUT
        {
            public byte byVideoFormat;						// 输出制式,0-PAL,1-NTSC 
            public byte byMenuAlphaValue;					// 菜单与背景图象对比度 
            public ushort wScreenSaveTime;					// 屏幕保护时间 0-从不,1-1分钟,2-2分钟,3-5分钟,4-10分钟,5-20分钟,6-30分钟 
            public ushort wVOffset;							// 视频输出偏移 
            public ushort wBrightness;						// 视频输出亮度 
            public byte byStartMode;						// 启动后视频输出模式(0:菜单,1:预览)
            public byte byEnableScaler;                    // 是否启动缩放 (0-不启动, 1-启动)
        }

        //DVR视频输出(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOOUT_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_VIDEOOUT_V30)]
            public NET_DVR_VOOUT[] struVOOut;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_VGA_V30)]
            public NET_DVR_VGAPARA[] struVGAPara;	// VGA参数 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_MATRIXOUT)]
            public NET_DVR_MATRIXPARA_V30[] struMatrixPara;		// MATRIX参数 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes;
        }

        //DVR视频输出
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOOUT
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_VIDEOOUT)]
            public NET_DVR_VOOUT[] struVOOut;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_VGA)]
            public NET_DVR_VGAPARA[] struVGAPara;	// VGA参数 
            public NET_DVR_MATRIXPARA struMatrixPara;		// MATRIX参数 
        }

        #endregion

        #region DVR用户参数结构

        //单用户参数(9000扩展)(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_INFO_V30
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwLocalRight;	// 本地权限 
            //数组0: 本地控制云台
            //数组1: 本地手动录象
            //数组2: 本地回放
            //数组3: 本地设置参数
            //数组4: 本地查看状态、日志
            //数组5: 本地高级操作(升级，格式化，重启，关机)
            //数组6: 本地查看参数 
            //数组7: 本地管理模拟和IP camera 
            //数组8: 本地备份 
            //数组9: 本地关机/重启 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwRemoteRight;	// 远程权限 
            //数组0: 远程控制云台
            //数组1: 远程手动录象
            //数组2: 远程回放 
            //数组3: 远程设置参数
            //数组4: 远程查看状态、日志
            //数组5: 远程高级操作(升级，格式化，重启，关机)
            //数组6: 远程发起语音对讲
            //数组7: 远程预览
            //数组8: 远程请求报警上传、报警输出
            //数组9: 远程控制，本地输出
            //数组10: 远程控制串口
            //数组11: 远程查看参数 
            //数组12: 远程管理模拟和IP camera 
            //数组13: 远程关机/重启 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byNetPreviewRight;		// 远程可以预览的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byLocalPlaybackRight;	// 本地可以回放的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byNetPlaybackRight;	// 远程可以回放的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byLocalRecordRight;		// 本地可以录像的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byNetRecordRight;		// 远程可以录像的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byLocalPTZRight;		// 本地可以PTZ的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byNetPTZRight;			// 远程可以PTZ的通道 0-有权限，1-无权限
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byLocalBackupRight;		// 本地备份权限通道 0-有权限，1-无权限
            NET_DVR_IPADDR struUserIP;		// 用户IP地址(为0时表示允许任何地址) 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMACAddr;	// 物理地址 
            public byte byPriority;				// 优先级，0xff-无，0--低，1--中，2--高 
            // 无……表示不支持优先级的设置
            // 低……默认权限:包括本地和远程回放,本地和远程查看日志和状态,本地和远程关机/重启
            // 中……包括本地和远程控制云台,本地和远程手动录像,本地和远程回放,语音对讲和远程预览本地备份,本地/远程关机/重启
            // 高……管理员
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public byte[] byRes;
        }

        //单用户参数(SDK_V15扩展)(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_INFO_EX
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwLocalRight;	// 权限 
            //数组0: 本地控制云台
            //数组1: 本地手动录象
            //数组2: 本地回放
            //数组3: 本地设置参数
            //数组4: 本地查看状态、日志
            //数组5: 本地高级操作(升级，格式化，重启，关机)
            public uint dwLocalPlaybackRight;		// 本地可以回放的通道 bit0 -- channel 1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwRemoteRight;	// 权限 
            //数组0: 远程控制云台
            //数组1: 远程手动录象
            //数组2: 远程回放 
            //数组3: 远程设置参数
            //数组4: 远程查看状态、日志
            //数组5: 远程高级操作(升级，格式化，重启，关机)
            //数组6: 远程发起语音对讲
            //数组7: 远程预览
            //数组8: 远程请求报警上传、报警输出
            //数组9: 远程控制，本地输出
            //数组10: 远程控制串口
            public uint dwNetPreviewRight;		// 远程可以预览的通道 bit0 -- channel 1
            public uint dwNetPlaybackRight;		// 远程可以回放的通道 bit0 -- channel 1
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sUserIP;				// 用户IP地址(为0时表示允许任何地址) 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMACAddr;	// 物理地址 
        }

        //单用户参数(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwLocalRight;	// 权限 
            //数组0: 本地控制云台
            //数组1: 本地手动录象
            //数组2: 本地回放
            //数组3: 本地设置参数
            //数组4: 本地查看状态、日志
            //数组5: 本地高级操作(升级，格式化，重启，关机)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT)]
            public uint[] dwRemoteRight;	// 权限 
            //数组0: 远程控制云台
            //数组1: 远程手动录象
            //数组2: 远程回放 
            //数组3: 远程设置参数
            //数组4: 远程查看状态、日志
            //数组5: 远程高级操作(升级，格式化，重启，关机)
            //数组6: 远程发起语音对讲
            //数组7: 远程预览
            //数组8: 远程请求报警上传、报警输出
            //数组9: 远程控制，本地输出
            //数组10: 远程控制串口
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sUserIP;				// 用户IP地址(为0时表示允许任何地址) 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMACAddr;	// 物理地址 
        }

        //DVR用户参数(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM_V30)]
            public NET_DVR_USER_INFO_V30[] struUser;
        }

        //DVR用户参数(SDK_V15扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_EX
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM)]
            public NET_DVR_USER_INFO_EX[] struUser;
        }

        //DVR用户参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM)]
            public NET_DVR_USER_INFO[] struUser;
        }

        #endregion

        //DVR异常参数(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_EXCEPTION_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_EXCEPTIONNUM_V30)]
            public NET_DVR_HANDLEEXCEPTION_V30[] struExceptionHandleType;
            //数组0-盘满,1- 硬盘出错,2-网线断,3-局域网内IP 地址冲突, 4-非法访问, 9-输入/输出视频制式不匹配, 10-视频信号异常
        }

        //DVR异常参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_EXCEPTION
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_EXCEPTIONNUM)]
            public NET_DVR_HANDLEEXCEPTION[] struExceptionHandleType;
            //数组0-盘满,1- 硬盘出错,2-网线断,3-局域网内IP 地址冲突,4-非法访问, 5-输入/输出视频制式不匹配, 6-输入/输出视频制式不匹配
        }

        #region 服务器状态

        //通道状态(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CHANNELSTATE_V30
        {
            public byte byRecordStatic; //通道是否在录像,0-不录像,1-录像
            public byte bySignalStatic; //连接的信号状态,0-正常,1-信号丢失
            public byte byHardwareStatic;//通道硬件状态,0-正常,1-异常,例如DSP死掉
            public byte byRes1;//保留
            public uint dwBitRate;//实际码率
            public uint dwLinkNum;//客户端连接的个数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_LINK)]
            public NET_DVR_IPADDR[] struClientIP;//客户端的IP地址
            public uint dwIPLinkNum;//如果该通道为IP接入，那么表示IP接入当前的连接数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] byRes;
        }

        //通道状态
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CHANNELSTATE
        {
            public byte byRecordStatic; //通道是否在录像,0-不录像,1-录像
            public byte bySignalStatic; //连接的信号状态,0-正常,1-信号丢失
            public byte byHardwareStatic;//通道硬件状态,0-正常,1-异常,例如DSP死掉
            public byte reservedData;
            public uint dwBitRate;//实际码率
            public uint dwLinkNum;//客户端连接的个数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_LINK)]
            public uint[] dwClientIP;//客户端的IP地址
        }

        //硬盘状态
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DISKSTATE
        {
            public uint dwVolume;//硬盘的容量
            public uint dwFreeSpace;//硬盘的剩余空间
            public uint dwHardDiskStatic; //硬盘的状态,休眠,活动,不正常等
        }

        //DVR工作状态(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_WORKSTATE_V30
        {

            public uint dwDeviceStatic; 	//设备的状态,0-正常,1-CPU占用率太高,超过85%,2-硬件错误,例如串口死掉
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30)]
            public NET_DVR_DISKSTATE[] struHardDiskStatic;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public NET_DVR_CHANNELSTATE_V30[] struChanStatic;//通道的状态
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN_V30)]
            public byte[] byAlarmInStatic; //报警端口的状态,0-没有报警,1-有报警
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30)]
            public byte[] byAlarmOutStatic; //报警输出端口的状态,0-没有输出,1-有报警输出
            public uint dwLocalDisplay;//本地显示状态,0-正常,1-不正常
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V30)]
            public byte[] byAudioChanStatus;//表示语音通道的状态 0-未使用，1-使用中, 0xff无效
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] byRes;
        }

        //DVR工作状态
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_WORKSTATE
        {
            public uint dwDeviceStatic; 	//设备的状态,0-正常,1-CPU占用率太高,超过85%,2-硬件错误,例如串口死掉
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM)]
            public NET_DVR_DISKSTATE[] struHardDiskStatic;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM)]
            public NET_DVR_CHANNELSTATE[] struChanStatic;//通道的状态
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN)]
            public byte[] byAlarmInStatic; //报警端口的状态,0-没有报警,1-有报警
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT)]
            public byte[] byAlarmOutStatic; //报警输出端口的状态,0-没有输出,1-有报警输出
            public uint dwLocalDisplay;//本地显示状态,0-正常,1-不正常
        }

        #endregion

        #region DVR日志

        #region 常量

        #region 报警
        //主类型
        public const int MAJOR_ALARM = 0x1;
        //次类型
        public const int MINOR_ALARM_IN = 0x1;// 报警输入 
        public const int MINOR_ALARM_OUT = 0x2;// 报警输出 
        public const int MINOR_MOTDET_START = 0x3;// 移动侦测报警开始 
        public const int MINOR_MOTDET_STOP = 0x4;// 移动侦测报警结束 
        public const int MINOR_HIDE_ALARM_START = 0x5;// 遮挡报警开始 
        public const int MINOR_HIDE_ALARM_STOP = 0x6;// 遮挡报警结束 
        #endregion

        #region 异常
        //主类型
        public const int MAJOR_EXCEPTION = 0x2;
        //次类型                                    
        public const int MINOR_VI_LOST = 0x21;// 视频信号丢失 
        public const int MINOR_ILLEGAL_ACCESS = 0x22;// 非法访问 
        public const int MINOR_HD_FULL = 0x23;// 硬盘满 
        public const int MINOR_HD_ERROR = 0x24;// 硬盘错误 
        public const int MINOR_DCD_LOST = 0x25;// MODEM 掉线(保留不使用) 
        public const int MINOR_IP_CONFLICT = 0x26;// IP地址冲突 
        public const int MINOR_NET_BROKEN = 0x27;// 网络断开
        public const int MINOR_REC_ERROR = 0x28;// 录像出错 
        public const int MINOR_IPC_NO_LINK = 0x29;// IPC连接异常 
        public const int MINOR_VI_EXCEPTION = 0x2a;// 视频输入异常(只针对模拟通道) 
        #endregion

        #region 操作

        //主类型
        public const int MAJOR_OPERATION = 0x3;
        //次类型                                    =
        public const int MINOR_START_DVR = 0x41;// 开机 
        public const int MINOR_STOP_DVR = 0x42;// 关机 
        public const int MINOR_STOP_ABNORMAL = 0x43;// 异常关机 
        public const int MINOR_REBOOT_DVR = 0x44;//本地重启设备

        public const int MINOR_LOCAL_LOGIN = 0x50;// 本地登陆 
        public const int MINOR_LOCAL_LOGOUT = 0x51;// 本地注销登陆 
        public const int MINOR_LOCAL_CFG_PARM = 0x52;// 本地配置参数 
        public const int MINOR_LOCAL_PLAYBYFILE = 0x53;// 本地按文件回放或下载 
        public const int MINOR_LOCAL_PLAYBYTIME = 0x54;// 本地按时间回放或下载
        public const int MINOR_LOCAL_START_REC = 0x55;// 本地开始录像 
        public const int MINOR_LOCAL_STOP_REC = 0x56;// 本地停止录像 
        public const int MINOR_LOCAL_PTZCTRL = 0x57;// 本地云台控制 
        public const int MINOR_LOCAL_PREVIEW = 0x58;// 本地预览 (保留不使用)
        public const int MINOR_LOCAL_MODIFY_TIME = 0x59;// 本地修改时间(保留不使用) 
        public const int MINOR_LOCAL_UPGRADE = 0x5a;// 本地升级 
        public const int MINOR_LOCAL_RECFILE_OUTPUT = 0x5b;// 本地备份录象文件 
        public const int MINOR_LOCAL_FORMAT_HDD = 0x5c;// 本地初始化硬盘 
        public const int MINOR_LOCAL_CFGFILE_OUTPUT = 0x5d;// 导出本地配置文件 
        public const int MINOR_LOCAL_CFGFILE_INPUT = 0x5e;// 导入本地配置文件 
        public const int MINOR_LOCAL_COPYFILE = 0x5f;// 本地备份文件 
        public const int MINOR_LOCAL_LOCKFILE = 0x60;// 本地锁定录像文件 
        public const int MINOR_LOCAL_UNLOCKFILE = 0x61;// 本地解锁录像文件 
        public const int MINOR_LOCAL_DVR_ALARM = 0x62;// 本地手动清除和触发报警
        public const int MINOR_IPC_ADD = 0x63;// 本地添加IPC 
        public const int MINOR_IPC_DEL = 0x64;// 本地删除IPC 
        public const int MINOR_IPC_SET = 0x65;// 本地设置IPC 
        public const int MINOR_LOCAL_START_BACKUP = 0x66;// 本地开始备份 
        public const int MINOR_LOCAL_STOP_BACKUP = 0x67;// 本地停止备份
        public const int MINOR_LOCAL_COPYFILE_START_TIME = 0x68;// 本地备份开始时间
        public const int MINOR_LOCAL_COPYFILE_END_TIME = 0x69;// 本地备份结束时间

        public const int MINOR_REMOTE_LOGIN = 0x70;// 远程登录 
        public const int MINOR_REMOTE_LOGOUT = 0x71;// 远程注销登陆 
        public const int MINOR_REMOTE_START_REC = 0x72;// 远程开始录像 
        public const int MINOR_REMOTE_STOP_REC = 0x73;// 远程停止录像 
        public const int MINOR_START_TRANS_CHAN = 0x74;// 开始透明传输 
        public const int MINOR_STOP_TRANS_CHAN = 0x75;// 停止透明传输 
        public const int MINOR_REMOTE_GET_PARM = 0x76;// 远程获取参数 
        public const int MINOR_REMOTE_CFG_PARM = 0x77;// 远程配置参数 
        public const int MINOR_REMOTE_GET_STATUS = 0x78;// 远程获取状态 
        public const int MINOR_REMOTE_ARM = 0x79;// 远程布防 
        public const int MINOR_REMOTE_DISARM = 0x7a;// 远程撤防 
        public const int MINOR_REMOTE_REBOOT = 0x7b;// 远程重启 
        public const int MINOR_START_VT = 0x7c;// 开始语音对讲 
        public const int MINOR_STOP_VT = 0x7d;// 停止语音对讲 
        public const int MINOR_REMOTE_UPGRADE = 0x7e;// 远程升级 
        public const int MINOR_REMOTE_PLAYBYFILE = 0x7f;// 远程按文件回放 
        public const int MINOR_REMOTE_PLAYBYTIME = 0x80;// 远程按时间回放 
        public const int MINOR_REMOTE_PTZCTRL = 0x81;// 远程云台控制 
        public const int MINOR_REMOTE_FORMAT_HDD = 0x82;// 远程格式化硬盘 
        public const int MINOR_REMOTE_STOP = 0x83;// 远程关机 
        public const int MINOR_REMOTE_LOCKFILE = 0x84;// 远程锁定文件 
        public const int MINOR_REMOTE_UNLOCKFILE = 0x85;// 远程解锁文件 
        public const int MINOR_REMOTE_CFGFILE_OUTPUT = 0x86;// 远程导出配置文件 
        public const int MINOR_REMOTE_CFGFILE_INTPUT = 0x87;// 远程导入配置文件 
        public const int MINOR_REMOTE_RECFILE_OUTPUT = 0x88;// 远程导出录象文件 
        public const int MINOR_REMOTE_DVR_ALARM = 0x89;// 远程手动清除和触发报警
        public const int MINOR_REMOTE_IPC_ADD = 0x8a;// 远程添加IPC 
        public const int MINOR_REMOTE_IPC_DEL = 0x8b;// 远程删除IPC 
        public const int MINOR_REMOTE_IPC_SET = 0x8c;// 远程设置IPC 

        #endregion

        #region 日志附加信息
        //主类型
        public const int MAJOR_INFORMATION = 0x4;//附加信息
        //次类型
        public const int MINOR_HDD_INFO = 0xa1;//硬盘信息
        public const int MINOR_SMART_INFO = 0xa2;//SMART信息
        public const int MINOR_REC_START = 0xa3;//开始录像
        public const int MINOR_REC_STOP = 0xa4;//停止录像
        public const int MINOR_REC_OVERDUE = 0xa5;//过期录像删除

        //当日志的主类型为MAJOR_OPERATION=03，次类型为MINOR_LOCAL_CFG_PARM=0x52或者MINOR_REMOTE_GET_PARM=0x76或者MINOR_REMOTE_CFG_PARM=0x77时，dwParaType:参数类型有效，其含义如下：
        public const int PARA_VIDEOOUT = 0x1;
        public const int PARA_IMAGE = 0x2;
        public const int PARA_ENCODE = 0x4;
        public const int PARA_NETWORK = 0x8;
        public const int PARA_ALARM = 0x10;
        public const int PARA_EXCEPTION = 0x20;
        public const int PARA_DECODER = 0x40;//解码器
        public const int PARA_RS232 = 0x80;
        public const int PARA_PREVIEW = 0x100;
        public const int PARA_SECURITY = 0x200;
        public const int PARA_DATETIME = 0x400;
        public const int PARA_FRAMETYPE = 0x800;//帧格式
        #endregion

        #endregion

        //日志信息(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_LOG_V30
        {
            public NET_DVR_TIME strLogTime;
            public uint dwMajorType;	//主类型 1-报警; 2-异常; 3-操作; 0xff-全部
            public uint dwMinorType;//次类型 0-全部; 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_NAMELEN)]
            public string sPanelUser; //操作面板的用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_NAMELEN)]
            public string sNetUser;//网络操作的用户名
            public NET_DVR_IPADDR struRemoteHostAddr;//远程主机地址
            public uint dwParaType;//参数类型
            public uint dwChannel;//通道号
            public uint dwDiskNumber;//硬盘号
            public uint dwAlarmInPort;//报警输入端口
            public uint dwAlarmOutPort;//报警输出端口
            public uint dwInfoLen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LOG_INFO_LEN)]
            public string sInfo;
        }

        //日志信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_LOG
        {
            public NET_DVR_TIME strLogTime;
            public uint dwMajorType;	//主类型 1-报警; 2-异常; 3-操作; 0xff-全部
            public uint dwMinorType;//次类型 0-全部; 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_NAMELEN)]
            public string sPanelUser; //操作面板的用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_NAMELEN)]
            public string sNetUser;//网络操作的用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteHostAddr;//远程主机地址
            public uint dwParaType;//参数类型
            public uint dwChannel;//通道号
            public uint dwDiskNumber;//硬盘号
            public uint dwAlarmInPort;//报警输入端口
            public uint dwAlarmOutPort;//报警输出端口
        }

        #endregion

        //报警输出状态(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTSTATUS_V30
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30)]
            public byte[] Output;
        }

        //报警输出状态
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTSTATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT)]
            public byte[] Output;
        }

        //交易信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_TRADEINFO
        {
            public ushort m_Year;
            public ushort m_Month;
            public ushort m_Day;
            public ushort m_Hour;
            public ushort m_Minute;
            public ushort m_Second;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string DeviceName;	//设备名称
            public uint dwChannelNumer;	//通道号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string CardNumber;	//卡号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string cTradeType;	//交易类型
            public uint dwCash;			//交易金额
        }

        #region ATM专用

        #region 常量
        public const int NCR = 0;
        public const int DIEBOLD = 1;
        public const int WINCOR_NIXDORF = 2;
        public const int SIEMENS = 3;
        public const int OLIVETTI = 4;
        public const int FUJITSU = 5;
        public const int HITACHI = 6;
        public const int SMI = 7;
        public const int IBM = 8;
        public const int BULL = 9;
        public const int YiHua = 10;
        public const int LiDe = 11;
        public const int GDYT = 12;
        public const int Mini_Banl = 13;
        public const int GuangLi = 14;
        public const int DongXin = 15;
        public const int ChenTong = 16;
        public const int NanTian = 17;
        public const int XiaoXing = 18;
        public const int GZYY = 19;
        public const int QHTLT = 20;
        public const int DRS918 = 21;
        public const int KALATEL = 22;
        public const int NCR_2 = 23;
        public const int NXS = 24;
        #endregion

        //帧格式
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FRAMETYPECODE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] code;		// 代码 
        }

        //ATM参数(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FRAMEFORMAT_V30
        {

            public uint dwSize;
            public NET_DVR_IPADDR struATMIP;            // ATM IP地址 
            public uint dwATMType;						// ATM类型 
            public uint dwInputMode;					// 输入方式	
            public uint dwFrameSignBeginPos;            // 报文标志位的起始位置
            public uint dwFrameSignLength;              // 报文标志位的长度
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] byFrameSignContent;			// 报文标志位的内容 
            public uint dwCardLengthInfoBeginPos;		// 卡号长度信息的起始位置 
            public uint dwCardLengthInfoLength;			// 卡号长度信息的长度 
            public uint dwCardNumberInfoBeginPos;		// 卡号信息的起始位置 
            public uint dwCardNumberInfoLength;			// 卡号信息的长度 
            public uint dwBusinessTypeBeginPos;         // 交易类型的起始位置 
            public uint dwBusinessTypeLength;           // 交易类型的长度 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public NET_DVR_FRAMETYPECODE[] frameTypeCode;// 交易类型
            public ushort wATMPort;						 // 卡号捕捉端口号(网络协议方式) 
            public ushort wProtocolType;				 // 网络协议类型 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] byRes;
        }

        //ATM参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FRAMEFORMAT
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] sATMIP;						// ATM IP地址 
            public uint dwATMType;						// ATM类型 
            public uint dwInputMode;						// 输入方式	
            public uint dwFrameSignBeginPos;              // 报文标志位的起始位置
            public uint dwFrameSignLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]// 报文标志位的长度 
            public byte[] byFrameSignContent;			// 报文标志位的内容 
            public uint dwCardLengthInfoBeginPos;			// 卡号长度信息的起始位置 
            public uint dwCardLengthInfoLength;			// 卡号长度信息的长度 
            public uint dwCardNumberInfoBeginPos;			// 卡号信息的起始位置 
            public uint dwCardNumberInfoLength;			// 卡号信息的长度 
            public uint dwBusinessTypeBeginPos;           // 交易类型的起始位置 
            public uint dwBusinessTypeLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]// 交易类型的长度 
            public NET_DVR_FRAMETYPECODE[] frameTypeCode;// 类型 
        }

        #endregion

        #region DS-6001D/F
        //DS-6001D Decoder

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderIP;		//解码设备连接的服务器IP
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderUser;		//解码设备连接的服务器的用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderPasswd;	//解码设备连接的服务器的密码
            public byte bySendMode;			//解码设备连接服务器的连接模式
            public byte byEncoderChannel;		//解码设备连接的服务器的通道号
            public ushort wEncoderPort;			//解码设备连接的服务器的端口号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] reservedData;		//保留
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERSTATE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderIP;		//解码设备连接的服务器IP
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderUser;		//解码设备连接的服务器的用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string byEncoderPasswd;	//解码设备连接的服务器的密码
            public byte byEncoderChannel;		//解码设备连接的服务器的通道号
            public byte bySendMode;			//解码设备连接的服务器的连接模式
            public short wEncoderPort;			//解码设备连接的服务器的端口号
            public uint dwConnectState;		//解码设备连接服务器的状态
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] reservedData;		//保留
        }

        #region 解码设备控制码定义
        public const int NET_DEC_STARTDEC = 1;
        public const int NET_DEC_STOPDEC = 2;
        public const int NET_DEC_STOPCYCLE = 3;
        public const int NET_DEC_CONTINUECYCLE = 4;
        #endregion

        //连接的通道配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECCHANINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;				// DVR IP地址 
            public ushort wDVRPort;			 		// 端口号 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            public byte byChannel;					// 通道号 
            public byte byLinkMode;				// 连接模式 
            public byte byLinkType;				// 连接类型 0－主码流 1－子码流 
        }

        //每个解码通道的配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECINFO
        {
            public byte byPoolChans;			//每路解码通道上的循环通道数量, 最多4通道 0表示没有解码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DECPOOLNUM)]
            public NET_DVR_DECCHANINFO[] struchanConInfo;
            public byte byEnablePoll;			//是否轮巡 0-否 1-是
            public byte byPoolTime;				//轮巡时间 0-保留 1-10秒 2-15秒 3-20秒 4-30秒 5-45秒 6-1分钟 7-2分钟 8-5分钟 
        }

        //整个设备解码配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECCFG
        {
            public uint dwSize;
            public uint dwDecChanNum; 		//解码通道的数量
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DECNUM)]
            public NET_DVR_DECINFO[] struDecInfo;
        }

        // 2005-08-01 解码设备透明通道设置 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PORTINFO
        {
            public uint dwEnableTransPort;	// 是否启动透明通道 0－不启用 1－启用
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDecoderIP;		// DVR IP地址 
            public short wDecoderPort;			// 端口号 
            public short wDVRTransPort;			// 配置前端DVR是从485/232输出，1表示232串口,2表示485串口 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] cReserve;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PORTCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_TRANSPARENTNUM)]
            public NET_DVR_PORTINFO[] struTransPortInfo; // 数组0表示232 数组1表示485 
        }

        // 控制网络文件回放 
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PLAYREMOTEFILE
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDecoderIP;		// DVR IP地址 
            public short wDecoderPort;			// 端口号 
            public short wLoadMode;				// 回放下载模式 1－按名字 2－按时间 
            [StructLayout(LayoutKind.Explicit)]
            public struct tModeSize
            {
                [StructLayout(LayoutKind.Sequential)]
                public struct tPlaybackName
                {
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
                    public string sName; // 回放的文件名 
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct tPlaybackTime
                {
                    public uint dwChannel;
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                    public string sUserName;	//请求视频用户名
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
                    public string sPassword;	// 密码 
                    public NET_DVR_TIME struStartTime;	// 按时间回放的开始时间 
                    public NET_DVR_TIME struStopTime;	// 按时间回放的结束时间 
                }

                [FieldOffset(0)]
                public tPlaybackName byFile;

                [FieldOffset(0)]
                public tPlaybackTime bytime;
            }
            public tModeSize mode_size;
        }

        //当前设备解码连接状态
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECCHANSTATUS
        {
            public uint dwWorkType;		//工作方式：1：轮巡、2：动态连接解码、3：文件回放下载 4：按时间回放下载
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;		//连接的设备ip
            public short wDVRPort;			//连接端口号
            public byte byChannel;			// 通道号 
            public byte byLinkMode;		// 连接模式 
            public uint dwLinkType;		//连接类型 0－主码流 1－子码流
            [StructLayout(LayoutKind.Explicit)]
            public struct tObjectInfo
            {
                [StructLayout(LayoutKind.Sequential)]
                public struct tUserInfo
                {
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                    public string sUserName;	//请求视频用户名
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
                    public string sPassword;	// 密码 
                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
                    public byte cReserve;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct tFileInfo
                {
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
                    public string fileName;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct tTimeInfo
                {
                    public uint dwChannel;
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                    public string sUserName;	//请求视频用户名
                    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
                    public string sPassword;	// 密码 
                    public NET_DVR_TIME struStartTime;		// 按时间回放的开始时间 
                    public NET_DVR_TIME struStopTime;		// 按时间回放的结束时间 
                }

                [FieldOffset(0)]
                public tUserInfo userInfo;

                [FieldOffset(0)]
                public tFileInfo fileInfo;

                [FieldOffset(0)]
                public tTimeInfo timeInfo;
            }
            public tObjectInfo objectInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DECSTATUS
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DECNUM)]
            public NET_DVR_DECCHANSTATUS[] struDecState;
        }

        #endregion

        #region 叠加字符

        //单字符参数(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRINGINFO
        {
            public ushort wShowString;				// 预览的图象上是否显示字符,0-不显示,1-显示 区域大小704*576,单个字符的大小为32*32
            public ushort wStringSize;				// 该行字符的长度，不能大于44个字符 
            public ushort wShowStringTopLeftX;		// 字符显示位置的x坐标 
            public ushort wShowStringTopLeftY;		// 字符名称显示位置的y坐标 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 44)]
            public string sString;				// 要显示的字符内容 
        }

        //叠加字符扩展(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM_V30)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;				// 要显示的字符内容 
        }

        //叠加字符扩展(8条字符)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING_EX
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM_EX)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;				// 要显示的字符内容 
        }

        //叠加字符
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;				// 要显示的字符内容 
        }

        #endregion

        #region DS9000新增结构

        //EMAIL参数结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_EMAILCFG_V30
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sAccount;				// 账号 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_EMAIL_PWD_LEN)]
            public string sPassword;			//密码 
            [StructLayout(LayoutKind.Sequential)]
            public struct tSender
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                public string sName;				// 发件人姓名 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_EMAIL_ADDR_LEN)]
                public string sAddress;		// 发件人地址 
            }
            public tSender struSender;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_EMAIL_ADDR_LEN)]
            public string sSmtpServer;	// smtp服务器 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_EMAIL_ADDR_LEN)]
            public string sPop3Server;	// pop3服务器 
            [StructLayout(LayoutKind.Sequential)]
            public struct tReceiver
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                public string sName;				// 收件人姓名 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_EMAIL_ADDR_LEN)]
                public string sAddress;		// 收件人地址 
            }
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            tReceiver[] struReceiver;							// 最多可以设置3个收件人 
            public byte byAttachment;					// 是否带附件 
            public byte bySmtpServerVerify;				// 发送服务器要求身份验证 
            public byte byMailInterval;                 // mail interval 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 77)]
            public byte[] res;
        }

        //DVR实现巡航数据结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_PARA
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS)]
            public byte[] byPresetNo;		// 预置点号 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS)]
            public byte[] byCruiseSpeed;	// 巡航速度 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS)]
            public ushort[] wDwellTime;		// 停留时间 
            public byte byEnableThisCruise;						// 是否启用 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] res;
        }

        #endregion

        //时间点
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_TIMEPOINT
        {
            public uint dwMonth;		//月 0-11表示1-12个月
            public uint dwWeekNo;		//第几周 0－第1周 1－第2周 2－第3周 3－第4周 4－最后一周
            public uint dwWeekDate;	//星期几 0－星期日 1－星期一 2－星期二 3－星期三 4－星期四 5－星期五 6－星期六
            public uint dwHour;		//小时	开始时间0－23 结束时间1－23
            public uint dwMin;		//分	0－59
        }

        //夏令时参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ZONEANDDST
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes1;			//保留
            public uint dwEnableDST;		//是否启用夏时制 0－不启用 1－启用
            public byte byDSTBias;	//夏令时偏移值，30min, 60min, 90min, 120min, 以分钟计，传递原始数值
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes2;
            public NET_DVR_TIMEPOINT struBeginPoint;	//夏时制开始时间
            public NET_DVR_TIMEPOINT struEndPoint;	//夏时制停止时间
        }

        //图片质量
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_JPEGPARA
        {
            //注意：当图像压缩分辨率为VGA时，支持0=CIF, 1=QCIF, 2=D1抓图，
            //当分辨率为3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA,7=XVGA, 8=HD900p
            //仅支持当前分辨率的抓图
            public ushort wPicSize;				// 0=CIF, 1=QCIF, 2=D1 3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA
            public ushort wPicQuality;			// 图片质量系数 0-最好 1-较好 2-一般
        }

        //辅助输出参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_AUXOUTCFG
        {
            public uint dwSize;
            public uint dwAlarmOutChan;                       // 选择报警弹出大报警通道切换时间：1画面的输出通道: 0:主输出/1:辅1/2:辅2/3:辅3/4:辅4 
            public uint dwAlarmChanSwitchTime;                // :1秒 - 10:10秒 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUXOUT)]
            public uint[] dwAuxSwitchTime;			// 辅助输出切换时间: 0-不切换,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUXOUT * MAX_WINDOW)]
            public byte[] byAuxOrder;	// 辅助输出预览顺序, 0xff表示相应的窗口不预览 
        }

        #region 网络配置结构

        //ntp
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NTPPARA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sNTPServer;   // Domain Name or IP addr of NTP server 
            public ushort wInterval;		 // adjust time interval(hours) 
            public byte byEnableNTP;    // enable NPT client 0-no，1-yes
            public sbyte cTimeDifferenceH; // 与国际标准时间的 小时偏移-12 ... +13 
            public sbyte cTimeDifferenceM;// 与国际标准时间的 分钟偏移0, 30, 45
            public byte res1;
            public ushort wNtpPort;         // ntp server tcpPort 9000新增 设备默认为123
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] res2;
        }

        //ddns
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUsername;  // DDNS账号用户名/密码 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sDomainName;       // 域名 
            public byte byEnableDDNS;			//是否应用 0-否，1-是
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] res;
        }

        //ddns扩展
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA_EX
        {
            public byte byHostIndex;			// 0-Hikvision DNS 1－Dyndns 2－PeanutHull(花生壳), 3-希网3322
            public byte byEnableDDNS;			//是否应用 0-否，1-是
            public ushort wDDNSPort;						// DDNS端口号 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUsername;  // DDNS账号用户名/密码 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
            public string sDomainName;       // 域名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
            public string sServerName;	// DDNS 对应的服务器地址，可以是IP地址或域名 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes;
        }

        //ddns(9000扩展)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA_V30
        {
            public byte byEnableDDNS;			//是否应用 0-否，1-是
            public byte byHostIndex;			// 0-Hikvision DNS(保留) 1－Dyndns 2－PeanutHull(花生壳) 3-希网3322 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes1;
            [StructLayout(LayoutKind.Sequential)]
            public struct tDDNS
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
                public string sUsername;			// DDNS账号用户名
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
                public string sPassword;			// 密码 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
                public string sDomainName;	// 设备配备的域名地址 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
                public string sServerName;	// DDNS协议对应的服务器地址，可以是IP地址或域名 
                public ushort wDDNSPort;						// 端口号 
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public byte[] byRes;
            }
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DDNS_NUMS)]
            public tDDNS[] struDDNS;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes2;
        }

        //网络参数配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NETAPPCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDNSIp;                // DNS服务器地址 
            public NET_DVR_NTPPARA struNtpClientParam;      // NTP参数 
            public NET_DVR_DDNSPARA struDDNSClientParam;     // DDNS参数 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 464)]
            public byte[] res;			// 保留 
        }

        //nfs结构配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_NFS
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sNfsHostIPAddr;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PATHNAME_LEN)]
            public string sNfsDirectory;        // PATHNAME_LEN = 128
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NFSCFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NFS_DISK)]
            public NET_DVR_SINGLE_NFS[] struNfsDiskParam;
        }

        #endregion

        //巡航点配置(HIK IP快球专用)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_POINT
        {
            public byte PresetNum;	//预置点
            public byte Dwell;		//停留时间
            public byte Speed;		//速度
            public byte Reserve;	//保留
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_RET
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            NET_DVR_CRUISE_POINT[] struCruisePoint;			//最大支持32个巡航点
        }

        #region 多路解码器

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NETCFG_OTHER
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sFirstDNSIP;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sSecondDNSIP;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DECINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;				// DVR IP地址 
            public ushort wDVRPort;			 	// 端口号 
            public byte byChannel;				// 通道号 
            public byte byTransProtocol;			// 传输协议类型 0-TCP, 1-UDP 
            public byte byTransMode;				// 传输码流模式 0－主码流 1－子码流
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;			// 监控主机登陆帐号 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;			// 监控主机密码 
        }

        //启动/停止动态解码
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DYNAMIC_DEC
        {
            public uint dwSize;
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;		// 动态解码通道信息 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_CHAN_STATUS
        {
            public uint dwSize;
            public uint dwIsLinked;         // 解码通道状态 0－休眠 1－正在连接 2－已连接 3-正在解码 
            public uint dwStreamCpRate;     // Stream copy rate, X kbits/second 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public char[] cRes;		// 保留 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_CHAN_INFO
        {
            public uint dwSize;
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;		// 解码通道信息 
            public uint dwDecState;	// 0-动态解码 1－循环解码 2－按时间回放 3－按文件回放 
            public NET_DVR_TIME StartTime;		// 按时间回放开始时间 
            public NET_DVR_TIME StopTime;		// 按时间回放停止时间 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sFileName;		// 按文件回放文件名 
        }

        //连接的通道配置 2007-11-05
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DECCHANINFO
        {
            public uint dwEnable;					// 是否启用 0－否 1－启用
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;		// 轮循解码通道信息 
        }

        //2007-11-05 新增每个解码通道的配置
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_LOOP_DECINFO
        {
            public uint dwSize;
            public uint dwPoolTime;			//轮巡时间 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CYCLE_CHAN)]
            public NET_DVR_MATRIX_DECCHANINFO[] struchanConInfo;
        }

        //2007-12-22
        [StructLayout(LayoutKind.Sequential)]
        public struct TTY_CONFIG
        {
            public byte baudrate; 	// 波特率 
            public byte databits;		// 数据位 
            public byte stopbits;		// 停止位 
            public byte parity;		// 奇偶校验位 
            public byte flowcontrol;	// 流控 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] res;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_TRAN_CHAN_INFO
        {
            public byte byTranChanEnable;	// 当前透明通道是否打开 0：关闭 1：打开 	
            //	多路解码器本地有1个485串口，1个232串口都可以作为透明通道,设备号分配如下：
            //	0 RS485
            //	1 RS232 Console
            public byte byLocalSerialDevice;			// Local serial device 
            //	远程串口输出还是两个,一个RS232，一个RS485
            //	1表示232串口
            //	2表示485串口
            public byte byRemoteSerialDevice;			// Remote output serial device 
            public byte res1;							// 保留 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteDevIP;				// Remote Device IP 
            public ushort wRemoteDevPort;				// Remote Net Communication Port 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] res2;						// 保留 
            public TTY_CONFIG RemoteSerialDevCfg;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_TRAN_CHAN_CONFIG
        {
            public uint dwSize;
            public byte by232IsDualChan; // 设置哪路232透明通道是全双工的 取值1到MAX_SERIAL_NUM 
            public byte by485IsDualChan; // 设置哪路485透明通道是全双工的 取值1到MAX_SERIAL_NUM 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] res;	// 保留 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SERIAL_NUM)]
            public NET_DVR_MATRIX_TRAN_CHAN_INFO[] struTranInfo;//同时支持建立MAX_SERIAL_NUM个透明通道
        }

        //2007-12-24 Merry Christmas Eve...
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;		// DVR IP地址 	
            public ushort wDVRPort;			// 端口号 	
            public byte byChannel;			// 通道号 
            public byte byReserve;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sUserName;		// 用户名 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPassword;		// 密码 
            public uint dwPlayMode;   	// 0－按文件 1－按时间        	
            public NET_DVR_TIME StartTime;
            public NET_DVR_TIME StopTime;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sFileName;
        }

        #region 文件播放命令
        //public const int NET_DVR_PLAYSTART = 1;//开始播放
        //public const int NET_DVR_PLAYSTOP = 2;//停止播放
        //public const int NET_DVR_PLAYPAUSE = 3;//暂停播放
        //public const int NET_DVR_PLAYRESTART = 4;//恢复播放
        //public const int NET_DVR_PLAYFAST = 5;//快放
        //public const int NET_DVR_PLAYSLOW = 6;//慢放
        //public const int NET_DVR_PLAYNORMAL = 7;//正常速度
        //public const int NET_DVR_PLAYSTARTAUDIO = 9;//打开声音
        //public const int NET_DVR_PLAYSTOPAUDIO = 10;//关闭声音
        //public const int NET_DVR_PLAYSETPOS = 12;//改变文件回放的进度
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY_CONTROL
        {
            public uint dwSize;
            public uint dwPlayCmd;		// 播放命令 见文件播放命令
            public uint dwCmdParam;		// 播放命令参数 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY_STATUS
        {
            public uint dwSize;
            public uint dwCurMediaFileLen;		// 当前播放的媒体文件长度        
            public uint dwCurMediaFilePosition;	// 当前播放文件的播放位置         
            public uint dwCurMediaFileDuration;	// 当前播放文件的总时间         
            public uint dwCurPlayTime;			// 当前已经播放的时间         
            public uint dwCurMediaFIleFrames;		// 当前播放文件的总帧数         
            public uint dwCurDataType;			// 当前传输的数据类型，19-文件头，20-流数据， 21-播放结束标志         
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 72)]
            public byte[] res;
        }

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_EMAILCFG
        {	// 12 bytes 
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sPassWord;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sFromName;			// Sender //字符串中的第一个字符和最后一个字符不能是"@",并且字符串中要有"@"字符
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sFromAddr;			// Sender address 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sToName1;			// Receiver1 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sToName2;			// Receiver2 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sToAddr1;			// Receiver address1 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sToAddr2;			// Receiver address2 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sEmailServer;		// Email server address 
            public byte byServerType;			// Email server type: 0-SMTP, 1-POP, 2-IMTP…
            public byte byUseAuthen;			// Email server authentication method: 1-enable, 0-disable 
            public byte byAttachment;			// enable attachment 
            public byte byMailinterval;			// mail interval 0-2s, 1-3s, 2-4s. 3-5s
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_NEW
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_EX struLowCompression;	//定时录像
            public NET_DVR_COMPRESSION_INFO_EX struEventCompression;	//事件触发录像
        }

        //球机位置信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PTZPOS
        {
            public ushort wAction;//获取时该字段无效
            public ushort wPanPos;//水平参数
            public ushort wTiltPos;//垂直参数
            public ushort wZoomPos;//变倍参数
        }

        //球机范围信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PTZSCOPE
        {
            public ushort wPanPosMin;//水平参数min
            public ushort wPanPosMax;//水平参数max
            public ushort wTiltPosMin;//垂直参数min
            public ushort wTiltPosMax;//垂直参数max
            public ushort wZoomPosMin;//变倍参数min
            public ushort wZoomPosMax;//变倍参数max
        }

        //rtsp配置 ipcamera专用
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_RTSPCFG
        {
            public uint dwSize;         //长度
            public ushort wPort;          //rtsp服务器侦听端口
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 54)]
            public byte[] byReserve;  //预留
        }

        #region 接口参数结构

        //NET_DVR_Login()参数结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SERIALNO_LEN)]
            public string sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		//DVR报警输入个数
            public byte byAlarmOutPortNum;		//DVR报警输出个数
            public byte byDiskNum;				//DVR 硬盘个数
            public byte byDVRType;				//DVR类型, 
            public byte byChanNum;				//DVR 通道个数
            public byte byStartChan;			//起始通道号,例如DVS-1,DVR - 1
        };

        //NET_DVR_Login_V30()参数结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SERIALNO_LEN)]
            public string sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		//报警输入个数
            public byte byAlarmOutPortNum;		//报警输出个数
            public byte byDiskNum;				//硬盘个数
            public byte byDVRType;				//设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				//模拟通道个数
            public byte byStartChan;			//起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;         //语音通道数
            public byte byIPChanNum;					//最大数字通道个数  
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] byRes1;					//保留
        };

        //sdk网络环境枚举变量，用于远程升级
        enum SDK_NETWORK_ENVIRONMENT
        {
            LOCAL_AREA_NETWORK = 0,
            WIDE_AREA_NETWORK
        }

        //显示模式
        enum DISPLAY_MODE
        {
            NORMALMODE = 0,
            OVERLAYMODE
        }

        //发送模式
        enum SEND_MODE
        {
            PTOPTCPMODE = 0,
            PTOPUDPMODE,
            MULTIMODE,
            RTPMODE,
            RESERVEDMODE
        }

        //抓图模式
        enum CAPTURE_MODE
        {
            BMP_MODE = 0,		//BMP模式
            JPEG_MODE = 1		//JPEG模式 
        }

        //SDK状态信息(9000新增)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SDKSTATE
        {
            public uint dwTotalLoginNum;		//当前login用户数
            public uint dwTotalRealPlayNum;	//当前realplay路数
            public uint dwTotalPlayBackNum;	//当前回放或下载路数
            public uint dwTotalAlarmChanNum;	//当前建立报警通道路数
            public uint dwTotalFormatNum;		//当前硬盘格式化路数
            public uint dwTotalFileSearchNum;	//当前日志或文件搜索路数
            public uint dwTotalLogSearchNum;	//当前日志或文件搜索路数
            public uint dwTotalSerialNum;	    //当前透明通道路数
            public uint dwTotalUpgradeNum;	//当前升级路数
            public uint dwTotalVoiceComNum;	//当前语音转发路数
            public uint dwTotalBroadCastNum;	//当前语音广播路数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public uint[] dwRes;
        }

        //SDK功能支持信息(9000新增)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SDKABL
        {
            public uint dwMaxLoginNum;		//最大login用户数 MAX_LOGIN_USERS
            public uint dwMaxRealPlayNum;		//最大realplay路数 WATCH_NUM
            public uint dwMaxPlayBackNum;		//最大回放或下载路数 WATCH_NUM
            public uint dwMaxAlarmChanNum;	//最大建立报警通道路数 ALARM_NUM
            public uint dwMaxFormatNum;		//最大硬盘格式化路数 SERVER_NUM
            public uint dwMaxFileSearchNum;	//最大文件搜索路数 SERVER_NUM
            public uint dwMaxLogSearchNum;	//最大日志搜索路数 SERVER_NUM
            public uint dwMaxSerialNum;	    //最大透明通道路数 SERVER_NUM
            public uint dwMaxUpgradeNum;	    //最大升级路数 SERVER_NUM
            public uint dwMaxVoiceComNum;		//最大语音转发路数 SERVER_NUM
            public uint dwMaxBroadCastNum;	//最大语音广播路数 MAX_CASTNUM
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public uint[] dwRes;
        }

        //报警设备信息
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMER
        {
            public byte byUserIDValid;                 // userid是否有效 0-无效，1-有效 
            public byte bySerialValid;                 // 序列号是否有效 0-无效，1-有效 
            public byte byVersionValid;                // 版本号是否有效 0-无效，1-有效 
            public byte byDeviceNameValid;             // 设备名字是否有效 0-无效，1-有效 
            public byte byMacAddrValid;                // MAC地址是否有效 0-无效，1-有效 
            public byte byLinkPortValid;               // login端口是否有效 0-无效，1-有效 
            public byte byDeviceIPValid;               // 设备IP是否有效 0-无效，1-有效 
            public byte bySocketIPValid;               // socket ip是否有效 0-无效，1-有效 
            public int lUserID;                       // NET_DVR_Login()返回值, 布防时有效 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SERIALNO_LEN)]
            public string sSerialNumber;	// 序列号 
            public uint dwDeviceVersion;			    // 版本信息 高16位表示主版本，低16位表示次版本
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sDeviceName;		    // 设备名字 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN)]
            public byte[] byMacAddr;		// MAC地址 
            public ushort wLinkPort;                     // link tcpPort 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceIP;    			// IP地址 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sSocketIP;    			// 报警主动上传时的socket IP地址 
            public byte byIpProtocol;                  // Ip协议 0-IPV4, 1-IPV6 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] byRes2;
        }

        //硬解码显示区域参数(子结构)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DISPLAY_PARA
        {
            public int bToScreen;
            public int bToVideoOut;
            public int nLeft;
            public int nTop;
            public int nWidth;
            public int nHeight;
            public int nReserved;
        }

        //硬解码预览参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CARDINFO
        {
            public int lChannel;//通道号
            public int lLinkMode; //最高位(31)为0表示主码流，为1表示子，0－30位表示码流连接方式:0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-电话线，5－128k宽带，6－256k宽带，7－384k宽带，8－512k宽带；
            public string sMultiCastIP;
            public NET_DVR_DISPLAY_PARA struDisplayPara;
        }

        //录象文件参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FIND_DATA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//文件名
            public NET_DVR_TIME struStartTime;//文件的开始时间
            public NET_DVR_TIME struStopTime;//文件的结束时间
            public uint dwFileSize;//文件的大小
        }

        //录象文件参数(9000)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINDDATA_V30
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//文件名
            public NET_DVR_TIME struStartTime;//文件的开始时间
            public NET_DVR_TIME struStopTime;//文件的结束时间
            public uint dwFileSize;//文件的大小
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;
            public byte byLocked;//9000设备支持,1表示此文件已经被锁定,0表示正常的文件
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes;
        }

        //录象文件参数(带卡号)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINDDATA_CARD
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//文件名
            public NET_DVR_TIME struStartTime;//文件的开始时间
            public NET_DVR_TIME struStopTime;//文件的结束时间
            public uint dwFileSize;//文件的大小
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;
        }

        //录象文件查找条件结构
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FILECOND
        {
            public int lChannel;//通道号
            public uint dwFileType;//录象文件类型0xff－全部，0－定时录像,1-移动侦测 ，2－报警触发，
            //3-报警|移动侦测 4-报警&移动侦测 5-命令触发 6-手动录像
            public uint dwIsLocked;//是否锁定 0-正常文件,1-锁定文件, 0xff表示所有文件
            public uint dwUseCardNo;//是否使用卡号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNumber;//卡号
            public NET_DVR_TIME struStartTime;//开始时间
            public NET_DVR_TIME struStopTime;//结束时间
        }

        //云台区域选择放大缩小(HIK 快球专用)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_POINT_FRAME
        {
            public int xTop;     //方框起始点的x坐标
            public int yTop;     //方框结束点的y坐标
            public int xBottom;  //方框结束点的x坐标
            public int yBottom;  //方框结束点的y坐标
            public int bCounter; //保留
        }

        //语音对讲参数
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_AUDIO
        {
            public byte byAudioEncType;   //音频编码类型 0-G722; 1-G711
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] bytes;//这里保留音频的压缩参数 
        }

        #endregion

        #endregion


        [DllImport(_dllPath, EntryPoint = "NET_DVR_Init")]
        public static extern int NET_DVR_Init();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Cleanup")]
        public static extern int NET_DVR_Cleanup();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessage")]
        public static extern int NET_DVR_SetDVRMessage(uint nMessage, IntPtr hWnd);

        #region NET_DVR_SetDVRMessage的扩展
        public delegate void ExceptionCallBack(uint dwType, int lUserID, int lHandle, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetExceptionCallBack_V30")]
        public static extern int NET_DVR_SetExceptionCallBack_V30(uint nMessage, IntPtr hWnd, ExceptionCallBack fExceptionCallBack, IntPtr pUser);

        public delegate int MessCallBack(int lCommand, string sDVRIP, IntPtr pBuf, uint dwBufLen);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessCallBack")]
        public static extern int NET_DVR_SetDVRMessCallBack(MessCallBack fMessCallBack);

        public delegate int MessCallBack_EX(int lCommand, int lUserID, IntPtr pBuf, uint dwBufLen);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessCallBack_EX")]
        public static extern int NET_DVR_SetDVRMessCallBack_EX(MessCallBack_EX fMessCallBack_EX);

        public delegate int MessCallBack_NEW(int lCommand, string sDVRIP, IntPtr pBuf, uint dwBufLen, short dwLinkDVRPort);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessCallBack_NEW")]
        public static extern int NET_DVR_SetDVRMessCallBack_NEW(MessCallBack_NEW fMessCallBack_NEW);

        public delegate int MessageCallBack(int lCommand, string sDVRIP, IntPtr pBuf, uint dwBufLen, uint dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessageCallBack")]
        public static extern int NET_DVR_SetDVRMessageCallBack(MessageCallBack fMessageCallBack, uint dwUser);

        public delegate void MSGCallBack(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRMessageCallBack_V30")]
        public static extern int NET_DVR_SetDVRMessageCallBack_V30(MSGCallBack fMessageCallBack, IntPtr pUser);
        #endregion

        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetConnectTime")]
        public static extern int NET_DVR_SetConnectTime(uint dwWaitTime, uint dwTryTimes);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetReconnect")]
        public static extern int NET_DVR_SetReconnect(uint dwInterval, int bEnableRecon);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetSDKVersion")]
        public static extern uint NET_DVR_GetSDKVersion();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetSDKBuildVersion")]
        public static extern uint NET_DVR_GetSDKBuildVersion();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_IsSupport")]
        public static extern int NET_DVR_IsSupport();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartListen")]
        public static extern int NET_DVR_StartListen(string sLocalIP, ushort wLocalPort);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopListen")]
        public static extern int NET_DVR_StopListen();

        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartListen_V30")]
        public static extern int NET_DVR_StartListen_V30(string sLocalIP, ushort wLocalPort, MSGCallBack DataCallback, IntPtr pUserData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopListen_V30")]
        public static extern int NET_DVR_StopListen_V30(int lListenHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Login")]
        public static extern int NET_DVR_Login(string IP, ushort Port, string Name, string Pwd, out NET_DVR_DEVICEINFO Info);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Login_V30")]
        public static extern int NET_DVR_Login_V30(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, out Hikvision.NET_DVR_DEVICEINFO_V30 lpDeviceInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Logout")]
        public static extern int NET_DVR_Logout(int ID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Logout_V30")]
        public static extern int NET_DVR_Logout_V30(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetLastError")]
        public static extern int NET_DVR_GetLastError();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetErrorMsg")]
        public static extern string NET_DVR_GetErrorMsg(out int pErrorNo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetShowMode")]
        public static extern int NET_DVR_SetShowMode(uint dwShowType, uint colorKey);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDVRIPByResolveSvr")]
        public static extern int NET_DVR_GetDVRIPByResolveSvr(string sServerIP, ushort wServerPort, string sDVRName, ushort wDVRNameLen, string sDVRSerialNumber, ushort wDVRSerialLen, out string sGetIP);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDVRIPByResolveSvr_EX")]
        public static extern int NET_DVR_GetDVRIPByResolveSvr_EX(string sServerIP, ushort wServerPort, string sDVRName, ushort wDVRNameLen, string sDVRSerialNumber, ushort wDVRSerialLen, out string sGetIP, out uint dwPort);

        #region 预览相关接口
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RealPlay")]
        public static extern int NET_DVR_RealPlay(int ID, ref NET_DVR_CLIENTINFO Info);

        [DllImport(_dllPath, EntryPoint = "NET_DVR_RealPlay_V30")]
        public static extern int NET_DVR_RealPlay_V30(int lUserID, ref NET_DVR_CLIENTINFO lpClientInfo, RealDataCallBack_V30 fRealDataCallBack_V30, IntPtr pUser, int bBlocked);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopRealPlay")]
        public static extern int NET_DVR_StopRealPlay(int ID);
        public delegate void DrawFuncCallBack(int lRealHandle, IntPtr hDc, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RigisterDrawFun")]
        public static extern int NET_DVR_RigisterDrawFun(int lRealHandle, DrawFuncCallBack CallBack, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetPlayerBufNumber")]
        public static extern int NET_DVR_SetPlayerBufNumber(int lRealHandle, uint dwBufNum);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ThrowBFrame")]
        public static extern int NET_DVR_ThrowBFrame(int lRealHandle, uint dwNum);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetAudioMode")]
        public static extern int NET_DVR_SetAudioMode(uint dwMode);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_OpenSound")]
        public static extern int NET_DVR_OpenSound(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseSound")]
        public static extern int NET_DVR_CloseSound();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_OpenSoundShare")]
        public static extern int NET_DVR_OpenSoundShare(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseSoundShare")]
        public static extern int NET_DVR_CloseSoundShare(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Volume")]
        public static extern int NET_DVR_Volume(int lRealHandle, ushort wVolume);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SaveRealData")]
        public static extern int NET_DVR_SaveRealData(int lRealHandle, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopSaveRealData")]
        public static extern int NET_DVR_StopSaveRealData(int lRealHandle);
        public delegate void RealDataCallBack(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetRealDataCallBack")]
        public static extern int NET_DVR_SetRealDataCallBack(int lRealHandle, RealDataCallBack fRealDataCallBack, IntPtr dwUser);
        public delegate void StdDataCallBack(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetStandardDataCallBack")]
        public static extern int NET_DVR_SetStandardDataCallBack(int lRealHandle, StdDataCallBack fStdDataCallBack, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CapturePicture")]
        public static extern int NET_DVR_CapturePicture(int lRealHandle, string sPicFileName);//bmp
        #endregion

        #region 动态生成I帧
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MakeKeyFrame")]
        public static extern int NET_DVR_MakeKeyFrame(int lUserID, int lChannel);//主码流
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MakeKeyFrameSub")]
        public static extern int NET_DVR_MakeKeyFrameSub(int lUserID, int lChannel);//子码流
        #endregion

        #region 云台控制相关接口
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControl")]
        public static extern int NET_DVR_PTZControl(int lRealHandle, uint dwPTZCommand, uint dwStop);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControl_Other")]
        public static extern int NET_DVR_PTZControl_Other(int lUserID, int lChannel, uint dwPTZCommand, uint dwStop);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_TransPTZ")]
        public static extern int NET_DVR_TransPTZ(int lRealHandle, IntPtr pPTZCodeBuf, uint dwBufSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_TransPTZ_Other")]
        public static extern int NET_DVR_TransPTZ_Other(int lUserID, int lChannel, IntPtr pPTZCodeBuf, uint dwBufSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZPreset")]
        public static extern int NET_DVR_PTZPreset(int lRealHandle, uint dwPTZPresetCmd, uint dwPresetIndex);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZPreset_Other")]
        public static extern int NET_DVR_PTZPreset_Other(int lUserID, int lChannel, uint dwPTZPresetCmd, uint dwPresetIndex);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_TransPTZ_EX")]
        public static extern int NET_DVR_TransPTZ_EX(int lRealHandle, IntPtr pPTZCodeBuf, uint dwBufSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControl_EX")]
        public static extern int NET_DVR_PTZControl_EX(int lRealHandle, uint dwPTZCommand, uint dwStop);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZPreset_EX")]
        public static extern int NET_DVR_PTZPreset_EX(int lRealHandle, uint dwPTZPresetCmd, uint dwPresetIndex);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZCruise")]
        public static extern int NET_DVR_PTZCruise(int lRealHandle, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZCruise_Other")]
        public static extern int NET_DVR_PTZCruise_Other(int lUserID, int lChannel, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZCruise_EX")]
        public static extern int NET_DVR_PTZCruise_EX(int lRealHandle, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZTrack")]
        public static extern int NET_DVR_PTZTrack(int lRealHandle, uint dwPTZTrackCmd);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZTrack_Other")]
        public static extern int NET_DVR_PTZTrack_Other(int lUserID, int lChannel, uint dwPTZTrackCmd);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZTrack_EX")]
        public static extern int NET_DVR_PTZTrack_EX(int lRealHandle, uint dwPTZTrackCmd);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControlWithSpeed")]
        public static extern int NET_DVR_PTZControlWithSpeed(int lRealHandle, uint dwPTZCommand, uint dwStop, uint dwSpeed);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControlWithSpeed_Other")]
        public static extern int NET_DVR_PTZControlWithSpeed_Other(int lUserID, int lChannel, uint dwPTZCommand, uint dwStop, uint dwSpeed);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZControlWithSpeed_EX")]
        public static extern int NET_DVR_PTZControlWithSpeed_EX(int lRealHandle, uint dwPTZCommand, uint dwStop, uint dwSpeed);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetPTZCruise")]
        public static extern int NET_DVR_GetPTZCruise(int lUserID, int lChannel, int lCruiseRoute, out NET_DVR_CRUISE_RET lpCruiseRet);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZMltTrack")]
        public static extern int NET_DVR_PTZMltTrack(int lRealHandle, uint dwPTZTrackCmd, uint dwTrackIndex);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZMltTrack_Other")]
        public static extern int NET_DVR_PTZMltTrack_Other(int lUserID, int lChannel, uint dwPTZTrackCmd, uint dwTrackIndex);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZMltTrack_EX")]
        public static extern int NET_DVR_PTZMltTrack_EX(int lRealHandle, uint dwPTZTrackCmd, uint dwTrackIndex);
        #endregion

        #region 文件查找与回放
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindFile")]
        public static extern int NET_DVR_FindFile(int lUserID, int lChannel, uint dwFileType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindNextFile")]
        public static extern int NET_DVR_FindNextFile(int lFindHandle, ref NET_DVR_FIND_DATA lpFindData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindClose")]
        public static extern int NET_DVR_FindClose(int lFindHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindNextFile_V30")]
        public static extern int NET_DVR_FindNextFile_V30(int lFindHandle, ref NET_DVR_FINDDATA_V30 lpFindData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindFile_V30")]
        public static extern int NET_DVR_FindFile_V30(int lUserID, ref NET_DVR_FILECOND pFindCond);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindClose_V30")]
        public static extern int NET_DVR_FindClose_V30(int lFindHandle);
        //2007-04-16增加查询结果带卡号的文件查找
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindNextFile_Card")]
        public static extern int NET_DVR_FindNextFile_Card(int lFindHandle, ref NET_DVR_FINDDATA_CARD lpFindData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindFile_Card")]
        public static extern int NET_DVR_FindFile_Card(int lUserID, int lChannel, uint dwFileType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_LockFileByName")]
        public static extern int NET_DVR_LockFileByName(int lUserID, string sLockFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_UnlockFileByName")]
        public static extern int NET_DVR_UnlockFileByName(int lUserID, string sUnlockFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PlayBackByName")]
        public static extern int NET_DVR_PlayBackByName(int lUserID, string sPlayBackFileName, IntPtr hWnd);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PlayBackByTime")]
        public static extern int NET_DVR_PlayBackByTime(int lUserID, int lChannel, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, IntPtr hWnd);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PlayBackControl")]
        public static extern int NET_DVR_PlayBackControl(int lPlayHandle, uint dwControlCode, uint dwInValue, out uint lpOutValue);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopPlayBack")]
        public static extern int NET_DVR_StopPlayBack(int lPlayHandle);
        public delegate void PlayDataCallBack(int lPlayHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetPlayDataCallBack")]
        public static extern int NET_DVR_SetPlayDataCallBack(int lPlayHandle, PlayDataCallBack fPlayDataCallBack, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PlayBackSaveData")]
        public static extern int NET_DVR_PlayBackSaveData(int lPlayHandle, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopPlayBackSave")]
        public static extern int NET_DVR_StopPlayBackSave(int lPlayHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetPlayBackOsdTime")]
        public static extern int NET_DVR_GetPlayBackOsdTime(int lPlayHandle, out NET_DVR_TIME lpOsdTime);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PlayBackCaptureFile")]
        public static extern int NET_DVR_PlayBackCaptureFile(int lPlayHandle, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetFileByName")]
        public static extern int NET_DVR_GetFileByName(int lUserID, string sDVRFileName, string sSavedFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetFileByTime")]
        public static extern int NET_DVR_GetFileByTime(int lUserID, int lChannel, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, string sSavedFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopGetFile")]
        public static extern int NET_DVR_StopGetFile(int lFileHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDownloadPos")]
        public static extern int NET_DVR_GetDownloadPos(int lFileHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetPlayBackPos")]
        public static extern int NET_DVR_GetPlayBackPos(int lPlayHandle);
        #endregion

        #region 升级
        [DllImport(_dllPath, EntryPoint = "NET_DVR_Upgrade")]
        public static extern int NET_DVR_Upgrade(int lUserID, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetUpgradeState")]
        public static extern int NET_DVR_GetUpgradeState(int lUpgradeHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetUpgradeProgress")]
        public static extern int NET_DVR_GetUpgradeProgress(int lUpgradeHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseUpgradeHandle")]
        public static extern int NET_DVR_CloseUpgradeHandle(int lUpgradeHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetNetworkEnvironment")]
        public static extern int NET_DVR_SetNetworkEnvironment(uint dwEnvironmentLevel);
        #endregion

        #region 远程格式化硬盘
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FormatDisk")]
        public static extern int NET_DVR_FormatDisk(int lUserID, int lDiskNumber);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetFormatProgress")]
        public static extern int NET_DVR_GetFormatProgress(int lFormatHandle, out int pCurrentFormatDisk, out int pCurrentDiskPos, out int pFormatStatic);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseFormatHandle")]
        public static extern int NET_DVR_CloseFormatHandle(int lFormatHandle);
        #endregion

        #region 报警
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetupAlarmChan")]
        public static extern int NET_DVR_SetupAlarmChan(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseAlarmChan")]
        public static extern int NET_DVR_CloseAlarmChan(int lAlarmHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetupAlarmChan_V30")]
        public static extern int NET_DVR_SetupAlarmChan_V30(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseAlarmChan_V30")]
        public static extern int NET_DVR_CloseAlarmChan_V30(int lAlarmHandle);
        #endregion

        #region 语音对讲
        public delegate void VoiceDataCallBack(int lVoiceComHandle, IntPtr pRecvDataBuffer, uint dwBufSize, byte byAudioFlag, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartVoiceCom")]
        public static extern int NET_DVR_StartVoiceCom(int lUserID, VoiceDataCallBack callBack, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartVoiceCom_V30")]
        public static extern int NET_DVR_StartVoiceCom_V30(int lUserID, uint dwVoiceChan, int bNeedCBNoEncData, VoiceDataCallBack fVoiceDataCallBack, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetVoiceComClientVolume")]
        public static extern int NET_DVR_SetVoiceComClientVolume(int lVoiceComHandle, ushort wVolume);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopVoiceCom")]
        public static extern int NET_DVR_StopVoiceCom(int lVoiceComHandle);
        #endregion

        #region 语音转发
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartVoiceCom_MR")]
        public static extern int NET_DVR_StartVoiceCom_MR(int lUserID, VoiceDataCallBack fVoiceDataCallBack, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartVoiceCom_MR_V30")]
        public static extern int NET_DVR_StartVoiceCom_MR_V30(int lUserID, uint dwVoiceChan, VoiceDataCallBack fVoiceDataCallBack, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_VoiceComSendData")]
        public static extern int NET_DVR_VoiceComSendData(int lVoiceComHandle, IntPtr pSendBuf, uint dwBufSize);
        #endregion

        #region 语音广播
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientAudioStart")]
        public static extern int NET_DVR_ClientAudioStart();
        public delegate void AudioDataCallBack(IntPtr pRecvDataBuffer, uint dwBufSize, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientAudioStart_V30")]
        public static extern int NET_DVR_ClientAudioStart_V30(AudioDataCallBack fAudioDataCallBack, IntPtr pUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientAudioStop")]
        public static extern int NET_DVR_ClientAudioStop();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_AddDVR")]
        public static extern int NET_DVR_AddDVR(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_AddDVR_V30")]
        public static extern int NET_DVR_AddDVR_V30(int lUserID, uint dwVoiceChan);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DelDVR")]
        public static extern int NET_DVR_DelDVR(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DelDVR_V30")]
        public static extern int NET_DVR_DelDVR_V30(int lVoiceHandle);
        #endregion

        #region 透明通道设置
        public delegate void SerialDataCallBack(int lSerialHandle, IntPtr pRecvDataBuffer, uint dwBufSize, IntPtr dwUser);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SerialStart")]
        public static extern int NET_DVR_SerialStart(int lUserID, int lSerialPort, NetDvrDll.SerialDataCallBack fSerialDataCallBack, IntPtr dwUser);
        //485作为透明通道时，需要指明通道号，因为不同通道号485的设置可以不同(比如波特率)
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SerialSend")]
        public static extern int NET_DVR_SerialSend(int lSerialHandle, int lChannel, IntPtr pSendBuf, uint dwBufSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SerialStop")]
        public static extern int NET_DVR_SerialStop(int lSerialHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SendTo232Port")]
        public static extern int NET_DVR_SendTo232Port(int lUserID, IntPtr pSendBuf, uint dwBufSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SendToSerialPort")]
        public static extern int NET_DVR_SendToSerialPort(int lUserID, uint dwSerialPort, uint dwSerialIndex, IntPtr pSendBuf, uint dwBufSize);
        #endregion

        #region 解码 nBitrate = 16000
        [DllImport(_dllPath, EntryPoint = "NET_DVR_InitG722Decoder")]
        public static extern IntPtr NET_DVR_InitG722Decoder(int nBitrate);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ReleaseG722Decoder")]
        public static extern void NET_DVR_ReleaseG722Decoder(IntPtr pDecHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DecodeG722Frame")]
        public static extern int NET_DVR_DecodeG722Frame(IntPtr pDecHandle, IntPtr pInBuffer, IntPtr pOutBuffer);
        #endregion
        #region 编码
        [DllImport(_dllPath, EntryPoint = "NET_DVR_InitG722Encoder")]
        public static extern IntPtr NET_DVR_InitG722Encoder();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_EncodeG722Frame")]
        public static extern int NET_DVR_EncodeG722Frame(IntPtr pEncodeHandle, IntPtr pInBuffer, IntPtr pOutBuffer);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ReleaseG722Encoder")]
        public static extern void NET_DVR_ReleaseG722Encoder(IntPtr pEncodeHandle);
        #endregion

        #region 远程控制本地显示
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClickKey")]
        public static extern int NET_DVR_ClickKey(int lUserID, int lKeyIndex);
        #endregion
        #region 远程控制设备端手动录像
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartDVRRecord")]
        public static extern int NET_DVR_StartDVRRecord(int lUserID, int lChannel, int lRecordType);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopDVRRecord")]
        public static extern int NET_DVR_StopDVRRecord(int lUserID, int lChannel);
        #endregion

        #region 解码卡
        [DllImport(_dllPath, EntryPoint = "NET_DVR_InitDevice_Card")]
        public static extern int NET_DVR_InitDevice_Card(out int pDeviceTotalChan);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ReleaseDevice_Card")]
        public static extern int NET_DVR_ReleaseDevice_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_InitDDraw_Card")]
        public static extern int NET_DVR_InitDDraw_Card(IntPtr hParent, uint colorKey);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ReleaseDDraw_Card")]
        public static extern int NET_DVR_ReleaseDDraw_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RealPlay_Card")]
        public static extern int NET_DVR_RealPlay_Card(int lUserID, ref NET_DVR_CARDINFO lpCardInfo, int lChannelNum);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ResetPara_Card")]
        public static extern int NET_DVR_ResetPara_Card(int lRealHandle, ref NET_DVR_DISPLAY_PARA lpDisplayPara);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RefreshSurface_Card")]
        public static extern int NET_DVR_RefreshSurface_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClearSurface_Card")]
        public static extern int NET_DVR_ClearSurface_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RestoreSurface_Card")]
        public static extern int NET_DVR_RestoreSurface_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_OpenSound_Card")]
        public static extern int NET_DVR_OpenSound_Card(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CloseSound_Card")]
        public static extern int NET_DVR_CloseSound_Card(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetVolume_Card")]
        public static extern int NET_DVR_SetVolume_Card(int lRealHandle, ushort wVolume);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_AudioPreview_Card")]
        public static extern int NET_DVR_AudioPreview_Card(int lRealHandle, int bEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetCardLastError_Card")]
        public static extern int NET_DVR_GetCardLastError_Card();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetChanHandle_Card")]
        public static extern IntPtr NET_DVR_GetChanHandle_Card(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CapturePicture_Card")]
        public static extern int NET_DVR_CapturePicture_Card(int lRealHandle, string sPicFileName);
        //获取解码卡序列号此接口无效，改用GetBoardDetail接口获得(2005-12-08支持)
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetSerialNum_Card")]
        public static extern int NET_DVR_GetSerialNum_Card(int lChannelNum, out uint pDeviceSerialNo);
        #endregion

        #region 日志
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindDVRLog")]
        public static extern int NET_DVR_FindDVRLog(int lUserID, int lSelectMode, uint dwMajorType, uint dwMinorType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindNextLog")]
        public static extern int NET_DVR_FindNextLog(int lLogHandle, ref NET_DVR_LOG lpLogData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindLogClose")]
        public static extern int NET_DVR_FindLogClose(int lLogHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindDVRLog_V30")]
        public static extern int NET_DVR_FindDVRLog_V30(int lUserID, int lSelectMode, uint dwMajorType, uint dwMinorType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, int bOnlySmart);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindNextLog_V30")]
        public static extern int NET_DVR_FindNextLog_V30(int lLogHandle, ref NET_DVR_LOG_V30 lpLogData);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindLogClose_V30")]
        public static extern int NET_DVR_FindLogClose_V30(int lLogHandle);
        // ATM DVR
        [DllImport(_dllPath, EntryPoint = "NET_DVR_FindFileByCard")]
        public static extern int NET_DVR_FindFileByCard(int lUserID, int lChannel, uint dwFileType, int nFindType, string sCardNumber, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);
        #endregion


        //2005-09-15
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CaptureJPEGPicture")]
        public static extern int NET_DVR_CaptureJPEGPicture(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sPicFileName);
        //JPEG抓图到内存
        [DllImport(_dllPath, EntryPoint = "NET_DVR_CaptureJPEGPicture_NEW")]
        public static extern int NET_DVR_CaptureJPEGPicture_NEW(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, IntPtr sJpegPicBuffer, uint dwPicSize, out uint lpSizeReturned);

        #region 2006-02-16
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetRealPlayerIndex")]
        public static extern int NET_DVR_GetRealPlayerIndex(int lRealHandle);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetPlayBackPlayerIndex")]
        public static extern int NET_DVR_GetPlayBackPlayerIndex(int lPlayHandle);
        #endregion

        #region 2006-08-28 704-640 缩放配置
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetScaleCFG")]
        public static extern int NET_DVR_SetScaleCFG(int lUserID, uint dwScale);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetScaleCFG")]
        public static extern int NET_DVR_GetScaleCFG(int lUserID, out uint lpOutScale);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetScaleCFG_V30")]
        public static extern int NET_DVR_SetScaleCFG_V30(int lUserID, ref NET_DVR_SCALECFG pScalecfg);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetScaleCFG_V30")]
        public static extern int NET_DVR_GetScaleCFG_V30(int lUserID, ref NET_DVR_SCALECFG pScalecfg);
        #endregion

        #region 2006-08-28 ATM机端口设置
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetATMPortCFG")]
        public static extern int NET_DVR_SetATMPortCFG(int lUserID, ushort wATMPort);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetATMPortCFG")]
        public static extern int NET_DVR_GetATMPortCFG(int lUserID, out ushort LPOutATMPort);
        #endregion

        #region 2006-11-10 支持显卡辅助输出
        [DllImport(_dllPath, EntryPoint = "NET_DVR_InitDDrawDevice")]
        public static extern int NET_DVR_InitDDrawDevice();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ReleaseDDrawDevice")]
        public static extern int NET_DVR_ReleaseDDrawDevice();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDDrawDeviceTotalNums")]
        public static extern int NET_DVR_GetDDrawDeviceTotalNums();
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDDrawDevice")]
        public static extern int NET_DVR_SetDDrawDevice(int lPlayPort, uint nDeviceNum);

        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZSelZoomIn")]
        public static extern int NET_DVR_PTZSelZoomIn(int lRealHandle, ref NET_DVR_POINT_FRAME pStruPointFrame);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_PTZSelZoomIn_EX")]
        public static extern int NET_DVR_PTZSelZoomIn_EX(int lUserID, int lChannel, ref NET_DVR_POINT_FRAME pStruPointFrame);
        #endregion

        #region 解码设备DS-6001D/DS-6001F
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartDecode")]
        public static extern int NET_DVR_StartDecode(int lUserID, int lChannel, ref NET_DVR_DECODERINFO lpDecoderinfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopDecode")]
        public static extern int NET_DVR_StopDecode(int lUserID, int lChannel);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDecoderState")]
        public static extern int NET_DVR_GetDecoderState(int lUserID, int lChannel, out NET_DVR_DECODERSTATE lpDecoderState);
        //2005-08-01
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDecInfo")]
        public static extern int NET_DVR_SetDecInfo(int lUserID, int lChannel, ref NET_DVR_DECCFG lpDecoderinfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDecInfo")]
        public static extern int NET_DVR_GetDecInfo(int lUserID, int lChannel, out NET_DVR_DECCFG lpDecoderinfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDecTransPort")]
        public static extern int NET_DVR_SetDecTransPort(int lUserID, ref NET_DVR_PORTCFG lpTransPort);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDecTransPort")]
        public static extern int NET_DVR_GetDecTransPort(int lUserID, out NET_DVR_PORTCFG lpTransPort);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DecPlayBackCtrl")]
        public static extern int NET_DVR_DecPlayBackCtrl(int lUserID, int lChannel, uint dwControlCode, uint dwInValue, out uint LPOutValue, ref NET_DVR_PLAYREMOTEFILE lpRemoteFileInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StartDecSpecialCon")]
        public static extern int NET_DVR_StartDecSpecialCon(int lUserID, int lChannel, ref NET_DVR_DECCHANINFO lpDecChanInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_StopDecSpecialCon")]
        public static extern int NET_DVR_StopDecSpecialCon(int lUserID, int lChannel, ref NET_DVR_DECCHANINFO lpDecChanInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DecCtrlDec")]
        public static extern int NET_DVR_DecCtrlDec(int lUserID, int lChannel, uint dwControlCode);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_DecCtrlScreen")]
        public static extern int NET_DVR_DecCtrlScreen(int lUserID, int lChannel, uint dwControl);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDecCurLinkStatus")]
        public static extern int NET_DVR_GetDecCurLinkStatus(int lUserID, int lChannel, out NET_DVR_DECSTATUS lpDecStatus);
        #endregion

        #region 多路解码器
        //2007-11-30 V211支持以下接口 //11
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixStartDynamic")]
        public static extern int NET_DVR_MatrixStartDynamic(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DYNAMIC_DEC lpDynamicInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixStopDynamic")]
        public static extern int NET_DVR_MatrixStopDynamic(int lUserID, uint dwDecChanNum);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetDecChanInfo")]
        public static extern int NET_DVR_MatrixGetDecChanInfo(int lUserID, uint dwDecChanNum, out NET_DVR_MATRIX_DEC_CHAN_INFO lpInter);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetLoopDecChanInfo")]
        public static extern int NET_DVR_MatrixSetLoopDecChanInfo(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_LOOP_DECINFO lpInter);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetLoopDecChanInfo")]
        public static extern int NET_DVR_MatrixGetLoopDecChanInfo(int lUserID, uint dwDecChanNum, out NET_DVR_MATRIX_LOOP_DECINFO lpInter);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetLoopDecChanEnable")]
        public static extern int NET_DVR_MatrixSetLoopDecChanEnable(int lUserID, uint dwDecChanNum, uint dwEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetLoopDecChanEnable")]
        public static extern int NET_DVR_MatrixGetLoopDecChanEnable(int lUserID, uint dwDecChanNum, out uint lpdwEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetLoopDecEnable")]
        public static extern int NET_DVR_MatrixGetLoopDecEnable(int lUserID, out uint lpdwEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetDecChanEnable")]
        public static extern int NET_DVR_MatrixSetDecChanEnable(int lUserID, uint dwDecChanNum, uint dwEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetDecChanEnable")]
        public static extern int NET_DVR_MatrixGetDecChanEnable(int lUserID, uint dwDecChanNum, out uint lpdwEnable);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetDecChanStatus")]
        public static extern int NET_DVR_MatrixGetDecChanStatus(int lUserID, uint dwDecChanNum, out NET_DVR_MATRIX_DEC_CHAN_STATUS lpInter);
        //2007-12-22 增加支持接口 //18
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetTranInfo")]
        public static extern int NET_DVR_MatrixSetTranInfo(int lUserID, ref NET_DVR_MATRIX_TRAN_CHAN_CONFIG lpTranInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetTranInfo")]
        public static extern int NET_DVR_MatrixGetTranInfo(int lUserID, out NET_DVR_MATRIX_TRAN_CHAN_CONFIG lpTranInfo);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetRemotePlay")]
        public static extern int NET_DVR_MatrixSetRemotePlay(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DEC_REMOTE_PLAY lpInter);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixSetRemotePlayControl")]
        public static extern int NET_DVR_MatrixSetRemotePlayControl(int lUserID, uint dwDecChanNum, uint dwControlCode, uint dwInValue, out uint lpOutValue);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_MatrixGetRemotePlayStatus")]
        public static extern int NET_DVR_MatrixGetRemotePlayStatus(int lUserID, uint dwDecChanNum, out NET_DVR_MATRIX_DEC_REMOTE_PLAY_STATUS lpOuter);
        #endregion

        [DllImport(_dllPath, EntryPoint = "NET_DVR_RefreshPlay")]
        public static extern int NET_DVR_RefreshPlay(int lPlayHandle);
        //恢复默认值
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RestoreConfig")]
        public static extern int NET_DVR_RestoreConfig(int lUserID);
        //保存参数
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SaveConfig")]
        public static extern int NET_DVR_SaveConfig(int lUserID);
        //重启
        [DllImport(_dllPath, EntryPoint = "NET_DVR_RebootDVR")]
        public static extern int NET_DVR_RebootDVR(int lUserID);
        //关闭DVR
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ShutDownDVR")]
        public static extern int NET_DVR_ShutDownDVR(int lUserID);

        #region 参数配置
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDVRConfig")]
        public static extern int NET_DVR_GetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpOutBuffer, uint dwOutBufferSize, out uint lpBytesReturned);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetDVRConfig")]
        public static extern int NET_DVR_SetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpInBuffer, uint dwInBufferSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDVRWorkState_V30")]
        public static extern int NET_DVR_GetDVRWorkState_V30(int lUserID, out NET_DVR_WORKSTATE_V30 lpWorkState);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetDVRWorkState")]
        public static extern int NET_DVR_GetDVRWorkState(int lUserID, out NET_DVR_WORKSTATE lpWorkState);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetVideoEffect")]
        public static extern int NET_DVR_SetVideoEffect(int lUserID, int lChannel, uint dwBrightValue, uint dwContrastValue, uint dwSaturationValue, uint dwHueValue);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetVideoEffect")]
        public static extern int NET_DVR_GetVideoEffect(int lUserID, int lChannel, out uint pBrightValue, out uint pContrastValue, out uint pSaturationValue, out uint pHueValue);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientGetframeformat")]
        public static extern int NET_DVR_ClientGetframeformat(int lUserID, out NET_DVR_FRAMEFORMAT lpFrameFormat);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientSetframeformat")]
        public static extern int NET_DVR_ClientSetframeformat(int lUserID, ref NET_DVR_FRAMEFORMAT lpFrameFormat);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientGetframeformat_V30")]
        public static extern int NET_DVR_ClientGetframeformat_V30(int lUserID, out NET_DVR_FRAMEFORMAT_V30 lpFrameFormat);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientSetframeformat_V30")]
        public static extern int NET_DVR_ClientSetframeformat_V30(int lUserID, ref NET_DVR_FRAMEFORMAT_V30 lpFrameFormat);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetAlarmOut_V30")]
        public static extern int NET_DVR_GetAlarmOut_V30(int lUserID, out NET_DVR_ALARMOUTSTATUS_V30 lpAlarmOutState);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetAlarmOut")]
        public static extern int NET_DVR_GetAlarmOut(int lUserID, out NET_DVR_ALARMOUTSTATUS lpAlarmOutState);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetAlarmOut")]
        public static extern int NET_DVR_SetAlarmOut(int lUserID, int lAlarmOutPort, int lAlarmOutStatic);
        #endregion

        #region 视频参数调节
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientSetVideoEffect")]
        public static extern int NET_DVR_ClientSetVideoEffect(int lRealHandle, uint dwBrightValue, uint dwContrastValue, uint dwSaturationValue, uint dwHueValue);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_ClientGetVideoEffect")]
        public static extern int NET_DVR_ClientGetVideoEffect(int lRealHandle, out uint pBrightValue, out uint pContrastValue, out uint pSaturationValue, out uint pHueValue);
        #endregion

        #region 配置文件
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetConfigFile")]
        public static extern int NET_DVR_GetConfigFile(int lUserID, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetConfigFile")]
        public static extern int NET_DVR_SetConfigFile(int lUserID, string sFileName);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetConfigFile_V30")]
        public static extern int NET_DVR_GetConfigFile_V30(int lUserID, IntPtr sOutBuffer, uint dwOutSize, out uint pReturnSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetConfigFile_EX")]
        public static extern int NET_DVR_GetConfigFile_EX(int lUserID, IntPtr sOutBuffer, uint dwOutSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetConfigFile_EX")]
        public static extern int NET_DVR_SetConfigFile_EX(int lUserID, IntPtr sInBuffer, uint dwInSize);
        #endregion

        #region 启用日志文件写入接口
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetLogToFile")]
        public static extern int NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, int bAutoDel);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetSDKState")]
        public static extern int NET_DVR_GetSDKState(out NET_DVR_SDKSTATE pSDKState);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetSDKAbility")]
        public static extern int NET_DVR_GetSDKAbility(out NET_DVR_SDKABL pSDKAbl);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetPTZProtocol")]
        public static extern int NET_DVR_GetPTZProtocol(int lUserID, out NET_DVR_PTZCFG pPtzcfg);
        #endregion

        #region 前面板锁定
        [DllImport(_dllPath, EntryPoint = "NET_DVR_LockPanel")]
        public static extern int NET_DVR_LockPanel(int lUserID);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_UnLockPanel")]
        public static extern int NET_DVR_UnLockPanel(int lUserID);
        #endregion

        #region Rtsp设置
        [DllImport(_dllPath, EntryPoint = "NET_DVR_SetRtspConfig")]
        public static extern int NET_DVR_SetRtspConfig(int lUserID, uint dwCommand, ref NET_DVR_RTSPCFG lpInBuffer, uint dwInBufferSize);
        [DllImport(_dllPath, EntryPoint = "NET_DVR_GetRtspConfig")]
        public static extern int NET_DVR_GetRtspConfig(int lUserID, uint dwCommand, out NET_DVR_RTSPCFG lpOutBuffer, uint dwOutBufferSize);
        #endregion

    }
}
