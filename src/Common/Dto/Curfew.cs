using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public sealed partial class Curfew
	{
		public TimeSpan Break
		{
			get { return TimeSpan.FromSeconds(BreakValue); }
			set { BreakValue = (long)value.TotalSeconds; }
		}

		public TimeSpan Time
		{
			get { return TimeSpan.FromSeconds(TimeValue); }
			set { TimeValue = (long)value.TotalSeconds; }
		}
	}
}
