using CenterStorageCmd;
using StorageDataProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownload
{
    public class DownloadCmd : IDisposable
    {
        ISourceInfo _source;
        DownloadProxy _downProxy;
        public Action<Exception> ErrorEvent;
        public Action<Exception> ErrorReceived;
        public Action<DownloadInfoExpandPacket[]> DownloadInfoExpandAllReceived;
        public Action<DownloadInfoExpandPacket> DownloadInfoExpandAnyReceived;
        public Action<DownloadExpandPart> DownloadExpandPartReceived;

        public DownloadCmd(ISourceInfo source)
        {
            _source = source;
            loadProxy();
        }

        public void GetAllDownloadInfos()
        {
            _downProxy.GetAllDownloadInfos();
        }

        private void loadProxy()
        {
            _downProxy = new DownloadProxy(_source.SourceIp, _source.SourcePort);
            _downProxy.ErrorOccured += onErrorEvent;
            _downProxy.ExceptionReceived += onErrorReceived;
            _downProxy.DownloadInfoExpandAllReceived += onDownloadInfoExpandAll;
            _downProxy.DownloadInfoExpandAddReceived += onDownloadInfoExpandAny;
            _downProxy.DownloadExpandPartReceived += onDownloadExpandPart;
        }

        public void DownloadControl(DownloadControlCode code, byte[] buffer)
        {
            _downProxy?.DownloadControl(code, buffer);
        }

        private void disposeProxy()
        {
            if (_downProxy != null)
            {
                _downProxy.ErrorOccured -= onErrorEvent;
                _downProxy.ExceptionReceived -= onErrorReceived;
                _downProxy.DownloadInfoExpandAllReceived -= onDownloadInfoExpandAll;
                _downProxy.DownloadInfoExpandAddReceived -= onDownloadInfoExpandAny;
                _downProxy.DownloadExpandPartReceived -= onDownloadExpandPart;
                _downProxy.Dispose();
            }
            _downProxy = null;
        }

        private void onDownloadInfoExpandAll(DownloadInfoExpandPacket[] obj)
        {
            var handle = DownloadInfoExpandAllReceived;
            if (handle != null)
                handle(obj);
        }

        private void onDownloadInfoExpandAny(DownloadInfoExpandPacket obj)
        {
            var handle = DownloadInfoExpandAnyReceived;
            if (handle != null)
                handle(obj);
        }

        private void onDownloadExpandPart(DownloadExpandPart obj)
        {
            var handle = DownloadExpandPartReceived;
            if (handle != null)
                handle(obj);
        }

        private void onErrorEvent(Exception ex)
        {
            var handle = ErrorEvent;
            if (handle != null)
                handle(ex);
        }

        private void onErrorReceived(Exception ex)
        {
            var handle = ErrorReceived;
            if (handle != null)
                handle(ex);
        }

        public void Dispose()
        {
            disposeProxy();
        }
    }
}
