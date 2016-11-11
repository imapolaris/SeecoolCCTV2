using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamModels
{
    public interface IByteSerializer
    {
        byte[] Serialize();
        void Deserialize(byte[] bytes);
    }
}
