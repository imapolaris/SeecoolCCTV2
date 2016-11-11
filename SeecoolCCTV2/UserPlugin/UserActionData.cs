using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPlugin
{
	public class UserActionData
	{
		public string UserName { get; set; }		
		public DateTime Time { get; set; }	
		public UserActionType ActionType { get; set; }
		public string Name { get; set; }
		public string ToolTip { get; set; }
		public string Content { get; set; }
	}

	public enum UserActionType
	{
		ButtonClick,
		MenuClick,
	}
}
