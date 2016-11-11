using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public enum DataType
    {
        SysHead = 1,        //系统头数据
        StreamData = 2,     //流数据（包括复合流或音视频分开的视频流数据）
        AudioStreamData = 3,//音频数据
        StreamDataKey = 4,  //关键帧流数据
        StopSign = 100      //结束标记
    }
}
