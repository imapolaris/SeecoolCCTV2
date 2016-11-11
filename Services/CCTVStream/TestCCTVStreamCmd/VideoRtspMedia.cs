using Media.Rtp;
using Media.Rtsp;
using Media.Rtsp.Server.MediaTypes;
using Media.Sdp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd
{
    public class VideoRtspMedia : RFC6184Media
    {
        public VideoRtspMedia(int width, int height, string name, string directory = null, bool watch = true)
            : base(width, height, name, directory, watch)
        {
        }
        public static byte[] FullStartSequence = new byte[] { 0x00, 0x00, 0x00, 0x01 };

        public void UpdateHeader(byte[] header)
        {
            var indexArray = indexOfAll(header, FullStartSequence);
            for (int i = 0; i < indexArray.Length - 1; i++)
            {
                int len = indexArray[i + 1] - indexArray[i];
                if (len > FullStartSequence.Length)
                {
                    byte[] buffer = new byte[len];
                    Array.Copy(header, indexArray[i], buffer, 0, buffer.Length);
                    if (buffer[FullStartSequence.Length] == 0x67)
                        this.sps = buffer;
                    else if (buffer[FullStartSequence.Length] == 0x68)
                        this.pps = buffer;
                }
            }
        }

        public void UpdateHeader(byte[] sps, byte[] pps)
        {
            this.sps = sps;
            this.pps = pps;
        }

        public void AddFrame(byte[] stream)
        {
            if (stream == null || stream.Length < 8)
                return;
            if ((stream[4] & 0x1f) == 5)
            {
                add(sps.Skip(4).Concat(pps.Skip(1)).ToArray());//header
            }
            add(stream.Skip(4).ToArray());
        }

        private void add(byte[] nal)
        {
            try
            {
                RFC6184Media.RFC6184Frame frame = new RFC6184Media.RFC6184Frame(96);
                frame.Packetize(nal);
                AddFrame(frame);
            }
            catch (Exception ex)
            {
                Console.WriteLine(nal.Length + "\t "+ ex.ToString());
            }
        }

        private static int[] indexOfAll(byte[] source, byte[] comp)
        {
            List<int> indexs = new List<int>();
            int index = 0;
            while (index <= source.Length - comp.Length)
            {
                if (isOnHeader(source, comp, index))
                {
                    indexs.Add(index);
                    index += comp.Length;
                }
                else
                    index++;
            }
            if (indexs.Count > 0)
                indexs.Add(source.Length);
            return indexs.ToArray();
        }

        private static bool isOnHeader(byte[] source, byte[] comp, int index)
        {
            for (int j = 0; j < comp.Length; j++)
            {
                if (source[index + j] != comp[j])
                    return false;
            }
            return true;
        }
    }
}
