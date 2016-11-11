using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketHelper.Events;
using VideoStreamModels.Model;

namespace VideoStreamServer.Linker
{
    internal abstract class AbstractStreamLinker : IStreamLinker
    {
        public Uri StreamUri { get; protected set; }
        public IStreamHeader CurrentHeader { get; protected set; }

        public bool HasListener { get { return _streamDataReceived != null; } }

        protected abstract void Start();
        protected abstract void Stop();

        private bool _started = false;
        private void checkStart()
        {
            if (_started)
            {
                if (_streamDataReceived == null)
                {
                    Stop();
                    _started = false;
                }
            }
            else
            {
                if (_streamDataReceived != null)
                {
                    Start();
                    _started = true;
                }
            }
        }

        #region 【事件定义】
        private EventHandler<StreamData> _streamDataReceived;
        public event EventHandler<StreamData> StreamDataReceived
        {
            add
            {
                _streamDataReceived += value;
                checkStart();
            }
            remove
            {
                _streamDataReceived -= value;
                checkStart();
            }
        }

        protected virtual void OnStreamDataReceived(StreamData data)
        {
            var handler = _streamDataReceived;
            if (handler != null)
                handler(this, data);
        }

        public event EventHandler<IStreamHeader> _streamHeaderReceived;
        public event EventHandler<IStreamHeader> StreamHeaderReceived
        {
            add
            {
                _streamHeaderReceived += value;
                checkStart();
                if (_started && CurrentHeader != null)
                    value(this, CurrentHeader);
            }
            remove
            {
                _streamHeaderReceived -= value;
                checkStart();
            }
        }

        protected virtual void OnStreamHeaderRecieved(IStreamHeader header)
        {
            var handler = _streamHeaderReceived;
            if (handler != null)
                handler(this, header);
        }

        public event EventHandler<ErrorEventArgs> ErrorOccurred;
        protected virtual void OnErrorOccurred(ErrorEventArgs args)
        {
            var handler = ErrorOccurred;
            if (handler != null)
                handler(this, args);
        }
        #endregion 【事件定义】

        #region 【实现IDisposable接口】
        protected bool IsDisposed { get; private set; }
        public void Dispose()
        {
            doDisposed(true);
        }

        private void doDisposed(bool disposing)
        {
            if (!IsDisposed)
            {
                Dispose(disposing);
                IsDisposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        ~AbstractStreamLinker()
        {
            doDisposed(false);
        }
        #endregion 【实现IDisposable接口】
    }
}
