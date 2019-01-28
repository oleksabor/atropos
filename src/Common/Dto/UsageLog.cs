using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public class UsageLog : IdDto
	{
		public int UserId { get; set; }
		public TimeSpan Used { get; set; }
		public TimeSpan Started { get; set; }
		public DateTime Date { get; set; }
	}
}
