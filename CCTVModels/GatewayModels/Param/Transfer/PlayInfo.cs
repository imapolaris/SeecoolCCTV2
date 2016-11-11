using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param.Transfer
{
    public class PlayInfo : Packet
    {
        public int Code { get { return MessageCode.PlayInfo; } }
        public string PlatformId { get; set; }
        public string PlayDeviceId { get; set; }
        public string ReceiveIp { get; set; }
        public int ReceiveRTPPort { get; set; }

        public PlayInfo()
        {

        }

        public PlayInfo(string platId, string playDevice, string receiveIp, int receivePort)
        {
            PlatformId = platId;
            PlayDeviceId = playDevice;
            ReceiveIp = receiveIp;
            ReceiveRTPPort = receivePort;
        }

        public override byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(Code);
                    WriteString(bw, PlatformId);
                    WriteString(bw, PlayDeviceId);
                    WriteString(bw, ReceiveIp);
                    bw.Write(ReceiveRTPPort);
                    return ms.ToArray();
                }
            }
        }

        public override void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int code = br.ReadInt32();
                    PlatformId = ReadString(br);
                    PlayDeviceId = ReadString(br);
                    ReceiveIp = ReadString(br);
                    ReceiveRTPPort = br.ReadInt32();
                }
            }
        }

        public static PlayInfo DeserializeObject(byte[] bytes)
        {
            PlayInfo pi = new PlayInfo();
            pi.Deserialize(bytes);
            return pi;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            PlayInfo pi = obj as PlayInfo;
            if (pi == null)
                return false;
            return pi.ReceiveIp == this.ReceiveIp && pi.ReceiveRTPPort == this.ReceiveRTPPort
                && pi.PlatformId == this.PlatformId && pi.PlayDeviceId == this.PlayDeviceId;
        }
    }
}
