using CCTVReplay.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Interface
{
    public interface ISourcePersistence
    {
        int Count();
        IEnumerable<DataSource> SelectAll();
        DataSource Select(string siName);
        bool Insert(DataSource si);
        bool Update(string oldName, DataSource newSI);
        bool Delete(string siName);
    }
}
