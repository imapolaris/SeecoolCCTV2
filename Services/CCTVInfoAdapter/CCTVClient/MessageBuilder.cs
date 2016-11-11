using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class MessageBuilder : IDisposable
    {
        public class MessageWriter : BinaryWriter
        {
            public MessageWriter(Stream stream)
                : base(stream, Encoding.Default)
            {
            }

            public override void Write(string value)
            {
                byte[] bytes = Encoding.Default.GetBytes(value);
                base.Write(bytes);
                Write((byte)0);
            }
        }

        private MemoryStream _stream = new MemoryStream();
        public MessageWriter Writer { get; private set; }

        public MessageBuilder(int messageID)
            : base()
        {
            Writer = new MessageWriter(_stream);

            Writer.Write((int)8);
            Writer.Write(messageID);
        }

        public byte[] ToMessage()
        {
            byte[] message = _stream.ToArray();
            int len = message.Length;
            byte[] bytes = BitConverter.GetBytes(len);
            Array.Copy(bytes, message, 4);
            return message;
        }

        public void Dispose()
        {
            Writer.Dispose();
            Writer = null;
            _stream.Dispose();
        }
    }
}
