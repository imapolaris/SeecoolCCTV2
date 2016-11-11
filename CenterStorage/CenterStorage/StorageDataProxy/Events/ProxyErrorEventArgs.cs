using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageDataProxy.Events
{
    public class ProxyErrorEventArgs : EventArgs
    {
        private string _caller;
        private string _errorMsg;
        private Exception _innerEx;

        public ProxyErrorEventArgs(string method, string errorMsg, Exception innerEx)
        {
            _caller = method;
            _errorMsg = errorMsg;
            _innerEx = innerEx;
        }

        public string Method { get { return _caller; } }

        public string ErrorMessage { get { return _errorMsg; } }

        public Exception InnerException { get { return _innerEx; } }
    }
}
