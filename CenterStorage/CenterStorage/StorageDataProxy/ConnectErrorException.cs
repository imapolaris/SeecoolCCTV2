using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageDataProxy
{

    [Serializable]
    public class ConnnectErrorException : Exception
    {
        public ConnnectErrorException() { }
        public ConnnectErrorException(string message) : base(message) { }
        public ConnnectErrorException(string message, Exception inner) : base(message, inner) { }
        protected ConnnectErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
