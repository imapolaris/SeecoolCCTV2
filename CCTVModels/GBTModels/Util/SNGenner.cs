using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Util
{
    public class SNGenner
    {
        public static SNGenner Instance { get; private set; }
        static SNGenner()
        {
            Instance = new SNGenner();
        }

        private SNGenner()
        {

        }

        private int Seed = 1;

        public int CreateSN()
        {
            return Seed++;
        }
    }
}
