using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class IdNamePair
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IdNamePair()
        {

        }

        public IdNamePair(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
