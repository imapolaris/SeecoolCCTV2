using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamClient
{
    [Serializable]
    public class InvalidServerException : Exception
    {
        public InvalidServerException() { }
        public InvalidServerException(string message) : base(message) { }
        public InvalidServerException(string message, Exception inner) : base(message, inner) { }
        protected InvalidServerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
