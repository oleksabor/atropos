using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.DateTimeConv
{
	public static class DateTimeDtoConvertor
	{
		public static long ToDto(this DateTime date)
		{
			return date.ToBinary();
		}

		public static DateTime ToDate(this long value)
		{
			return DateTime.FromBinary(value);
		}

		public static long ToDto(this TimeSpan time)
		{
			return (long)time.TotalSeconds;
		}

		public static TimeSpan ToTime(this long value)
		{
			return TimeSpan.FromSeconds(value);
		}
	}
}
