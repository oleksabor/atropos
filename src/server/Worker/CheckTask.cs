using Atropos.Common;
using Atropos.Common.Logging;
using Atropos.Common.String;
using Atropos.Server.Db;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	/// <summary>
	/// check user time logic
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.DisposeGently" />
	public class CheckTask : DisposeGently
	{
		Storage _st;
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public CheckTask(Storage st)
		{
			_st = st;
		}

		public UsageResult Check(string login, DateTime date)
		{
			var usage = _st.GetUsage(login, date);
			if (usage == null)
				return new UsageResult { Kind = UsageResultKind.NoRestriction };

			var user = _st.GetUser(login);
			var curFews = _st.GetUserCurfews(user);

			var day = (int)date.DayOfWeek;

			return Check(curFews, day, usage);
		}

		/// <summary>
		/// Checks the specified cufews. Minimal curfew Time allowed is taken if several curfews are defined for the same day (use comma and dash as separators)
		/// </summary>
		/// <param name="curFews">The curfews list.</param>
		/// <param name="day">The day to check.</param>
		/// <param name="usage">The usage log.</param>
		/// <returns>Usage result variable with Kind allowed to proceed or not</returns>
		public UsageResult Check(IEnumerable<Curfew> curFews, int day, UsageLog usage)
		{
			var curfew = curFews.OrderBy(_ => _.Time).FirstOrDefault(_ => IsWeekDay(_.WeekDay, day));
			if (curfew != null)
			{
				if (curfew.Time < usage.Used)
					return new UsageResult { Kind = UsageResultKind.Blocked, Used = usage.Used };

				// not enough data to understand how many time passed since user started to use the computer
				//if (curfew.Break > TimeSpan.Zero)
				//{
				//	var beforeBreak = curfew.Time.TotalSeconds / 2;
				//	if (usage.Used > TimeSpan.FromSeconds(beforeBreak))
				//		return UsageResult.Break;
				//}
			}
			return new UsageResult { Kind = UsageResultKind.NoRestriction, Used = usage.Used };
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
		public bool IsWeekDay(string weekDayString, int day)
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
							Log.WarnFormat("unknown range:{0} from weekDay:{1}", range, weekDayString);
							break;
					}
				}
				catch (FormatException fe)
				{
					throw new ArgumentException($"failed to parse week day string '{weekDayString}'", fe);
				}
			return false;
		}

		int Parse(string value)
		{
			value = value.Trim();
			if (value.Length != 1)
				throw new ArgumentException($"unexpected value'${value}' length");
			return int.Parse(value);
		}

		public override void DisposeIt()
		{
			_st.Dispose();
		}
	}
}
