using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using SocketHelper;
using VideoStreamModels;
using VideoStreamModels.Model;
using VideoStreamModels.Util;
using VideoStreamServer.Linker;

namespace VideoStreamServer
{
    internal class StreamPipe : IDisposable
    {
        public string Id { get; private set; }

        private SocketAdapter _adapter;
        private IStreamLinker _linker;
        public StreamPipe(SocketAdapter adapter)
        {
            Id = Guid.NewGuid().ToString();
            _adapter = adapter;
            _adapter.Closed += _adapter_Closed;
            _adapter.ErrorOccured += _adapter_ErrorOccured;
            _adapter.ReceiveCompleted += _adapter_ReceiveCompleted;
        }

        private void _adapter_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            string errMsg = "";
            if (_linker != null)
            {
                errMsg = "URL:" + _linker.StreamUri + ";";
            }
            errMsg += args.ErrorMessage;
            Logger.Default.Error(errMsg);
            _adapter.Close();
        }

        private void _adapter_Closed(object sender, EventArgs e)
        {
            destoryLinker();
            OnAbandoned();
        }

        private void _adapter_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLenght >= 4)
            {
                int code = BitConverter.ToInt32(args.ReceivedBytes, 0);
                switch (code)
                {
                    case StreamEntityCode.StreamAddress:
                        {
                            StreamAddress sa = StreamAddress.DeserializeTo(args.ReceivedBytes);
                            destoryLinker();
                            Console.WriteLine("Uri:" + sa.Url);
                            _linker = StreamLinkerManager.Instance.GetStreamLinker(sa);
                            if (_linker != null)
                            {
                                _linker.StreamHeaderReceived += linker_StreamHeaderReceived;
                                _linker.StreamDataReceived += linker_StreamDataReceived;
                                _linker.ErrorOccurred += linker_ErrorOccurred;
                            }
                        }
                        break;
                }
            }
        }

        private void linker_ErrorOccurred(object sender, SocketHelper.Events.ErrorEventArgs e)
        {
            RemoteError re = new RemoteError(e.ErrorMessage);
            if (_adapter.IsConnected)
                _adapter.Send(re.Serialize());
            destoryLinker();
        }

        private void linker_StreamHeaderReceived(object sender, IStreamHeader e)
        {
            try
            {
                if (_adapter.IsConnected)
                    _adapter.Send(e.Serialize());
            }
            catch
            {
                destoryLinker();
            }
        }

        private void linker_StreamDataReceived(object sender, StreamData data)
        {
            try
            {
                if (_adapter.IsConnected)
                    _adapter.Send(data.Serialize());
            }
            catch { destoryLinker(); }
        }

        object _desObj = new object();
        private void destoryLinker()
        {
            lock (_desObj)
            {
                if (_linker != null)
                {
                    _linker.StreamDataReceived -= linker_StreamDataReceived;
                    _linker.StreamHeaderReceived -= linker_StreamHeaderReceived;
                    _linker.ErrorOccurred -= linker_ErrorOccurred;
                    _linker.Dispose();
                    _linker = null;
                }
            }
        }

        #region 【事件定义】
        public event EventHandler Abandoned;
        private void OnAbandoned()
        {
            var handler = Abandoned;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion 【事件定义】

        #region 【实现IDisposable接口】
        protected bool IsDisposed { get; private set; }
        public void Dispose()
        {
            doDispose(true);
        }

        private void doDispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Dispose(disposing);
                IsDisposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _adapter.Closed -= _adapter_Closed;
            _adapter.ErrorOccured -= _adapter_ErrorOccured;
            _adapter.ReceiveCompleted -= _adapter_ReceiveCompleted;
            _adapter.Close();
            destoryLinker();
        }

        ~StreamPipe()
        {
            doDispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}
