using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class AnalyzePersistence : BasePersistence<CCTVVideoAnalyze>
    {
        public static AnalyzePersistence Instance { get; private set; }
        static AnalyzePersistence()
        {
            Instance = new AnalyzePersistence();
        }

        private AnalyzePersistence() : base("CCTVAnalyze")
        {
        }
    }
}
