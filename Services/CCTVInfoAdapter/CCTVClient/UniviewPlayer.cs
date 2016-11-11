using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class UniviewPlayer : IDisposable
    {
        public delegate void DecFrameCallback(int width, int height, byte[] frame, long timestamp);
        public event DecFrameCallback DecFrameEvent;
        private void fireDecFrameEvent(int width, int height, byte[] frame, long timestamp)
        {
            var callback = DecFrameEvent;
            if (callback != null)
                callback(width, height, frame, timestamp);
        }

        public string DecodeTag { get; private set; }

        long _timeStamp = 0;
        int _port = -1;

        public UniviewPlayer(string decodeTag)
        {
            DecodeTag = decodeTag;
        }

        public void Dispose()
        {
            if (_port >= 0)
            {
                UniviewNative.IMOS_XP_StopPlay(_port);
                UniviewNative.IMOS_XP_CloseInputStream(_port);
                _portMgr.Value.ReleasePort(_port);
            }
            _port = -1;
        }

        public void InputData(byte[] data)
        {
            if (_port < 0)
            {
                _port = _portMgr.Value.QueryPort();
                UniviewNative.IMOS_XP_OpenInputStream(_port, null);
                UniviewNative.IMOS_XP_SetDecoderTag(_port, DecodeTag);
                UniviewNative.IMOS_XP_SetDecodeVideoMediaDataCB(_port, onDecodeVideoData, false);
                UniviewNative.IMOS_XP_StartPlay(_port);
                _timeStamp = 0;
            }

            UniviewNative.IMOS_XP_InputMediaData(_port, data);
        }

        private void onDecodeVideoData(int port, int width, int height, byte[] frame, int timestampType, long timestamp)
        {
            if (timestampType == 0)
                _timeStamp += 1000 / timestamp;
            else
                _timeStamp = timestamp;

            fireDecFrameEvent(width, height, frame, _timeStamp);
        }

        class PortMgr
        {
            public PortMgr()
            {
                UniviewNative.IMOS_XP_Init();
            }

            ~PortMgr()
            {
                UniviewNative.IMOS_XP_Cleanup();
            }

            HashSet<int> _ports = new HashSet<int>();

            public int QueryPort()
            {
                for (int i = 0; true; i++)
                {
                    lock (_ports)
                    {
                        if (!_ports.Contains(i))
                        {
                            _ports.Add(i);
                            return i;
                        }
                    }
                }
            }

            public void ReleasePort(int port)
            {
                lock (_ports)
                    _ports.Remove(port);
            }
        }
        static Lazy<PortMgr> _portMgr = new Lazy<PortMgr>();
    }
}
