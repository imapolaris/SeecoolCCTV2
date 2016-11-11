using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper
{


    [Serializable]
    public class UnConnectedException : Exception
    {
        public UnConnectedException() { }
        public UnConnectedException(string message) : base(message) { }
        public UnConnectedException(string message, Exception inner) : base(message, inner) { }
        protected UnConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
