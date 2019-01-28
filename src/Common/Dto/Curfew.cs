using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public class Curfew : IdDto
	{
		public int UserId { get; set; }
		public string WeekDay { get; set; }
		public TimeSpan Time { get; set; }
		public TimeSpan Break { get; set; }
	}
}
