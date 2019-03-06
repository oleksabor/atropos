using Atropos.Common.DateTimeConv;
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
			get { return BreakValue.ToTime(); }
			set { BreakValue = value.ToDto(); }
		}

		public TimeSpan Time
		{
			get { return TimeValue.ToTime(); }
			set { TimeValue = value.ToDto(); }
		}
	}
}
