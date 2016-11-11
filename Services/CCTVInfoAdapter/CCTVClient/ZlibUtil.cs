using System;
using zlib;

namespace Seecool.Compression.ZLib
{
    public static class ZLibUtil
    {
        public static byte[] Compress(byte[] source, int start, int size)
        {
            ZStream stream = new ZStream();
            stream.next_in = source;
            stream.next_in_index = start;
            stream.avail_in = size;

            int maxCompSize = (int)((long)size * 1001 / 1000 + 12);
            byte[] compressed = new byte[maxCompSize];
            stream.next_out = compressed;
            stream.next_out_index = 0;
            stream.avail_out = maxCompSize;

            int err = stream.deflateInit(zlibConst.Z_DEFAULT_COMPRESSION);
            if (err != zlibConst.Z_OK)
                return null;

            err = stream.deflate(zlibConst.Z_FINISH);
            if (err != zlibConst.Z_STREAM_END)
            {
                stream.deflateEnd();
                return null;
            }

            int realSize = (int)stream.total_out;

            stream.deflateEnd();

            if (realSize < compressed.Length)
            {
                byte[] temp = new byte[realSize];
                Array.Copy(compressed, temp, realSize);
                return temp;
            }
            else
                return compressed;
        }

        public static byte[] Compress(byte[] source)
        {
            return Compress(source, 0, source.Length);
        }

        public static byte[] Uncompress(byte[] compressed, int start, int size, int maxUncompressedSize)
        {
            ZStream stream = new ZStream();
            stream.next_in = compressed;
            stream.next_in_index = start;
            stream.avail_in = compressed.Length;

            byte[] uncompressed = new byte[maxUncompressedSize];
            stream.next_out = uncompressed;
            stream.next_out_index = 0;
            stream.avail_out = maxUncompressedSize;

            int err = stream.inflateInit();
            if (err != zlibConst.Z_OK)
                return null;

            err = stream.inflate(zlibConst.Z_FINISH);
            if (err != zlibConst.Z_STREAM_END)
            {
                stream.inflateEnd();
                return null;
            }

            int realSize = (int)stream.total_out;

            err = stream.inflateEnd();
            if (err != zlibConst.Z_OK)
                return null;

            if (realSize < maxUncompressedSize)
            {
                byte[] temp = new byte[realSize];
                Array.Copy(uncompressed, temp, realSize);
                return temp;
            }
            else
                return uncompressed;
        }

        public static byte[] Uncompress(byte[] compressed, int maxUncompressedSize)
        {
            return Uncompress(compressed, 0, compressed.Length, maxUncompressedSize);
        }
    }
}
