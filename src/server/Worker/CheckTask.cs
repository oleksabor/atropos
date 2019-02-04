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
			var curfew = curFews.OrderBy(_ => _.Time).FirstOrDefault(_ => _.WeekDay.IsWeekDay(day, Log));
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

		public override void DisposeIt()
		{
			_st.Dispose();
		}
	}
}
