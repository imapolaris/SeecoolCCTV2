using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserModule;

namespace UserPlugin
{
	public class User : IUserData
	{ //
		public int Id { get; set; }
		public string UserName { get; set; }
	}
}
