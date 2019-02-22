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
			get { return TimeSpan.FromSeconds(StartedValue); }
			set { StartedValue = (long)value.TotalSeconds; }
		}

		public TimeSpan Used
		{
			get { return TimeSpan.FromSeconds(UsedValue); }
			set { UsedValue = (long)value.TotalSeconds; }
		}

		public DateTime Date
		{
			get { return DateTime.FromBinary(DateValue); }
			set { DateValue = (long)value.ToBinary(); }
		}
	}
}
