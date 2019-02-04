using Atropos.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.String
{
    public static class StringExtension
    {
		/// <summary>
		/// Determines whether string value is empty.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if is empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// Determines whether string value is empty or whitespace.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if is empty or whitespace; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmptyW(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		/// <summary>
		/// Converts day of week to the string. 
		/// </summary>
		/// <param name="day">The day.</param>
		/// <returns>First two letters</returns>
		public static string AsString(this DayOfWeek day)
		{
			var d = day.ToString();
			return d.Substring(0, 2);
		}

		/// <summary>
		/// Determines whether day parameter value matches to the specified week day string.
		/// </summary>
		/// <param name="weekDayString">The week day string.</param>
		/// <param name="day">The day.</param>
		/// <returns>
		///   <c>true</c> if day parameter value matches the specified week day string; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// no value - weekDayString
		/// or
		/// failed to parse week day string '{weekDayString}
		/// </exception>
		public static bool IsWeekDay(this string weekDayString, int day, ILog log = null)
		{
			if (weekDayString.IsEmpty())
				throw new ArgumentException("no value", nameof(weekDayString));

			var days = weekDayString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var d in days)
				try
				{
					var range = d.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
					switch (range.Length)
					{
						case 2:
							var start = Parse(range[0]);
							var end = Parse(range[1]);
							if (start >= end)
								throw new ArgumentException($"start:${start} should be less than end:${end} value");

							if (day >= start && day <= end)
								return true;

							break;
						case 1:
							var singleDay = Parse(range[0]);
							if (singleDay == day)
								return true;
							break;
						default:
							log.WarnFormat("unknown range:{0} from weekDay:{1}", range, weekDayString);
							break;
					}
				}
				catch (FormatException fe)
				{
					throw new ArgumentException($"failed to parse week day string '{weekDayString}'", fe);
				}
			return false;
		}

		static int Parse(string value)
		{
			value = value.Trim();
			if (value.Length != 1)
				throw new ArgumentException($"unexpected value'${value}' length");
			return int.Parse(value);
		}
	}
}
