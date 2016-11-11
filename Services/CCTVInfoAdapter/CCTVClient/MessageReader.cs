using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class MessageReader : IDisposable
    {
        public class StreamReader : BinaryReader
        {
            public StreamReader(Stream stream)
                : base(stream, Encoding.Default)
            {
            }

            public override string ReadString()
            {
                List<byte> bytes = new List<byte>();
                while (true)
                {
                    byte b = ReadByte();
                    if (b == 0)
                        break;
                    bytes.Add(b);
                }
                return Encoding.Default.GetString(bytes.ToArray());
            }
        }

        private MemoryStream _stream;
        public StreamReader Reader { get; private set; }
        public int MessageID { get; private set; }
        public int StreamLength { get { return (int)_stream.Length; } }
        public int BytesLeft { get { return (int)(_stream.Length - _stream.Position); } }

        public MessageReader(byte[] message)
        {
            _stream = new MemoryStream(message);
            Reader = new StreamReader(_stream);
            int length = Reader.ReadInt32();
            MessageID = Reader.ReadInt32();
        }

        public void Dispose()
        {
            Reader.Dispose();
            Reader = null;
            _stream.Dispose();
        }
    }
}
