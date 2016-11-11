using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class LogicalTreePersistence:BasePersistence<CCTVLogicalTree>
    {
        public static LogicalTreePersistence Instance { get; private set; }
        static LogicalTreePersistence()
        {
            Instance = new LogicalTreePersistence();
        }

        private LogicalTreePersistence():base("CCTVLogicalTree")
        {

        }
    }
}
