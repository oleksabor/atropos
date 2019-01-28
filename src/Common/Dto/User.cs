using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public class User : IdDto
	{
		public string Login { get; set; }
		public string Name { get; set; }
	}
}
