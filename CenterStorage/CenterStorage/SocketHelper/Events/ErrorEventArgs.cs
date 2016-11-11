using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public class ErrorEventArgs:EventArgs
    {
        private string _msg;
        private int _code = 0;
        private ErrorTypes _eType;
        private Exception _inner;

        public ErrorEventArgs(string errorMsg,ErrorTypes errorType)
        {
            _msg = errorMsg;
            _eType = errorType;
        }

        public ErrorEventArgs(string errorMsg, ErrorTypes errorType, int errorCode, Exception innerEx) : this(errorMsg,errorType)
        {
            _inner = innerEx;
            _code = errorCode;
        }

        public string ErrorMessage { get { return _msg; } }
        public int SocketErrorCode { get { return _code; } }
        public Exception InnerException { get { return _inner; } }
        public ErrorTypes ErrorType { get { return _eType; } }
    }
}
