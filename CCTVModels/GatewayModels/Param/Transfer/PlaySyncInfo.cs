using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param.Transfer
{
    public class PlaySyncInfo : Packet
    {
        public int Code { get { return MessageCode.PlaySyncInfo; } }
        public string ReceiveIp { get; set; }
        public int ReceiveRTPPort { get; set; }

        public PlaySyncInfo()
        {

        }

        public PlaySyncInfo(string receiveIp, int receivePort)
        {
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
                    ReceiveIp = ReadString(br);
                    ReceiveRTPPort = br.ReadInt32();
                }
            }
        }

        public static PlaySyncInfo DeserializeObject(byte[] bytes)
        {
            PlaySyncInfo pi = new PlaySyncInfo();
            pi.Deserialize(bytes);
            return pi;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            PlaySyncInfo psi = obj as PlaySyncInfo;
            if (psi == null)
                return false;
            return psi.ReceiveIp == this.ReceiveIp && psi.ReceiveRTPPort == this.ReceiveRTPPort;
        }
    }
}
