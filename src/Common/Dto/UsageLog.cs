using Atropos.Common.DateTimeConv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public sealed partial class UsageLog
	{
		public TimeSpan Started
		{
			get { return StartedValue.ToTime(); }
			set { StartedValue = value.ToDto(); }
		}

		public TimeSpan Used
		{
			get { return UsedValue.ToTime(); }
			set { UsedValue = value.ToDto(); }
		}

		public DateTime Date
		{
			get { return DateValue.ToDate(); }
			set { DateValue = value.ToDto(); }
		}

		public TimeSpan Finished
		{
			get { return FinishedValue.ToTime(); }
			set { FinishedValue = value.ToDto(); }
		}
	}
}
