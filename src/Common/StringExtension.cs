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
    }
}
